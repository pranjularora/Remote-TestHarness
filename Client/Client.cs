///////////////////////////////////////////////////////////////////////////////////
// Client.cs - client                                                            //
// ver 1.0                                                                       //
// Application: CSE681 - Software Modelling and Analysis, Project #4             //
// Author:      Pranjul Arora, Syracuse University,                              //
//              parora01@syr.edu, (315) 949-9061                                 //
///////////////////////////////////////////////////////////////////////////////////
/*
 *
 * 
 * 
 * Module Operations:
 * ==================
 * The functionality of this module is:
 * ==> Opens a port for listening.
 * ==> send DLL to repository
 * ==> send test request to the test harness server
 * ==> displays message after receiving from repository or test harness server
 * 
 * Function Operations:
 * ===================
 *public Client()
 * initialises receiver channel and creates channels for listening
 * 
 * static IBasicService CreateProxy(string url)
 * factory channel for sending messages
 * 
 *  Messages makeMessage(string author, string fromEndPoint, string toEndPoint)
 *   constructing a basic message without body
 * 
 * Main() function:
 * starting point of the client
 * 
 * 
 * public void wait()
 * wait for receiver thread to end

 * void rcvThreadProc()
 * After message gets dequeued, this function gets the meesage from repository or test harness server
 * check the type of message and perform the required functionality as per the type of the message.
 * 
 * Public Classes:
 * ===================
 *  class client
 * 
 *
 * 
 * 
 *  
 *  Maintanence History:
 * --------------------
 * ver 1.0 : 23 Nov 2016
 * Project #4 client
 * 
 *  
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;
using Client_File_Manager;

using ClientMessage;
using UtilitiesExtension;
using MessageCreate;
using ClientCommunication;
// client -- end point 8081
namespace Client
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class Client : IStreamService
    {
        int BlockSize = 1024;
        byte[] block;
        HRTimer.HiResTimer hrt = null;
        string filename;
        string savePath = @"..\..\..\..\Client-Files\Logs";
        public Comm<Client> comm { get; set; } = new Comm<Client>();

        public string endPoint { get; } = Comm<Client>.makeEndPoint("http://localhost", 8081);

        private Thread rcvThread = null;

        public Client()
        {
            block = new byte[BlockSize];
            hrt = new HRTimer.HiResTimer();
            
        }

        public void initialize()
        {
            comm.rcvr.CreateRecvChannel(endPoint);
            rcvThread = comm.rcvr.start(rcvThreadProc);
        }

        // constructing a basic message without body
        public Messages makeMessage(string author, string fromEndPoint, string toEndPoint)
        {
            Messages message = new Messages();
            message.author = author;
            message.fromUrl = fromEndPoint;
            message.toUrl = toEndPoint;
            return message;
        }
        public void wait()
        {
            rcvThread.Join();
        }
        void rcvThreadProc()
        {
            while (true)
            {
                Messages msg = comm.rcvr.GetMessage();
                msg.time = DateTime.Now;
                Console.Write("\n  {0} received message:", comm.name);
                if (msg.command == "TestResult")
                {
                    (" [[ Displaying Requirement 6 ---->>> Test Harness Server Sent back message of type: " + msg.command + " --> demonstrating test results  ]]").title('=');
                    msg.showLogs();
                    var dict = msg.message_content.Split(';');
                    string temp = null;
                    Console.WriteLine("    body:");
                    foreach (string line in dict)
                    {
                        temp = line.Replace(",", "\t:");
                        Console.Write("{0}\n", temp);
                    }
                }
                if (msg.command == "Error")
                {
                    (" [[ Displaying Requirement 3 ---->>> Test Harness Server Sent back message of type: " + msg.command + "  ]]").title('=');
                    msg.show();
                }
                if (msg.command == "Notification")
                {
                    (" [[ Displaying Requirement 9 ]]").title('=');
                    msg.show();
                }
                if (msg.message_content == "quit")
                    break;
            }
        }
        // For Receiving from Client
        static ServiceHost ReceiverCreateServiceChannel(string url)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.TransferMode = TransferMode.Streamed;
            binding.MaxReceivedMessageSize = 500000000;
            Uri baseAddress = new Uri(url);
            Type service = typeof(Client);
            ServiceHost host = new ServiceHost(service, baseAddress);
            host.AddServiceEndpoint(typeof(IStreamService), binding, baseAddress);
            return host;
        }
        public void upLoadFile(FileTransferMessage msg)
        {
            int totalBytes = 0;
            hrt.Start();
            filename = msg.fileName;
            string rfilename = Path.Combine(savePath, filename);
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            using (var outputStream = new FileStream(rfilename, FileMode.Create))
            {
                while (true)
                {
                    int bytesRead = msg.transferStream.Read(block, 0, BlockSize);
                    totalBytes += bytesRead;
                    if (bytesRead > 0)
                        outputStream.Write(block, 0, bytesRead);
                    else
                        break;
                }
            }
            hrt.Stop();
            Console.Write(
              "\n  Received file \"{0}\" of {1} bytes in {2} microsec.",
              filename, totalBytes, hrt.ElapsedMicroseconds
            );
        }

        public void receiveFiles()
        {
            ServiceHost host = ReceiverCreateServiceChannel("http://localhost:8002/StreamService");

            host.Open();
            Console.Write("\n  SelfHosted File Stream Service started");
            Console.Write("\n ========================================\n");
            Console.Write("\n  Press key to terminate service:\n");
            //  Console.ReadKey();
            Console.Write("\n");
            //   host.Close();
        }
        static void Main(string[] args)
        {
            try
            {
                Console.Write("\n CLIENT");
                Console.Write("\n =====================\n");
                Client client = new Client(); // by calling its constructor, the client starts listening on port 8081
                // And a reciever thread gets ready retrieving messages from Receiver Blocking Queue
                client.initialize();
                ClientFileManager fileManager = new ClientFileManager();
                //////// Implementing code for sending DLL's to repository before sending the test request.
                // before sending test request to Test Harness, client_Manager will upload files to Repository
                fileManager.client_Manager("client");
                // creating messages to send to Test Harness Server
                Messages msg = client.makeMessage("Pranjul", client.endPoint, client.endPoint);
                string TestHarnessEndPoint = Comm<Client>.makeEndPoint("http://localhost", 8080);
                msg.toUrl = TestHarnessEndPoint;
                msg.command = "TestRequest";
                msg.message_content = ClientMessages.makeTestRequest();
                " [[ Displaying Requirement 2 and 6 (Test Request) ---->>> XML Body sent as a Request specifies, DLL's to execute  ]]".title('=');
                msg.show();
                Console.WriteLine("----------------------------------------------------------------------------------");
                client.comm.sndr.PostMessage(msg);

                (" [[ Displaying Requirement 9--> requesting files from repository]]").title('=');
                Messages msgToRepo = client.makeMessage("Client-Logs_Repository", client.endPoint, client.endPoint);
                string RepositoryEndPoint = Comm<Client>.makeEndPoint("http://localhost", 8082);
                msgToRepo.toUrl = RepositoryEndPoint;
                msgToRepo.command = "SendAllLogs";
                msgToRepo.show();
                client.comm.sndr.PostMessage(msgToRepo);
                client.receiveFiles();
                Console.ReadKey();
                client.wait();
                Console.Write("\n\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught {0}", ex.Message);
            }
            Console.ReadKey();
        }
    }
}

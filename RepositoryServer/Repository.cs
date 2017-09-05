///////////////////////////////////////////////////////////////////////////////////
// Repository.cs - Repository Server Functionality                               //
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
 * ==> accepts test request from multiple clients, but process on at a time
 * ==> and execute them using a single child thread
 * 
 * Function Operations:
 * ===================
 * public void initialize()
 * initializes the object, the objects initialized in this can not be initialized in the constructor as they raise conflict for communication channel objects.
 * 
 * 
 * public Repository() 
 * initialized HR timer object and block object
 * 
 * public void upLoadFile(FileTransferMessage msg)
 * this function helps file transfer from test harness to repository and client
 * 
 * static ServiceHost ReceiverCreateServiceChannel(string url)
 * adds a host for file streaming object
 * 
 * public void wait()
 * ensures that receiver thread completes its functionality
 *
 * void rcvThreadProc()
 * call processing message function
 * 
 * void ProcessMessage(Messages receivedMessage)
 * function to process messages of type DLL_Request
 * creates temporary directory 
 * send requested files if available, else send message
 * deletes directory
 * 
 * 
 * 
 * 
 * public Messages makeMessage(string author, string fromEndPoint, string toEndPoint)
 * helps creating a message
 * 
 *
 * 
 * public void receiveFiles()
 * open a channel of file streaming
 * 
 * 
 * 
 * Main() function:
 * starting point of the repository server
 * 
 * Public Classes:
 * ===================
 * -- class Repository : IStreamService
 * --> defines all above stated functions 
 * 
 *
 *  
 *  Maintanence History:
 * --------------------
 * ver 1.0 : 23 Nov 2016
 * Project #4 Test Harness Server Functionality  
 * 
 *  
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ClientCommunication;
using RepoFileManager;
using MessageCreate;
using System.ServiceModel;
using UtilitiesExtension;
using System.IO;
using System.Text.RegularExpressions;

namespace RepositoryServer
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class Repository : IStreamService
    {
        int BlockSize = 1024;
        byte[] block;
        HRTimer.HiResTimer hrt = null;
        string filename;
        string savePath = @"..\..\..\..\Repository-Files\Repository\DLL";
        string savePath_Logs = @"..\..\..\..\Repository-Files\Repository\Logs";

        private Comm<Repository> comm { get; set; }

        private string endPoint;

        private Thread rcvThread = null;

        public Repository() // constructor
        {
            block = new byte[BlockSize];
            hrt = new HRTimer.HiResTimer();
        }

        public void initialize() // initialize function to initialize objects
        {
            comm = new Comm<Repository>();
            endPoint = Comm<Repository>.makeEndPoint("http://localhost", 8082); // 8082 is Repositpry server end point
            comm.rcvr.CreateRecvChannel(endPoint);
            rcvThread = comm.rcvr.start(rcvThreadProc);
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
                msg.show();
                // now message needs to be process
                ProcessMessage(msg);
                if (msg.message_content == "quit")
                    break;
            }
        }
        // used for message of type DLL request 
        void ProcessMessage(Messages receivedMessage)
        {
            if (receivedMessage.command == "DLL_Request")
            {
                HashSet<string> filesList = receivedMessage.message_content.FromXml<HashSet<string>>();
                repoFileManager fileManager = new repoFileManager();
                string toSendFilesPath = fileManager.Repo_Manager(filesList);// path of local directory created to send to Test Harness Server
                Messages fileStatusToTH = makeMessage(receivedMessage.author, receivedMessage.toUrl, receivedMessage.fromUrl); // from and to URL gets reversed as the message needs to
                fileStatusToTH.command = "Notification";
                if (toSendFilesPath == null)
                {// if all files are not found, a null will be returned
                    Console.WriteLine("All requested DLLs are Not found ");
                    fileStatusToTH.message_content = "All Requested DLLs are not in the Repository \n \n ================================ Sending Notification to Test Harness Server ===============================\n";
                }
                else
                {
                    Console.WriteLine("found and are at {0} \n\n ================================ Sending Notification to Test Harness Server ===============================\n ", toSendFilesPath);
                    fileStatusToTH.message_content = "All Requested DLLs Found";
                    fileManager.assignChannel("http://localhost:8000/StreamService");
                    fileManager.callUploader(filesList, toSendFilesPath);
                }
                fileStatusToTH.show();
                comm.sndr.PostMessage(fileStatusToTH);
            }
            else if (receivedMessage.command == "SendAllLogs")
            {
                repoFileManager fileManager1 = new repoFileManager();
                fileManager1.assignChannel("http://localhost:8002/StreamService");
                HashSet<string> filesList = new HashSet<string>();
                int numRetries = 10, waitMilliSec = 50;  // search all files from repository
                for (int i = 0; i < numRetries; i++) // retry 10 times incase of a failure
                {
                    try
                    {
                        if (!Directory.Exists(savePath_Logs))
                        {
                            sendReplyToClient(receivedMessage);
                            break;
                        }
                        string[] files = Directory.GetFiles(savePath_Logs, "*.*", SearchOption.AllDirectories);
                        foreach (string file in files)
                            fileManager1.uploadFile(Path.GetFileName(file), Path.GetDirectoryName(file));
                    }
                    catch (Exception ex)
                    {
                        Thread.Sleep(waitMilliSec);
                        Console.Write("\n  Error: \"{0}\" \nRetry attempt: {1} ", ex.Message, i + 1);
                    }
                }
            }
        }
        public void sendReplyToClient(Messages msg)
        {
            Messages fileStatusToClient = makeMessage("Repository", msg.toUrl, msg.fromUrl); // from and to URL gets reversed as the message needs to
            fileStatusToClient.command = "Notification";

            fileStatusToClient.message_content = "No Log Files Found";
            comm.sndr.PostMessage(fileStatusToClient);
        }

        // creates a basic message without body
        public Messages makeMessage(string author, string fromEndPoint, string toEndPoint)
        {
            Messages message = new Messages();
            message.author = author;
            message.fromUrl = fromEndPoint;
            message.toUrl = toEndPoint;
            return message;
        }
        // For Receiving from Client
        static ServiceHost ReceiverCreateServiceChannel(string url)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.TransferMode = TransferMode.Streamed;
            binding.MaxReceivedMessageSize = 500000000;
            Uri baseAddress = new Uri(url);
            Type service = typeof(Repository);
            ServiceHost host = new ServiceHost(service, baseAddress);
            host.AddServiceEndpoint(typeof(IStreamService), binding, baseAddress);
            return host;
        }
        // sending files 
        public void upLoadFile(FileTransferMessage msg)
        {
            int totalBytes = 0;
            hrt.Start();
            filename = msg.fileName;
            string location;
            int flag = 0;

            if (filename.Contains(".txt"))
            {
                string dirName = filename;
                dirName = dirName.Split('-')[1];
                string name = Regex.Replace(dirName, "[0-9]", "");
                name = Regex.Replace(name, " ", "");
                string timeStamp = DateTime.Now.ToString("MM-dd-yyyy") + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + " " + DateTime.Now.ToString("tt");
                name += timeStamp;
                location = Path.Combine(savePath_Logs, name);
                flag = 1;

            }
            else
                location = savePath;

            string rfilename = Path.Combine(location, filename);
            if (!Directory.Exists(location))
                Directory.CreateDirectory(location);
            using (var outputStream = new FileStream(rfilename, FileMode.OpenOrCreate))
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

            if (flag == 1)
            {
                (" [[ Displaying Requirement 7 ---->>> Received Logs from the Test Harness Server ]]").title('-');
                Console.Write("\n  Received file \"{0}\" of {1} bytes in {2} microsec.", filename, totalBytes, hrt.ElapsedMicroseconds);
                (" [[ Displaying Requirement 8 ---->>> Stored Logs at Location: " + location + " ]]").title('-');
            }
            else
                Console.Write("\n  Received file \"{0}\" of {1} bytes in {2} microsec.", filename, totalBytes, hrt.ElapsedMicroseconds);
        }

        // opens up a channel when files needs to be received
        public void receiveFiles()
        {
            ServiceHost host = ReceiverCreateServiceChannel("http://localhost:8001/StreamService");

            host.Open();
            Console.Write("\n  SelfHosted File Stream Service started");
            Console.Write("\n ========================================\n");
            Console.Write("\n");
        }


        // starting point of the repository server
        static void Main(string[] args)
        {
            Console.Write("\n  REPOSITORY SERVER");
            Console.Write("\n =====================\n");
            Repository repoServer = new Repository();
            repoServer.initialize();
            repoServer.receiveFiles();


            Console.ReadKey();

            Console.Write("\n\n");
        }
    }
}

///////////////////////////////////////////////////////////////////////////////////
// Client2.cs - client                                                            //
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
 *public Client2()
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
 * No public class 
 * 
 *
 * 
 * 
 *  
 *  Maintanence History:
 * --------------------
 * ver 1.0 : 23 Nov 2016
 * Project #4 client2
 * 
 *  
 */


// client2 -- end point 8085

using Client_File_Manager;
using ClientCommunication;
using MessageCreate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using ClientMessage;
using UtilitiesExtension;

namespace Client2
{
    class Client2
    {
        private Comm<Client2> comm { get; set; } = new Comm<Client2>();

        private string endPoint { get; } = Comm<Client2>.makeEndPoint("http://localhost", 8085);

        private Thread rcvThread = null;

        public Client2()
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
                if (msg.command == "Display")
                {
                    (" [[ Displaying Requirement 6 ---->>> Test Harness Server Sent back message of type: " + msg.command + " --> demonstrating test results  ]]").title('=');
                    if (msg.message_content != null)
                    {
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

                }
                if (msg.command == "Error")
                {
                    (" [[ Displaying Requirement 3 ---->>> Test Harness Server Sent back message of type: " + msg.command + "  ]]").title('=');
                    msg.show();
                }
                if (msg.message_content == "quit")
                    break;
            }
        }
        static void Main(string[] args)
        {
            try
            {
                Console.Write("\n CLIENT2");
                Console.Write("\n =====================\n");

                Client2 client2 = new Client2();

                ClientFileManager fileManager = new ClientFileManager();
                //////// Implementing code for sending DLL's to repository before sending the test request.

                fileManager.client_Manager("client2");

                // creating messages to send to Test Harness Server
                Messages msg = client2.makeMessage("Ashu", client2.endPoint, client2.endPoint);
                string TestHarnessEndPoint = Comm<Client2>.makeEndPoint("http://localhost", 8080);
                msg.toUrl = TestHarnessEndPoint;
                msg.command = "TestRequest";
                msg.message_content = ClientMessages.makeTestRequestClient2();
                " [[ Displaying Requirement 2 and 6 (Test Request)---->>> XML Body sent as a part of Request and specifies DLL's to execute  ]]".title('=');
                msg.show();
                Console.WriteLine("----------------------------------------------------------------------------------");
                client2.comm.sndr.PostMessage(msg);

                Console.ReadKey();

                Console.Write("\n  press key to exit: ");
                Console.ReadKey();

                client2.wait();
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

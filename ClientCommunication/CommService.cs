///////////////////////////////////////////////////////////////////////////////////
// CommService.cs - Communicator Service                                         //
// ver 1.0                                                                       //
// Application: CSE681 - Software Modelling and Analysis, Project #4             //
// Author:      Pranjul Arora, Syracuse University,                              //
//              parora01@syr.edu, (315) 949-9061                                 //
///////////////////////////////////////////////////////////////////////////////////
/*
 *
 * Package Operations:
 * -------------------
 * This package defindes a Sender class and Receiver class that
 * manage all of the details to set up a WCF channel.
 * 
 * 
 * 
 *  Maintanence History:
 * --------------------
 * ver 1.0 : 23 Nov 2016
 * Project #4 client2
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BlockingQueue;
using System.ServiceModel;
using MessageCreate;
using UtilitiesExtension;

namespace ClientCommunication
{

    public class FileTransfer
    {
        public static IStreamService SenderCreateServiceChannel(string url)
        {
            // by default security mode is None, so need of this check the code removing this line
            BasicHttpSecurityMode securityMode = BasicHttpSecurityMode.None;

            BasicHttpBinding binding = new BasicHttpBinding(securityMode);
            binding.TransferMode = TransferMode.Streamed;
            binding.MaxReceivedMessageSize = 500000000;
            EndpointAddress address = new EndpointAddress(url);

            ChannelFactory<IStreamService> factory = new ChannelFactory<IStreamService>(binding, address);
            return factory.CreateChannel();
        }

        


    }


    ///////////////////////////////////////////////////////////////////
    // Receiver hosts Communication service used by other Peers
    public class Receiver<T> : ICommunicator
    {
        static BlockingQueue<Messages> rcvBlockingQ = null;
        ServiceHost service = null;

        public string name { get; set; }

        public Receiver()
        {
            if (rcvBlockingQ == null)
                rcvBlockingQ = new BlockingQueue<Messages>();
        }

        public Thread start(ThreadStart rcvThreadProc)
        {
            Thread rcvThread = new Thread(rcvThreadProc);
            rcvThread.Start();
            return rcvThread;
        }
        public void close()
        {
            service.Close();
        }
        //  Create ServiceHost for Communication service
        public void CreateRecvChannel(string address)
        {
            WSHttpBinding binding = new WSHttpBinding();
            Uri baseAddress = new Uri(address);
            service = new ServiceHost(typeof(Receiver<T>), baseAddress);
            service.AddServiceEndpoint(typeof(ICommunicator), binding, baseAddress);
            service.Open();
            Console.Write("\n  Service is open listening on {0}", address);
            " [[ Displaying Requirement 6 and 10 (***** WCF communication *****)---->>> Listening started-- asynchronous channel (not blocked waiting) ]]".title('=');
        }
        // Implement service method to receive messages from other Peers
        public void PostMessage(Messages msg)
        {
            rcvBlockingQ.enQ(msg);
        }
        // Implement service method to extract messages from other Peers.
        // This will often block on empty queue, so user should provide
        // read thread.
        public Messages GetMessage()
        {
            Messages msg = rcvBlockingQ.deQ();
            Console.Write("\n  {0} dequeuing message from {1}", name, msg.fromUrl);
            return msg;
        }
    }


    ///////////////////////////////////////////////////////////////////
    // Sender is client of another Peer's Communication service
    public class Sender
    {
        public string name { get; set; }

        ICommunicator channel;
        BlockingQueue<Messages> sndBlockingQ = null;
        Thread sndThrd = null;
        int tryCount = 0, MaxCount = 10;
        string currEndpoint = "";
        //----< processing for send thread >-----------------------------

        void ThreadProc()
        {
            tryCount = 0;
            while (true)
            {
                Messages msg = sndBlockingQ.deQ();
                if (msg.toUrl != currEndpoint)
                {
                    currEndpoint = msg.toUrl;
                    CreateSendChannel(currEndpoint);
                }
                while(true)
                {
                    try
                    {
                        channel.PostMessage(msg);
                        Console.Write("\n  posted message from {0} to {1}", name, msg.toUrl);
                        tryCount = 0;
                        break;
                    }
                    catch(Exception)
                    {
                        Console.Write("\n Connection Failed");
                        if (++tryCount < MaxCount)
                            Thread.Sleep(100);
                        else
                        {
                            Console.Write("\n  {0}", "can't connect\n");
                            currEndpoint = "";
                            tryCount = 0;
                            break;
                        }
                    }
                }
                if (msg.message_content == "quit")
                    break;
            }
        }
        //----< initialize Sender >--------------------------------------
        public Sender()
        {
            sndBlockingQ = new BlockingQueue<Messages>();
            sndThrd = new Thread(ThreadProc);
            sndThrd.IsBackground = true;
            sndThrd.Start();
        }
        //----< posts message to another Peer's queue >------------------
        /*
         *  This is a non-service method that passes message to
         *  send thread for posting to service.
         */
        public void PostMessage(Messages msg)
        {
            sndBlockingQ.enQ(msg);
        }
        //----< Create proxy to another Peer's Communicator >------------
        public void CreateSendChannel(string address)
        {
            EndpointAddress baseAddress = new EndpointAddress(address);
            WSHttpBinding binding = new WSHttpBinding();
            ChannelFactory<ICommunicator> factory = new ChannelFactory<ICommunicator>(binding,address);
            channel = factory.CreateChannel();
            Console.Write("\n  service proxy created for {0}", address);
        }
    }
    ///////////////////////////////////////////////////////////////////
    // Comm class simply aggregates a Sender and a Receiver
    //
    public class Comm<T>
    {
        public string name { get; set; } = typeof(T).Name;

        public Receiver<T> rcvr { get; set; } = new Receiver<T>();

        public Sender sndr { get; set; } = new Sender();
        public Comm()
        {
            rcvr.name = name;
            sndr.name = name;
        }
        public static string makeEndPoint(string url, int port)
        {
            string endPoint = url + ":" + port.ToString() + "/ICommunicator";
            return endPoint;
        }

        //----< this thrdProc() used only for testing, below >-----------

        public void thrdProc()
        {
            while (true)
            {
                Messages msg = rcvr.GetMessage();
                msg.show();
                if (msg.message_content == "quit")
                {
                    break;
                }
            }
        }

    }

    // test stub
    class Cat { }
    class CommService
    {
        static void Main(string[] args)
        {
            Comm<Cat> comm = new Comm<Cat>();
            string endPoint = Comm<Cat>.makeEndPoint("http://localhost", 8080);
            comm.rcvr.CreateRecvChannel(endPoint);
            comm.rcvr.start(comm.thrdProc);
            comm.sndr = new Sender();
            comm.sndr.CreateSendChannel(endPoint);
            Messages msg1 = new Messages();
            msg1.message_content = "Message #1";
            comm.sndr.PostMessage(msg1);
            Messages msg2 = new Messages();
            msg2.message_content = "quit";
            comm.sndr.PostMessage(msg2);
            Console.Write("\n  Comm Service Running:");
            Console.Write("\n  Press key to quit");
            Console.ReadKey();
            Console.Write("\n\n");
        }
    }
}

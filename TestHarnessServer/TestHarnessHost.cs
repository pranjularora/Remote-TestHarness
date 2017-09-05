///////////////////////////////////////////////////////////////////////////////////
// TestHarnessHost.cs - Test Harness Server Functionality                        //
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
 * ==> accepts test request from multiple clients
 * ==> and execute them using Tasks
 * 
 * Function Operations:
 * ===================
 * public void initialize()
 * initializes the object, the objects initialized in this can not be initialized in the constructor as they raise conflict for communication channel objects.
 * 
 * 
 * public TestHarnessHost() 
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
 * After message gets dequeued, this function gets the test request
 * runs for multiple clients concurrently
 * check the type of message and perform the required functionality as per the type of the message.
 * 
 * public Messages makeMessage(string author, string fromEndPoint, string toEndPoint)
 * helps creating a message
 * 
 * public int Deserialize_msg(Messages msg)
 * accepts the message
 * parse it
 * request files from the repository
 * 
 * public void receiveFiles()
 * open a channel of file streaming
 * 
 * public void fileReceived(Messages msg)
 * Helper function of thread processing, to send files 
 * 
 * public void fileNotReceived(Messages msg)
 * Helper function of thread processing, to send Notification 
 * 
 * Main() function:
 * starting point of the test harness server
 * 
 * Public Classes:
 * ===================
 * -- class TestHarnessHost : IStreamService
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
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ClientCommunication;
using MessageCreate;
using AppDomainManager;
using System.IO;
using TestHarnessFileManager;
using UtilitiesExtension;

namespace TestHarnessServer
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class TestHarnessHost : IStreamService
    {
        private Comm<TestHarnessHost> comm { get; set; }

        private XMLParser.XMLParser xmlParser;

        private string endPoint;
        private string RepositoryEndPoint;
        private Manager manager = new Manager();
        private Thread rcvThread = null;

        private static bool notification_Received = false;
        object locker = new object();

      //  private static List<string> TempDir = null;

        //----------------------------------------------- For creating file stream receive end point
        private string filename;
        
        private static string savePath = @"..\..\..\..\RepositorySavedFiles"; 
        private static Queue<KeyValuePair<string, string>> testRequest_Queue;
        private int BlockSize = 1024;
        private byte[] block;
        HRTimer.HiResTimer hrt = null;

        //-----------------------------------------------  For creating file stream receive end point END


        public void initialize()
        {
            comm = new Comm<TestHarnessHost>();
            endPoint = Comm<TestHarnessHost>.makeEndPoint("http://localhost", 8080); // 8080 is test harness end point
            RepositoryEndPoint = Comm<TestHarnessHost>.makeEndPoint("http://localhost", 8082);
            comm.rcvr.CreateRecvChannel(endPoint);
            rcvThread = comm.rcvr.start(rcvThreadProc);
        }

        public TestHarnessHost()
        {
            //----------------------------------------------- For creating file stream receive end point
            block = new byte[BlockSize];
            hrt = new HRTimer.HiResTimer();
            //----------------------------------------------- For creating file stream receive end point END
        }

        //----------------------------------------------- For creating file stream receive end point
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
            Console.Write("\n  Received file \"{0}\" of {1} bytes in {2} microsec.", filename, totalBytes, hrt.ElapsedMicroseconds);
        }


        static ServiceHost ReceiverCreateServiceChannel(string url)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.TransferMode = TransferMode.Streamed;
            binding.MaxReceivedMessageSize = 500000000;
            Uri baseAddress = new Uri(url);
            Type service = typeof(TestHarnessHost);
            ServiceHost host = new ServiceHost(service, baseAddress);
            host.AddServiceEndpoint(typeof(IStreamService), binding, baseAddress);
            return host;
        }


        //----------------------------------------------- For creating file stream receive end point END

        public void wait()
        {
            rcvThread.Join();
        }

        void rcvThreadProc()
        {
            while (true)
            {
                Messages msg = comm.rcvr.GetMessage(); // receives message
                msg.time = DateTime.Now;
                Console.Write("\n  {0} received message:", comm.name);
                if (msg.command == "TestRequest") // processes test requests
                {
                    " [[ Displaying Requirement 2 ---->>> Test Harness Accepting Test Request in form of Message  ]]".title('=');
                    msg.show();
                    Console.WriteLine ("----------------------------------------------------------------------------------");
                    Task t1 = Task.Run(() => // locking used so that no two threads access each other variables values 
                    {// creates a task after receiving a message and will be executed concurrently 
                        lock (locker)
                        {
                            int received_Dll_Count = Deserialize_msg(msg);
                            int dll_Count;
                            while (true)
                            {
                                if (notification_Received) // waits for notification to arrive from the repository
                                    break;
                            }
                            notification_Received = false;
                            dll_Count = Directory.GetFiles(savePath).Length;

                            if (dll_Count != received_Dll_Count)
                            { // if all files are not received from the repository
                                    filesNotReceived(msg);
                            }
                            else  // if all files received from the repository
                                    fileReceived(msg);
                        }
                    });
                }
                else if (msg.command == "Notification") // notification from the repository
                    notification_Received = true;
            }
        }


        public void filesNotReceived(Messages msg)
        {
            Messages TestHarnessMsg = makeMessage(msg.author, msg.toUrl, msg.fromUrl); // from and to URL gets reversed as the message needs to
            TestHarnessMsg.message_content = "All Requested DLLs are not in the Repository";
            TestHarnessMsg.command = "Error";
            (" [[ Displaying Requirement 3 ---->>> " + TestHarnessMsg.message_content + "   ]]").title('=');
            Console.WriteLine("------- Sending Notification to the Client: {0} -------", msg.fromUrl);
            comm.sndr.PostMessage(TestHarnessMsg);
        }

        public void fileReceived(Messages msg)
        {
            Console.WriteLine("\n =============================== All requested DLLs Received received\n");
            Messages TestHarnessMsg;
            List<string> logs_and_Path = manager.manager(testRequest_Queue, savePath);
            string Logs_path = logs_and_Path[0];
            string logs = null;
            for (int i = 1; i < logs_and_Path.Count; i++)
            {
                logs += logs_and_Path[i];
                logs += ";";
            }
            TestHarnessMsg = makeMessage(msg.author, msg.toUrl, msg.fromUrl); // from and to URL gets reversed as the message needs to
            TestHarnessMsg.message_content = logs;
            TestHarnessMsg.command = "TestResult";
            comm.sndr.PostMessage(TestHarnessMsg);

            TestHarnessManager fileManager = new TestHarnessManager();
            (" [[ Displaying Requirement 7 ---->>> Sending Logs to the Repository ]]").title('=');
            fileManager.assignChannel("http://localhost:8001/StreamService");
            fileManager.callUploader(Logs_path);
        }

        // creates a message format
        public Messages makeMessage(string author, string fromEndPoint, string toEndPoint)
        {
            Messages message = new Messages();
            message.author = author;
            message.fromUrl = fromEndPoint;
            message.toUrl = toEndPoint;
            return message;
        }
        
        public int Deserialize_msg(Messages msg)
        {
            (" [[ Displaying Requirement 4 ---->>> Processing Dequeued Request Concurrently ]]").title('=');
            Messages receivedTestrequest;
            List<string> dllList;
            string time = null;
            string dirPath = null;
            lock (locker)
            {
                xmlParser = new XMLParser.XMLParser(); 
                receivedTestrequest = new Messages();
                receivedTestrequest = msg;
                dllList = new List<string>();
                time = msg.time.ToString("MM-dd-yyyy") + " " + msg.time.Hour + "-" + msg.time.Minute + "-" + msg.time.Second + " " + msg.time.ToString("tt");
                dirPath = @"..\..\..\..\" + msg.author + "-" + time;
            }
            Console.WriteLine("creating director with name: {0}", dirPath); // creates a temporary directory
            Directory.CreateDirectory(dirPath);
           // TempDir.Add(dirPath);
            Queue<KeyValuePair<string, string>> xml_Info_queue;
            int nu, count;
            lock (locker)
            {
                xml_Info_queue = new Queue<KeyValuePair<string, string>>();
                xml_Info_queue = xmlParser.Parse(msg.message_content);// call parser
                testRequest_Queue = new Queue<KeyValuePair<string, string>>();
                savePath = Path.GetFullPath(dirPath);  // this is the path of the temporary directory where the repo will upload files
                Console.WriteLine("Parsed in App domain manager");
                nu = xml_Info_queue.Count;
            }
            for (int i = 0; i < nu; i++)
            {
                var de = xml_Info_queue.Dequeue();
                testRequest_Queue.Enqueue(new KeyValuePair<string, string>(de.Key, de.Value));
                Console.WriteLine("key is: {0}  and val is: {1}", de.Key, de.Value);
                if (de.Key != "author")
                    dllList.Add(de.Value); // makes a dll list to be retrived for testing from the repository
            }
            lock (locker)
            {           // now the manager will ask the repository to send these files in this local directory..
                count = dllList.Count;
                Messages message_for_Repo = makeMessage("Repository", endPoint, RepositoryEndPoint);
                message_for_Repo.command = "DLL_Request";
                message_for_Repo.message_content = dllList.ToXml();
                comm.sndr.PostMessage(message_for_Repo);
            }
            Console.WriteLine("waiting for repository to upload files: \n");
            lock (locker)
                receiveFiles(); // opens host for receiving files
            return count;
        }

        public void receiveFiles()
        {
            ServiceHost host = ReceiverCreateServiceChannel("http://localhost:8000/StreamService");

            host.Open();
            Console.Write("\n  SelfHosted File Stream Service started");
            Console.Write("\n ========================================\n");
            while (true)
            {
                if (notification_Received) // waits for notification to arrive from the repository
                    break;
            }
            Console.Write("\n");
            host.Close();            
        }

        static void Main(string[] args)
        {
            Console.Write("\n  Test HARNESS SERVER");
            Console.Write("\n =====================\n");

            TestHarnessHost Server = new TestHarnessHost();
            Server.initialize();

            
            Console.ReadKey();


            Console.Write("\n\n");
        }
    }
}

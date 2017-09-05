///////////////////////////////////////////////////////////////////////////////////
// TestHarnessManager.cs - Test Harness File Manager                             //
// ver 1.0                                                                       //
// Application: CSE681 - Software Modelling and Analysis, Project #4             //
// Author:      Pranjul Arora, Syracuse University,                              //
//              parora01@syr.edu, (315) 949-9061                                 //
///////////////////////////////////////////////////////////////////////////////////

/*
 * Module Operations:
 * ==================
 * The Task of this module is to upload Files to the  client or repository
 * 
 * Function Operations:
 * ===================
 * public void assignChannel(string url)
 * creates a channel for sending files
 * 
 * 
 * public void uploadFile(string filename, string toSend)
 * uploads files
 * 
 * 
 * void callUploader(string toSend)
 * calls uploader function and send files one by one
 * 
 * Main() function:
 * Acts as a Test Stub.
 * 
 * Public Classes:
 * ===================
 * -- TestHarnessManager --> defines  all above stated function
 * 
 *  *  Maintanence History:
 * --------------------
 * ver 1.0 : 23 Nov 2016
 * Project #4 Test Harness file manager 
 * 
 */



using ClientCommunication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UtilitiesExtension;

namespace TestHarnessFileManager
{
    public class TestHarnessManager
    {
        IStreamService channel;

        int BlockSize = 1024;
        byte[] block;
        HRTimer.HiResTimer hrt = null;

        public TestHarnessManager()
        {
            block = new byte[BlockSize];
            hrt = new HRTimer.HiResTimer();
        }
        public void assignChannel(string url)
        {
            channel = FileTransfer.SenderCreateServiceChannel(url); // opens a channel for sending files
        }
        public void uploadFile(string filename, string toSend) // calluploader function calls it with name of the files and upload one by onee
        {
            int numRetries = 10;
            int waitMilliSec = 50;

            string fqname = Path.Combine(toSend, filename);
            string x = Path.GetFullPath(fqname);
            for (int i = 0; i < numRetries; i++) // retry 10 times incase of a failure
            {
                try
                {
                    hrt.Start();
                    using (var inputStream = new FileStream(fqname, FileMode.Open))
                    {

                        FileTransferMessage msg = new FileTransferMessage();
                        msg.fileName = filename;
                        msg.transferStream = inputStream;
                        channel.upLoadFile(msg);
                    }
                    hrt.Stop();
                    Console.Write("\n  Uploaded file \"{0}\" in {1} microsec.", filename, hrt.ElapsedMicroseconds);
                    break;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(waitMilliSec);
                    Console.Write("\n  Error: \"{0}\"", ex.Message);
                    Console.WriteLine("\nRetry attempt: {0}", i + 1);
                }
            }
        }

        // call the uploader function to upload files
        public void callUploader(string toSend)
        {
            HRTimer.HiResTimer hrtLocal = new HRTimer.HiResTimer();
            hrtLocal.Start();
            foreach (string files in Directory.GetFiles(toSend))
            {
                uploadFile(Path.GetFileName(files), toSend);
            }

            hrtLocal.Stop();
            Console.Write("\n\n  total elapsed time for uploading = {0} microsec.\n", hrtLocal.ElapsedMicroseconds);
            Console.Write("\n\n");
        }

        // test stub
        static void Main(string[] args)
        {
            " [[ Test Stub -- Test Harness Manager ]]".title('=');
            TestHarnessManager fileManager = new TestHarnessManager();
            string Logs_path = @"..\..\..\..\Stub Testing\Logs";
            fileManager.assignChannel("http://localhost:8001/StreamService");
            Console.WriteLine("\n Opened channel to stream File, but as it is test stub running independently there would not be any receiver");
            fileManager.callUploader(Logs_path);
            Console.ReadKey();
        }
    }
}

///////////////////////////////////////////////////////////////////////////////////
// ClientFileManager.cs - Test Harness File Manager                              //
// ver 1.0                                                                       //
// Application: CSE681 - Software Modelling and Analysis, Project #4             //
// Author:      Pranjul Arora, Syracuse University,                              //
//              parora01@syr.edu, (315) 949-9061                                 //
///////////////////////////////////////////////////////////////////////////////////

/*
 * Module Operations:
 * ==================
 * The Task of this module is to upload Files to the Test harness server or repository
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
 * -- ClientFileManager --> defines all above stated function
 * 
 *   Maintanence History:
 * --------------------
 * ver 1.0 : 23 Nov 2016
 * Project #4 client file manager 
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

namespace Client_File_Manager
{
    public class ClientFileManager
    {
        IStreamService channel;

        string sourceDLL = @"..\..\..\..\Client-Files\Client\DLL"; // all DLL files will be in this directory
        int BlockSize = 1024;
        byte[] block;
        HRTimer.HiResTimer hrt = null;

        public ClientFileManager()
        {
            block = new byte[BlockSize];
            hrt = new HRTimer.HiResTimer();
        }

        public void assignChannel(string url)
        {
            channel = FileTransfer.SenderCreateServiceChannel(url); // opens a channel for sending files
        }
        public void uploadFile(string filename) // calluploader function calls it with name of the files and upload one by onee
        {
            int numRetries = 10;
            int waitMilliSec = 50;

            string fqname = Path.Combine(sourceDLL, filename);
            string x = Path.GetFullPath(fqname);
            for (int i = 0; i < numRetries; i++)  // retry 10 times incase of a failure
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
                    // Sending files to repository using files Streaming..
                    Thread.Sleep(waitMilliSec);
                    Console.WriteLine("\n  ---------------------Unable to Send Files to the Repository--------------------- \n");
                    Console.Write("\n  Error: \"{0}\"", ex.Message);
                    Console.WriteLine("\nRetry attempt: {0} -->> To Send {1}", i + 1, Path.GetFileName(fqname));
                }
            }
        }
        // call the uploader function to upload files
        public void callUploader(List<string> files)
        {
            HRTimer.HiResTimer hrtLocal = new HRTimer.HiResTimer();
            hrtLocal.Start();
            foreach (var file in files)
            {
                uploadFile(file);
            }
            hrtLocal.Stop();
            Console.Write("\n\n  total elapsed time for uploading = {0} microsec.\n", hrtLocal.ElapsedMicroseconds);
            Console.Write("\n\n");
        }
        // this will be called by the client and send its name based on which directory will be searched for the DLLs
        public void client_Manager(string dir)
        {
            " [[ Displaying Requirement 2 and 6 (Test Libraries)=======>>>>>> Sending Required DLL's to Repository  ]]".title('=');
            assignChannel("http://localhost:8001/StreamService");
            List<String> files = new List<string>();
            sourceDLL = Path.Combine(sourceDLL, dir);
            if (Directory.Exists(sourceDLL))
            {
                foreach (string file in Directory.GetFiles(sourceDLL, "*.dll"))
                {
                    files.Add(Path.GetFileName(file));
                }
                (" [[ Displaying Requirement 6 ---->>> Sending Files using File Streaming  ]]").title('=');
                callUploader(files);
            }
        }

        // Test Stub
        static void Main(string[] args)
        {
            "[[ Test Stub ]]".title('=');
            ClientFileManager manager = new ClientFileManager();
            manager.client_Manager("client");
            Console.WriteLine("opened channel to stream File, but as it is test stub running independently there would not be any receiver");
            Console.ReadKey();

        }
    }
}

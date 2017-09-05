///////////////////////////////////////////////////////////////////////////////////
// RepositoryFileManager.cs - Test Harness File Manager                          //
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
 * string Repo_Manager(HashSet<string> filesList)
 * this is called by the repository server and perform the functionality stated in this.
 * if all files found, send using file streaming else, send a notification
 * 
 * Main() function:
 * Acts as a Test Stub.
 * 
 * Public Classes:
 * ===================
 * -- repoFileManager --> defines  all above stated function
 * 
 *  *  Maintanence History:
 * --------------------
 * ver 1.0 : 23 Nov 2016
 * Project #4 repository file manager 
 * 
 */



using ClientCommunication;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UtilitiesExtension;

namespace RepoFileManager
{
    public class repoFileManager
    {
        IStreamService channel;
        string Repository_folder = @"..\..\..\..\Repository-Files\Repository"; // source path

        string ToSendPath = null;
        int BlockSize = 1024;
        byte[] block;
        HRTimer.HiResTimer hrt = null;

        public repoFileManager()
        {
            block = new byte[BlockSize];
            hrt = new HRTimer.HiResTimer();
        }

        public void assignChannel(string url)
        {
            channel = FileTransfer.SenderCreateServiceChannel(url);  // opens a channel for sending files
        }
        public void uploadFile(string filename, string toSend)  // calluploader function calls it with name of the files and upload one by one
        {
            int numRetries = 10;
            int waitMilliSec = 50;
            string fqname = Path.Combine(toSend, filename);
            string x = Path.GetFullPath(fqname);
            for (int i = 0; i < numRetries; i++)
            {
                try
                {
                    hrt.Start();
                    using (var inputStream = new FileStream(fqname, FileMode.OpenOrCreate))
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
      
        public void callUploader(HashSet<string> files, string toSend)
        {
            HRTimer.HiResTimer hrtLocal = new HRTimer.HiResTimer();
            hrtLocal.Start();
            foreach (var file in files)
            {
                uploadFile(file, toSend);
            }
            hrtLocal.Stop();
            Console.Write("\n\n  total elapsed time for uploading = {0} microsec.\n", hrtLocal.ElapsedMicroseconds);
            Console.Write("\n\n");
        }
        // this will be called by the repository server
        public string Repo_Manager(HashSet<string> filesList)
        {
            if (!Directory.Exists(Repository_folder))
                Directory.CreateDirectory(Repository_folder);
            string source = Path.Combine(Repository_folder, "DLL");
            string local_target = Path.Combine(Repository_folder, "temp");
            if (Directory.Exists(local_target))
            {
                Directory.Delete(local_target, true);
            }
            Directory.CreateDirectory(local_target); // creates a temop directory that send all files
            int flag = 0;

            foreach (var x in filesList)
            {
                flag = 0;
                foreach (string s in Directory.GetFiles(source))
                {
                    if (Path.GetFileName(s) == x)
                    {
                        File.Copy(s, Path.Combine(local_target, x), true);
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    Console.WriteLine("\n******************* REPOSITORY DOES NOT CONTAINS ALL THE REQUIRED FILES ******************\n");
                    Directory.Delete(local_target, true);
                    break;
                }
            }
            if (flag == 1)
            {
                ToSendPath = local_target;
                Console.WriteLine("\n *******************  ALL FILES Found ****************** \n  ");
            }
            return ToSendPath;
        }



        static void Main(string[] args)
        {
            // dll list
           "Test Stub --> Repository File Manager".title();
            List<string> dllList = new List<string>();
            dllList.Add("TestDriver2.dll");
            dllList.Add("CodeToTest2.dll");
            repoFileManager manager = new repoFileManager();
            manager.Repository_folder = @"..\..\..\..\Stub Testing";
            string message_content = dllList.ToXml();
            HashSet<string> filesList = message_content.FromXml<HashSet<string>>();


            string toSendFilesPath = manager.Repo_Manager(filesList);
            if (toSendFilesPath == null)
            {
                // if all files are not found, a null will be returned  
                Console.WriteLine("All requested DLLs are Not found ");
            }
            else
            {
                Console.WriteLine("found and are at {0} ", toSendFilesPath);
                manager.assignChannel("http://localhost:8000/StreamService");
                Console.WriteLine("\n Opened channel to stream File, but as it is test stub running independently there would not be any receiver");
                manager.callUploader(filesList, toSendFilesPath);
            }
            Console.ReadKey();

        }
    }
}

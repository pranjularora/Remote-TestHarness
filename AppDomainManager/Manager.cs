///////////////////////////////////\\\\\////////////////////////////////////////////////////////////////
// ApplicationDomainManager.cs - App Domain Manager of Test Harness Server                            //
// ver 2.0                                                                                            //
// Application: CSE681 - Software Modelling and Analysis, Project #4                                  //
// Author:      Pranjul Arora, Syracuse University,                                                   //
//              parora01@syr.edu, (315) 949-9061                                                      //
////////////////////////////////////////////////////////////////////////////////////////////////////////


/*
 * Module Operations:
 * ==================
 * This Module is the Application Domain Manager. 
 * It has following Responsibilities:
 * ==> Created by test Harness server after it receives all files from the repository.
 * ==> Create Child Domain.
 * 
 * 
 * 
 * Function Operations:
 * ===================
 * List<string> childDomain(string name, Queue<KeyValuePair<string, string>> xmlData, string pathDLL)
 * This function creates a child Application Domain
 * And then the Application Domain Manager will Load the Loader into it.
 * Accepts child Domain Name and assing it to the child.
 * Accepts XML Data (Queue) and pass them to the loader.
 * Unload Child App Domain.
 * returns a list of strings that has logs and Path of Logs to the manager.
 * 
 * 
 * List<string> manager(Queue<KeyValuePair<string, string>> TestRequest, string path_Dll)
 * This function accepts the queue containing the Test Requests from the Client and Path of the Temporary Folder
 * Dequeue them and create child Domain.
 * returns a list of strings that has logs and Path of Logs to the child thread. 
 * 
 *              
 * 
 * Public Classes:
 * ===================
 * -- class CreateChild
 * --> defines childDomain() function
 * 
 * -- class AppDomainManager
 * --> defines Manager() function
 * 
 * 
 * Maintanence History:
 * --------------------
 * ver 2.0 : 23 Nov 2016
 * Project #4 Now function returns logs results and Path of logs
 * ver 1.0 : 5 Oct 2016
 *  Project #2
 */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MessageCreate;
using System.IO;
using UtilitiesExtension;
using ClientCommunication;
using XMLParser;
using System.Security.Policy;
using System.Reflection;




namespace AppDomainManager
{
    class CreateChild
    {
        public List<string> childDomain(string name, Queue<KeyValuePair<string, string>> xmlData, string pathDLL)
        {
            List<string> logs_and_Path = null;
            try
            {
                AppDomainSetup domainInfo = new AppDomainSetup();
                domainInfo.ApplicationBase = System.Environment.CurrentDirectory;
                Evidence evid = AppDomain.CurrentDomain.Evidence;

                Console.WriteLine("\n\nCreating Child Domain For Handling Test Request ===========>>>>> [[ Requirement 5 ]] ");
                // creating a Child App Domain
                AppDomain ad1 = AppDomain.CreateDomain(name, evid, domainInfo);

                ad1.SetData("file_Data", xmlData); // Passing XML data to loader (set Function)
                
                Assembly assemLoadLib = ad1.Load("Loader"); // Loading Loader.cs 
                object obj1 = ad1.CreateInstanceAndUnwrap("Loader", "Loader.Loader"); // Unwrapping and calling the Loader.cs constructor
                
                Type t = assemLoadLib.GetType("Loader.Loader");

                var hold = t.GetMethod("LoadAndExecuteControlHead");
                logs_and_Path = hold.Invoke(obj1, new object[] { pathDLL }) as List<string>;
                (" [[ Displaying Requirement 7 ---->>> Unloading Child App Domain   ]]").title('=');
                AppDomain.Unload(ad1); // Unloading Child App Domain
                Console.WriteLine("\nDeleting temporary Directory--> {0}", pathDLL);
            }
            catch (Exception e)
            {
                Console.WriteLine("\n Child Application domain throws an error with Message:\n ===========> {0}", e.Message);
            }
            return logs_and_Path;
        }
    }

    public class Manager
    {
        // this test request contains message body 
        public List<string> manager(Queue<KeyValuePair<string, string>> TestRequest, string path_Dll)
        {
            AppDomain main = AppDomain.CurrentDomain;
            ("[[ \"" + main.FriendlyName + "\" ]] ====>>>> APPLICATION DOMAIN created").title('=');

            (" [[ Displaying Requirement 4 ---->>> Application Domain Created ]]").title('=');

            CreateChild c1 = new CreateChild(); // creating child for every Test Request.
            string childName = "child ";
            HRTimer.HiResTimer testTimer = new HRTimer.HiResTimer();
            testTimer.Start();
            List<string> logs_and_Path = c1.childDomain(childName, TestRequest, path_Dll);
            testTimer.Stop();
            (" [[ Displaying Requirement 12 ---->>> Test Execution Time " + testTimer.ElapsedMicroseconds + "  ]]").title('=');
            return logs_and_Path;

        }
        // Stub
        static void Main(string[] args)
        {
            // making a dummy queue and refering to test stub path 
            Console.WriteLine("\n Application Domain Manager Stub: \n");
            Queue<KeyValuePair<string, string>> testRequest_Queue = new Queue<KeyValuePair<string, string>>();
            testRequest_Queue.Enqueue(new KeyValuePair<string, string>("author", "Pranjul"));
            testRequest_Queue.Enqueue(new KeyValuePair<string, string>("test1", "TestDriver2.dll"));
            testRequest_Queue.Enqueue(new KeyValuePair<string, string>("test1", "CodeToTest2.dll"));

            string path = @"..\..\..\..\Stub Testing";
            Manager manager = new Manager();
            List<string> logs_and_Path;
            logs_and_Path = manager.manager(testRequest_Queue, path);
            Console.WriteLine("Displaying logs information and path returned after processing");
            for (int i = 0; i< logs_and_Path.Count; i++)
            {
                Console.WriteLine("{0}\n", logs_and_Path[i]);
            }

            Console.ReadKey();


        }
    }
}

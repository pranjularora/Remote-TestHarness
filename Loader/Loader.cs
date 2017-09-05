using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Executor;
using UtilitiesExtension;
using Logger;

namespace Loader
{
    public class Loader : MarshalByRefObject
    {
        public Loader()
        {
            ("Loading Child Application Domain: " + AppDomain.CurrentDomain.FriendlyName).title('=');
        }

        public List<string> LoadAndExecuteControlHead(string pathDLL)
        {
            Queue<KeyValuePair<String, String>> testInfo = new Queue<KeyValuePair<string, string>>();
            List<Dictionary<string, string>> list_of_Dict_of_Logs = new List<Dictionary<string, string>>();
            List<string> logs_and_Path = new List<string>();

            testInfo = AppDomain.CurrentDomain.GetData("file_Data") as Queue<KeyValuePair<String, String>>;
            executor execute = new executor();
            Console.WriteLine("\n>> Looking up Directory: {0}, To Load TestDrivers.\n", pathDLL);
            " [[ Displaying Requirement 5 (can be verified from executor.cs -- lines 72-100 )]]".title('='); // Using the utilities.cs to print
            if (execute.LoadingTests(pathDLL, testInfo)) // Loading Tests by call Executor
            {
                list_of_Dict_of_Logs = execute.run(testInfo, pathDLL); // Running Test Drivers
            }
            else
                Console.WriteLine("\n==================>>>>>   couldn't load tests");


            // after everything is done, the loader will ask the logger to give back the location where logs are created..
            Logger.Logger logObj = new Logger.Logger();

            string logsPath = logObj.sendLogsDirectoryToLoader();
            logs_and_Path.Add(logsPath);
            for (int i = 0; i < list_of_Dict_of_Logs.Count; i++)
            {
                var dictToString = string.Join("; ", list_of_Dict_of_Logs[i].Select(p => string.Format("{0}, {1}", p.Key, p.Value)));
                logs_and_Path.Add(dictToString);
            }
            return logs_and_Path;
        }

        static void Main(string[] args)
        {
            // Test Stub
            // Loading Temp Data to check stubs functionality
            "LOADER STUB ====>>>>>>>>> ".title('=');

            Console.WriteLine("\n---------------------------------------------------------------------------------------------");
            Console.WriteLine(" The Main App Domain i.e. Loader, is the Only Domain for this. \n Considering it as a Child for demonstrating STUBS's Funcationality");
            Console.WriteLine("---------------------------------------------------------------------------------------------\n");

            string path = @"..\..\..\..\Stub Testing";
            Queue<KeyValuePair<string, string>> testRequest_Queue = new Queue<KeyValuePair<string, string>>();
            testRequest_Queue.Enqueue(new KeyValuePair<string, string>("author", "Pranjul"));
            testRequest_Queue.Enqueue(new KeyValuePair<string, string>("test1", "TestDriver2.dll"));
            testRequest_Queue.Enqueue(new KeyValuePair<string, string>("test1", "CodeToTest2.dll"));

            AppDomain.CurrentDomain.SetData("file_Data", testRequest_Queue);
            Loader loadStub = new Loader();
            loadStub.LoadAndExecuteControlHead(path);

            Console.ReadKey();
        }
    }
}

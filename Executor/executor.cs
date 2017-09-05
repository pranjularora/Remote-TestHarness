//////////////////////////////////////////////////////////////////////////////////
// Executor.cs - Test Harness File Manager                                      //
// ver 2.0                                                                      //
// Application: CSE681 - Software Modelling and Analysis, Project #4            //
// Author:      Pranjul Arora, Syracuse University,                             //
//              parora01@syr.edu, (315) 949-9061                                //
//////////////////////////////////////////////////////////////////////////////////

/*
 * Module Operations:
 * ==================
 * The Task of this module is to Load all the Test Drivers and Execute them.
 * 
 * Function Operations:
 * ===================
 * 
 * -------------------------------------------------------------------------------------------------------------------------
 * bool LoadingTests (string path, List<XMLInfo> xmlInfo, string repository_path):
 * 
 * This function is responsible for Loading all the test Drivers present in the Temporary Directory 
 * created by the Child App Domain.
 * -> checks all the Libraries in the Temporary folder and loads only the Test Drivers.
 * -> Returns true to the loader if the number of test drivers are greater than zero.
 * -> call the createDictionaryOfLogs function and retrieves Dictionary.
 * 
 * Additional Information about a functionality in bool LoadingTests function
 * ******* #1 Description *******
 * ==> if (types.Count() == 0)  
 * This is used in the function for the case when the Test Driver class is not public. 
 * GetExportedTypes(), wont be able to get Anything and count of "types" would be 0.
 * That means the Test Case(Test Driver which it is trying to load) cannot be Executed.
 * It only stalls that Test Case not the whole test request.
 *     
 * ==> throw (new Exception("Unable to get Public Members for the Test Driver"));
 * This exception will be thrown for Test Case 1. As, its class is intentionally made private.
 * This exception would not be thrown for CodeToTest, as its class can not be made private (Project would not compile).
 * --> Also,tThis exception will be thrown for any case in which test Driver is not loaded.
 * -------------------------------------------------------------------------------------------------------------------------
 * 
 * 
 * public void run(List<XMLInfo> xmlInfo, string repository_path)
 * This module maintains the list of the test drivers in a structure. 
 * run() function will execute all those tests.
 * Calls the createDictionaryOfLogs() function and retrieves Dictionary.
 * Creates a Logger class object.
 * Call its function by passing this dictionary to display Logs and stores them.
 * 
 * 
 * private Dictionary<string, string> createDictionaryOfLogs(XMLParser.XMLInfo x)
 * This function will assist the LoadingTests() and run() functions by creating a dictionary and returning it to them.
 * 
 * 
 * 
 * Main() function:
 * Acts as a Test Stub.
 * 
 * Public Classes:
 * ===================
 * -- executor --> defines all the above stated functions
 * 
 * 
 *   Maintanence History:
 * --------------------
 * ver 2.0 : 23 Nov 2016
 * Project #4 No changes in this module
 * ver 1.0 : 5 Oct 2016
 * Project #2
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Logger;

using TestInterfaces;
using UtilitiesExtension;


namespace Executor
{
    public class executor
    {
        private ILog logClassObject;

        private Dictionary<string, string> TestcaseInfo = null;

        private static List<Dictionary<string, string>> allLogs = new List<Dictionary<string, string>>();

        private struct TestData
        {
            public string Name;
            public ITest testDriver;
        }


        private List<TestData> testDriver = new List<TestData>();

        // A function that creates a local directory and return it to the LoadingTests() and run() functions
        private Dictionary<string, string> createDictionaryOfLogs(Queue<KeyValuePair<string, string>> x, string testCaseName)
        {
            Dictionary<string, string> localDict = new Dictionary<string, string>();
            string author = null;
            string testDriver = null;
            string codeToTest = null;
            foreach (KeyValuePair<string, string> val in x)
            {
                if (val.Key == "author")
                {
                    author = val.Value;
                }
                if (val.Key == testCaseName)
                {
                    if (val.Value.Contains("CodeTo"))
                    {
                        codeToTest = val.Value;
                    }
                    else if (val.Value.Contains("TestDriver"))
                    {
                        testDriver = val.Value;
                    }
                }
            }
            string fileName = testCaseName + "- " + author + " " + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + " " + DateTime.Now.ToString("tt");
            string timeStamp = DateTime.Now.ToString("MM-dd-yyyy") + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + " " + DateTime.Now.ToString("tt");
            localDict.Add("Test Name", testCaseName);
            localDict.Add("fileName", fileName);
            localDict.Add("Author", author);
            localDict.Add("TimeStamp", timeStamp);
            localDict.Add("Test Driver", testDriver);
            localDict.Add("Code To Test", codeToTest);

            return localDict;
        }

        // This function is loading all the Test Drivers found in the Directory
        // For every Important step a print statement is also included.
        public bool LoadingTests(string path, Queue<KeyValuePair<string, string>> xmlInfo)
        {
            string[] files = Directory.GetFiles(path, "*.dll"); // Getting all DLL's from the path 
            Assembly assem1;
            Console.WriteLine("Checking Temporary Created Folder: \"{0}\" for Loading Libraries", path);
            foreach (string file in files)
            {
                try
                {
                    assem1 = Assembly.LoadFrom(file);
                    Type[] types = assem1.GetExportedTypes();
                    if (types.Count() == 0) // Detailed comment for this has been added in the prologue
                        throw (new Exception("Unable to get Public Members for the Test Driver"));
                    foreach (Type t in types)
                    {
                        if (t.IsClass && typeof(ITest).IsAssignableFrom(t)) // check if a test driver is derived from ITest
                        {
                            ITest tdr = (ITest)Activator.CreateInstance(t);    // create instance of test driver
                            TestData td = new TestData();
                            td.Name = t.Name;
                            td.testDriver = tdr;
                            testDriver.Add(td);
                            Console.WriteLine("\n==============>>>>>> Test Driver: {0} Found, Loading It -------------------->>> [[ Part of ITest Interface ]] \n", td.Name);
                        }
                        else
                            Console.WriteLine("\n==============>>>>>> {0} is not a Test Driver. So will Not be Loaded in Test Drivers List\n", Path.GetFileName(file));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n==========>>>>> Unable to Load File: \"{0}\"\n", Path.GetFileName(file));
                    logClassObject = new Logger.Logger();// logging information for Test Driver 1..
                    TestcaseInfo = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> x in xmlInfo)
                    {
                        if (Path.GetFileName(file) == x.Value)
                        {
                            ("********** Test Case: \"" + x.Key + "\", Not Executed").title('=');
                            TestcaseInfo = createDictionaryOfLogs(xmlInfo, x.Key);
                            TestcaseInfo.Add("Test Result", "Test Case Not Executed");
                            TestcaseInfo.Add("Exception Thrown", ex.Message);
                            break;
                        }
                    }
                    allLogs.Add(TestcaseInfo);
                    logClassObject.getLogs(TestcaseInfo); // calling Logger.cs getLogs function to display logs
                    logClassObject.createLogFile(TestcaseInfo, path); // calling logger to create log File
                }
            }
            return testDriver.Count > 0;
        }


        public List<Dictionary<string, string>> run(Queue<KeyValuePair<string, string>> xmlInfo, string path)
        {
            foreach (TestData td in testDriver)  // enumerate the test list
            {
                TestcaseInfo = new Dictionary<string, string>();
                logClassObject = new Logger.Logger();
                try
                {
                    // for each on queue object
                    foreach (KeyValuePair<string, string> x in xmlInfo)
                    {
                        if ((td.Name + ".dll") == x.Value)
                        {
                            ("********** Test Case: " + x.Key + ", GETTING EXECUTED").title('='); // calling Utilities.cs
                            TestcaseInfo = createDictionaryOfLogs(xmlInfo, x.Key); // calling createDictionaryOfLogs() function
                            break;
                        }
                    }
                    if (td.testDriver.test() == true)
                    {
                        TestcaseInfo.Add("Test Result", "Test Passed"); // Test Passed information added to the dictionary and sent to logger
                        Console.WriteLine("\n  TEST=========>>>> PASSED\n");
                    }
                    else
                    {
                        TestcaseInfo.Add("Test Result", "Test Failed"); // Test Failed
                        Console.WriteLine("\n  TEST=========>>>> FAILED\n");
                    }
                }
                catch (Exception ex)
                {
                    TestcaseInfo.Add("Test Result", "Test Failed");
                    // In case of exception is thrown, it will be caught here..
                    TestcaseInfo.Add("Exception Thrown", ex.Message);
                }
                // TestcaseInfo Dictionary will be passed to getLog function in the logger class..
                allLogs.Add(TestcaseInfo);
                logClassObject.getLogs(TestcaseInfo);
                logClassObject.createLogFile(TestcaseInfo, path);

            }
            return allLogs;

        }

        // test stub
        static void Main(string[] args)
        {
            // making a dummy queue and refering to test stub path
            "[[ Test Stub Executor ]]".title('=');
            Queue<KeyValuePair<string, string>> testRequest_Queue = new Queue<KeyValuePair<string, string>>();
            testRequest_Queue.Enqueue(new KeyValuePair<string, string>("author", "Pranjul"));
            testRequest_Queue.Enqueue(new KeyValuePair<string, string>("test1", "TestDriver2.dll"));
            testRequest_Queue.Enqueue(new KeyValuePair<string, string>("test1", "CodeToTest2.dll"));

            string path = @"..\..\..\..\Stub Testing";
            List<Dictionary<string, string>> list_of_Dict_of_Logs = new List<Dictionary<string, string>>();

            executor exe = new executor();
            if (exe.LoadingTests(path, testRequest_Queue)) // Loading Tests by call Executor
            {
                list_of_Dict_of_Logs = exe.run(testRequest_Queue, path); // Running Test Drivers
            }
            for (int i = 0; i < list_of_Dict_of_Logs.Count; i++)
            {
                // the dictionary needs to be converted to string so that it can be processed further..
                var dictToString = string.Join("; ", list_of_Dict_of_Logs[i].Select(p => string.Format("{0}, {1}", p.Key, p.Value)));
                // displaying the dictionary converted to string
                Console.WriteLine(dictToString);

            }
            Console.ReadKey();



        }
    }
}

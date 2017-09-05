///////////////////////////////////////////////////////////////////////////////////
// Logger.cs - Display Logs and Store                                            //
// ver 2.0                                                                       //
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
 * ==> Display Logs of all the Test Cases.
 * ==> Stores them into Logs folder in the Repository
 * 
 * 
 * Function Operations:
 * ===================
 * getLogs(Dictionary<string, string> TestcaseInfo)
 * -- Receive a Dictionary having Log information 
 * -- Displays them on the console.
 * 
 * 
 * createLogFile(Dictionary<string, string> TestcaseInfo, string source_path)
 * -- Receive a Dictionary having Log information 
 * -- Creates a Logs Folder (with Author Name and Timestamp) in the repository, if it does exists.
 * -- creates a File with, "Test case Name + Author Name + Timestamp" and store them in the Logs Folder
 * 
 * Main() function:
 * Acts as a Test Stub.
 * 
 * Public Classes:
 * ===================
 * -- class Logger
 * --> defines all above stated functions 
 * 
 * Public Interface:
 * ===================
 *  ILog;
 *  -> void getLogs(Dictionary<string, string> TestcaseInfo)
 *  -> void createLogFile(Dictionary<string, string> TestcaseInfo, string source_path)
 *  
 *  Maintanence History:
 * --------------------
 * ver 2.0 : 23 Nov 2016
 * Project #4 No changes in this module
 * ver 1.0 : 5 Oct 2016
 *  Project #2
 *  
 */



using System;
using System.Collections.Generic;
using System.IO;
using TestInterfaces;

namespace Logger
{
    public class Logger : ILog
    {

        private static string logsPath;
        public void getLogs(Dictionary<string, string> TestcaseInfo)
        {
            // Displays all the logs for a Test Case.
            foreach (KeyValuePair<string, string> log_data in TestcaseInfo)
            {
                if (log_data.Key == "Author") // This is done for formatting
                    Console.WriteLine("{0}:\t\t    {1}", log_data.Key, log_data.Value);
                else
                    Console.WriteLine("{0}:\t    {1}", log_data.Key, log_data.Value);
            }
            Console.WriteLine("------------------------------------------------------------------------------------------\n\n");
        }

        public string sendLogsDirectoryToLoader()
        {
            return logsPath;
        }

        public void createLogFile(Dictionary<string, string> TestcaseInfo, string source_path)
        {
            string fileName = null;
            string author = null;
            string timeStamp = null;
            source_path = Path.Combine(source_path, "Logs");
            // check if Logs Directory exist,
            if (!Directory.Exists(source_path))
                Directory.CreateDirectory(source_path); // if not, create new

            foreach (KeyValuePair<string, string> log_Info in TestcaseInfo)
            {
                if (log_Info.Key == "fileName")
                {
                    fileName = log_Info.Value;
                    fileName += ".txt"; 
                }
                if (log_Info.Key == "Author")
                    author = log_Info.Value;

                if (log_Info.Key == "TimeStamp")
                    timeStamp = log_Info.Value;
            }
            // creating Log Directory Name and Path.
            string logs_Directory_name = author + " " + timeStamp;
            string logs_Directory_path = Path.Combine(source_path, logs_Directory_name);

            if (!Directory.Exists(logs_Directory_path))
            {
                Directory.CreateDirectory(logs_Directory_path);
                Console.WriteLine("=========================================>>>>>>> [[ Displaying REQUIREMENT 7 ]]\n");
                Console.WriteLine(">> Creating Directory with Author Name and Timestamp: \"{0}\"\n", logs_Directory_path);
            }

            // Writing Data to the Log File
            using (StreamWriter logFile = (File.CreateText(Path.Combine(logs_Directory_path, fileName))))
            {
                foreach (KeyValuePair<string, string> log_data in TestcaseInfo)
                {
                    if (log_data.Key == "Author")
                        logFile.WriteLine("{0}:\t\t          {1}", log_data.Key, log_data.Value);
                    else
                        logFile.WriteLine("{0}:\t          {1}", log_data.Key, log_data.Value);
                }
            }
            Console.WriteLine("Storing the Log Informtion by creating a Log file: \"{0}\"\nIn the Directory:  \"{1}\"  \n", fileName, logs_Directory_path);
            logsPath = logs_Directory_path;
        }



        static void Main(string[] args)
        {
            // Test Stub
            Dictionary<string, string> loggerTest = new Dictionary<string, string>();
            Logger log = new Logger();
            string timeStamp = DateTime.Now.ToString("MM-dd-yyyy") + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + " " + DateTime.Now.ToString("tt");
            string repository_Path = @"..\..\..\..\Stub Testing";
            string fileName = "Logger Test Stub";
            string author = "Logger Stub";
            loggerTest.Add("fileName", fileName);
            loggerTest.Add("Author", author);
            loggerTest.Add("TimeStamp", timeStamp);
            loggerTest.Add("Testing Logger4: ", "Test Case4");
            string source_path = Path.Combine(repository_Path, "Logs");
            string logs_Directory_name = author + " " + timeStamp;
            string logs_Directory_path = Path.Combine(source_path, logs_Directory_name);

            Console.WriteLine("\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\ \n\n");
            Console.WriteLine("\t\t\t\t  ------------------  Testing Logger STUB   ------------------ \n\n");
            Console.WriteLine("Calling GetLog Function and displaying the results by sending Dummy Logs\n\n");

            log.getLogs(loggerTest);
            Console.WriteLine("********************************************************************** \n\n");
            Console.WriteLine("Calling createLogFile Function---->>> \n");
            Console.WriteLine("Storing the Log Informtion by creating a Log file in the Directory: \"{0}\".. \nCheck Repository at Location: {1}  \n", logs_Directory_name, logs_Directory_path);
            log.createLogFile(loggerTest, repository_Path);
            Console.WriteLine("\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\ \n\n");
            Console.ReadKey();
        }
    }
}

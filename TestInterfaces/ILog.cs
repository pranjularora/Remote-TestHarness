////////////////////////////////////////////////////////////////////////////////////
// ILog.cs - Interface for Logger Class                                           //
// ver 2.0                                                                        //
// Application: CSE681 - Software Modelling and Analysis, Project #4              //
// Author:      Pranjul Arora, Syracuse University,                               //
//              parora01@syr.edu, (315) 949-9061                                  //
////////////////////////////////////////////////////////////////////////////////////

/*
 * Module Operations:
 * ==================
 * This Module is responsible for declaring the function which has to be implemented by all the classes 
 * implementing it.
 * 
 * Functions:
 * ===================
 * void getLogsData(Dictionary<string, string> TestcaseInfo);
 * void createLogFile(Dictionary<string, string> TestcaseInfo, string source_path);
 * These will be defined by Logger class
 * 
 *
 * Maintanence History:
 * --------------------
 * ver 2.0 : 23 Nov 2016
 * Project #4 No changes in this module
 * ver 1.0 : 5 Oct 2016
 *  Project #2
 *  
 */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestInterfaces
{
    // Interface for Logger Class
    public interface ILog
    {
        void getLogs(Dictionary<string, string> TestcaseInfo);
        void createLogFile(Dictionary<string, string> TestcaseInfo, string source_path);
    }
}

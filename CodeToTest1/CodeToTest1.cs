///////////////////////////////////////////////////////////////////////////////////
// CodeToTest1.cs - Code that Client needs to Tests                              //
// ver 2.0                                                                       //
// Application: CSE681 - Software Modelling and Analysis, Project #4             //
// Author:      Pranjul Arora, Syracuse University,                              //
//              parora01@syr.edu, (315) 949-9061                                 //
///////////////////////////////////////////////////////////////////////////////////

/*
 * Module Operations:
 * ==================
 * This Module represents the code which the client needs to test.
 * 
 * Function Operations:
 * ===================
 * int subtract(int n1, int n2)
 * This function will be called by the Test Driver and the functionality of this code will be checked by that driver.
 * 
 *
 * 
 * Main()
 * Acts as Test Stub
 * 
 * 
 * Public Classes:
 * ===================
 * class CodeToTest1
 * -- defines all the above stated function
 * 
 *  Maintanence History:
 * --------------------
 * ver 2.0 : 23 Nov 2016
 * Project #4 No changes in this module
 * ver 1.0 : 5 Oct 2016
 *  Project #2
 * 
 * 
 */




using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor
{
    public class CodeToTest1
    {
        public int subtract(int n1, int n2)
        {
            return (n1 - n2); // return subtraction results
        }
        static void Main(string[] args)
        {
            // Test Stub
            CodeToTest1 code = new CodeToTest1();
            int result = code.subtract(3, 2);
            Console.Write("Result is:{0}\n", result);
            Console.ReadKey();
        }
    }
}

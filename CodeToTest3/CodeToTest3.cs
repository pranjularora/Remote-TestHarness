//////////////////////////////////////////////////////////////////////////////////
// CodeToTest3.cs - Code that Client needs to Tests                             //
// ver 2.0                                                                      //
// Application: CSE681 - Software Modelling and Analysis, Project #4            //
// Author:      Pranjul Arora, Syracuse University,                             //
//              parora01@syr.edu, (315) 949-9061                                //
//////////////////////////////////////////////////////////////////////////////////

/*
* Module Operations:
 * ==================
 * This Module represents the code which the client needs to test.
 * 
 * Function Operations:
 * ===================
 * int sum(int n1, int n2)
 * This function will be called by the Test Driver and the functionality of this code is tested by that driver.
 * 
 *
 * 
 * Main()
 * Acts as Test Stub
 * 
 * 
 * Public Classes:
 * ===================
 * class CodeToTest3
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
using UtilitiesExtension;

namespace Executor
{
    public class CodeToTest3
    {

        public int sum(int n1, int n2)
        {
            return n1 + n2; // return addition results
        }

        static void Main(string[] args)
        {
            // Test Stub 
            "Test Stub code to test 3".title();
            CodeToTest3 code = new CodeToTest3();
            int result = code.sum(1, 2);
            Console.WriteLine("Sum is: {0}\n", result);
            Console.ReadKey();
        }
    }
}

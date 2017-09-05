//////////////////////////////////////////////////////////////////////////////////
// CodeToTest2.cs - Code that Client needs to Tests                             //
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
 * int multiply(int n1, int n2)
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
 * class CodeToTest2
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
    public class CodeToTest2
    {
        public int multiply(int n1, int n2)
        {
            return (n1 * n2);  // return Multiply results
        }
        static void Main(string[] args)
        {
            // Test Stub
            CodeToTest2 code = new CodeToTest2();
            int result = code.multiply(1, 2);
            Console.WriteLine("Multiplication result is: {0}\n", result);
            Console.ReadKey();
        }
    }
}

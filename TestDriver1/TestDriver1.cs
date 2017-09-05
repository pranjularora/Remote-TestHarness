///////////////////////////////////////////////////////////////////////////////////////////
// TestDriver1.cs - Test Driver to test Clients Code                                     //
// ver 2.0                                                                               //
// Application: CSE681 - Software Modelling and Analysis, Project #4                     //
// Author:      Pranjul Arora, Syracuse University,                                      //
//              parora01@syr.edu, (315) 949-9061                                         //
///////////////////////////////////////////////////////////////////////////////////////////

/*
 * Module Operations:
 * ==================
 * This Module is responsible for testing the code which the client needs to test.
 * 
 * Function Operations:
 * ===================
 * bool test()
 * calls the subtract function of code to test and checks the returned output with the desired.
 * Returns true or false based on that.
 * 
 * static ITest create()
 * Helps Main() function by creating object of the Test Driver1.
 * Only for providing the Test Stub its functionality.
 * 
 * Main()
 * Acts as Test Stub
 * 
 * 
 * Public Classes:
 * ===================
 * No Public Class defined.
 * It is done to check whether the functionality of one Test Case affects the other Test Cases
 * Also, to check that the exception in one child will not affect the functionality of the 
 * Test Harness as a whole.
 * 
 * 
 * 
 * Public Interface:
 * ===================
 *  ITest;
 *  -> bool test()
 * 
 * 
 *  Maintanence History:
 * --------------------
 * ver 2.0 : 23 Nov 2016
 * Project #4 No changes in this module
 * ver 1.0 : 5 Oct 2016
 *  Project #2
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Executor
{

    public class TestDriver1 : ITest
    {
        private CodeToTest1 code;
        public TestDriver1()
        {
            code = new CodeToTest1();
        }
        public bool test()
        {
            int result = code.subtract(3, 0); // calls the subtract function of CodeToTest2
            if (result == 3)
                return true;
            else
                return false;
        }

        public static ITest create()
        {
            return new TestDriver1();
        }


        static void Main(string[] args)
        {
            // Test Stub
            Console.WriteLine("\n  Local test:\n");

            ITest test = TestDriver1.create();

            if (test.test() == true)
            {
                Console.WriteLine("\n TEST=========>>>> PASSED\n");
            }
            else
            {
                Console.WriteLine("\n TEST=========>>>> FAILED\n");
            }
        }

    }
}
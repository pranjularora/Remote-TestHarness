//////////////////////////////////////////////////////////////////////////////
// TestDriver2.cs - Test Driver to test Clients Code                        //
// ver 2.0                                                                  //
// Application: CSE681 - Software Modelling and Analysis, Project #4        //
// Author:      Pranjul Arora, Syracuse University,                         //
//              parora01@syr.edu, (315) 949-9061                            //
//////////////////////////////////////////////////////////////////////////////

/*
 * Module Operations:
 * ==================
 * This Module is responsible for testing the code which the client needs to test.
 * 
 * Function Operations:
 * ===================
 * bool test()
 * calls the multiply function of code to test and checks the returned output with the desired.
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
 * class TestDriver2 
 * -- defines a constructor and all the above stated function
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
    public class TestDriver2 : ITest
    {
        private CodeToTest2 code;

        public TestDriver2()
        {
            code = new CodeToTest2();
        }

        public static ITest create()
        {
            return new TestDriver2();
        }

        public bool test()
        {
            int result = code.multiply(1, 2); // calls the multiply function of CodeToTest2
            if (result == 2)
                return true;
            else
                return false;
        }

        static void Main(string[] args)
        {
            // Test Stub
            ITest test = TestDriver2.create();

            if (test.test() == true)
            {
                Console.Write("\n TEST=========>>>> PASSED\n");
            }
            else
            {
                Console.Write("\n TEST=========>>>> FAILED\n");
            }
            Console.ReadKey();

        }
    }
}

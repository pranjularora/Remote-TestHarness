using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilitiesExtension;

namespace Executor
{
    public class TestDriver3 : ITest
    {
        private CodeToTest3 code;

        public TestDriver3()
        {
            code = new CodeToTest3();
        }

        public static ITest create() // used for creating an object of this class
        {
            return new TestDriver3();
        }


        public bool test()
        {
            int result = code.sum(1, 2); // call sum function of CodeToTest4
            if (result == 3)
                return true;
            else
                return false;
        }

        static void Main(string[] args)
        {
            // Test Stub
            "Test Stub Test Driver 3".title();
            ITest test = TestDriver3.create();

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

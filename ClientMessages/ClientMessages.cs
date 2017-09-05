///////////////////////////////////////////////////////////////////////////////////
// ClientMessages.cs - defines communication message                             //
// ver 1.0                                                                       //
// Application: CSE681 - Software Modelling and Analysis, Project #4             //
// Author:      Pranjul Arora, Syracuse University,                              //
//              parora01@syr.edu, (315) 949-9061                                 //
///////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * Messages provides helper code for building and parsing XML messages.
 *
 * Public Classes:
 * ===================
 * -- class TestElement
 * -- class TestRequest 
 * -- class ClientMessages
 * 
 *   Maintanence History:
 * --------------------
 * ver 1.0 : 23 Nov 2016
 * Project #4 communication message
 */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilitiesExtension;

namespace ClientMessage
{
    // defining test elements
    public class TestElement
    {
        public string testName { get; set; }
        public string testDriver { get; set; }
        public List<string> testCodes { get; set; } = new List<string>();

        public TestElement()
        { }
        public TestElement(string name)
        {
            testName = name;
        }
        public void addTestDriver(string name)
        {
            testDriver = name;
        }
        public void addTestCode(string name)
        {
            testCodes.Add(name);
        }
        public override string ToString()
        {
            string temp = "\n    test: " + testName;
            temp += "\n      testDriver: " + testDriver;
            foreach (string testCode in testCodes)
                temp += "\n      testCode:   " + testCode;
            return temp;
        }

    }
    // defining test request 
    public class TestRequest
    {
        public string author { get; set; }
        public List<TestElement> tests { get; set; } = new List<TestElement>();
        public TestRequest()
        { }
        public TestRequest(string auth)
        {
            author = auth;
        }
        public override string ToString()
        {
            string temp = "\n  author: " + author;
            foreach (TestElement te in tests)
                temp += te.ToString();
            return temp;
        }

    }


// making different message for different clients
    public class ClientMessages
    {
        public static string makeTestRequest()
        {
            TestElement te1 = new TestElement("test1");
            te1.addTestDriver("TestDriver1.dll");
            te1.addTestCode("CodeToTest1.dll");
            TestElement te2 = new TestElement("test2");
            te2.addTestDriver("TestDriver2.dll");
            te2.addTestCode("CodeToTest2.dll");

            TestRequest tr = new TestRequest();
            tr.author = "Pranjul Arora";
            tr.tests.Add(te1);
            tr.tests.Add(te2);
            return tr.ToXml();
        }

        public static string makeTestRequestClient1()
        {
            TestElement te1 = new TestElement("test3");
            te1.addTestDriver("TestDriver3.dll");
            te1.addTestCode("CodeToTest3.dll");
            
            TestRequest tr = new TestRequest();
            tr.author = "Nik";
            tr.tests.Add(te1);
            
            return tr.ToXml();
        }
        public static string makeTestRequestClient2()
        {
            TestElement te1 = new TestElement("test3");
            te1.addTestDriver("TestDriver4.dll");
            te1.addTestCode("CodeToTest4.dll");

            TestRequest tr = new TestRequest();
            tr.author = "Ashu rana";
            tr.tests.Add(te1);

            return tr.ToXml();
        }
        // Test Stub
        static void Main(string[] args)
        {
            "Testing THMessage Class".title('=');

            TestElement te1 = new TestElement();
            te1.testName = "test1";
            te1.addTestDriver("td1.dll");
            te1.addTestCode("tc1.dll");
            te1.addTestCode("tc2.dll");


            TestElement te2 = new TestElement();
            te2.testName = "test2";
            te2.addTestDriver("td2.dll");
            te2.addTestCode("tc3.dll");
            te2.addTestCode("tc4.dll");

            TestRequest tr = new TestRequest();
            tr.author = "Pranjul Arora";
            tr.tests.Add(te1);
            tr.tests.Add(te2);

            string trXml = tr.ToXml();
            Console.WriteLine("\n  Serialized TestRequest data structure:\n\n  {0}\n", trXml);
            Console.ReadKey();



        }
    }
}

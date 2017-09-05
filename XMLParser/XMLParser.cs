///////////////////////////////////////////////////////////////////////////////////
// XMLParser.cs - XML String Parser                                              //
// ver 2.0                                                                       //
// Application: CSE681 - Software Modelling and Analysis, Project #4             //
// Author:      Pranjul Arora, Syracuse University,                              //
//              parora01@syr.edu, (315) 949-9061                                 //
///////////////////////////////////////////////////////////////////////////////////


/*
 * Module Operations:
 * ==================
 * This module is responsible for Parsing the XML string and returning results to the Loader.
 * 
 * 
 * Function Operations:
 * ===================
 * Queue<KeyValuePair<string, string>> Parse(string xmlString)
 * This function is responsible for parsing the xml string based on some parameter and retrieve results.
 * Receives XML string as a parameter 
 * Returns Queue of type keyvalue pair of strings to the Child Thread.
 * Also:
 * -- It throws exception in case of Null or some invalid Data provided in the xml string.
 * -- call XMLInfo Class
 * 
 * void show()
 * The responsiblity of this function is to facilititate stub with the output containing information of the parsed 
 * xml string.
 * 
 * Main() function:
 * Acts as a Test Stub.
 * 
 * Public Classes:
 * ===================
 * -- class XMLTest
 * --> defines a constructor that initializes objects
 * --> defines parse () function
 * 
 * --class XMLInfo
 * --> define show() funciton
 * --> define Main() function
 * 
 * 
 * Maintanence History:
 * --------------------
 * ver 2.0 : 23 Nov 2016
 * Project #4 Now this moduled is called by the child thread instead of loader
 * ver 1.0 : 5 Oct 2016
 *  Project #2
 */



using ClientMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UtilitiesExtension;

namespace XMLParser
{
    public class XMLTest
    {
        private XDocument doc;
        private List<XMLInfo> xmlInfo;

        public XMLTest()
        {
            doc = new XDocument();
            xmlInfo = new List<XMLInfo>();
        }

        public List<XMLInfo> parse(string xml)
        {
            try
            {
                doc = XDocument.Parse(xml); // Parsing the xml String
                if (doc == null)
                    return xmlInfo;

                string author = doc.Descendants("author").First().Value;

                XElement[] xtests = doc.Descendants("TestElement").ToArray();
                int numTests = xtests.Count();
                for (int i = 0; i < numTests; i++)
                {
                    XMLInfo info = new XMLInfo();
                    info.testCode = new List<String>();
                    info.author = author;

                    info.testName = xtests[i].Element("testName") != null ? xtests[i].Element("testName").Value : "false";
                    info.testDriver = xtests[i].Element("testDriver").Value; // Element because test driver is the child now
                    IEnumerable<XElement> xtestCode = xtests[i].Elements("testCodes");
                    foreach (var xlibrary in xtestCode)
                    {
                        info.testCode.Add(xlibrary.Value);
                    }
                    xmlInfo.Add(info); // Adding the information to XMLInfo Object
                }
                return xmlInfo;
            }
            catch (Exception e)
            { // Exception will be caught when XML data is not correct
                Console.WriteLine("There is Some Problem Reading Test Request\n");
                Console.WriteLine("Exception is thrown with message: {0}\n", e.Message);
                return null;
            }
        }
    }

    public class XMLInfo
    {
        // Print all the parsed information
        // Function used by the test Stub
        public string testName { get; set; }
        public string author { get; set; }
        public DateTime timeStamp { get; set; }
        public String testDriver { get; set; }
        public List<String> testCode { get; set; }
        public void show()
        {
            Console.Write("\n {0,-12} : {1}", "test name", testName);
            Console.Write("\n {0,12} : {1}", "author", author);
            Console.Write("\n {0,12} : {1}", "Time Stamp", timeStamp);
            Console.Write("\n {0,12} : {1}", "Test Driver", testDriver);
            foreach (string library in testCode)
            {
                Console.Write("\n {0,12} : {1}", "library", library);
            }
        }

    }
    public class XMLParser
    {
        public Queue<KeyValuePair<string, string>> Parse(string xmlString)
        {
            List<XMLInfo> xmlInfo = null;
            Queue<KeyValuePair<string, string>> msgBody = new Queue<KeyValuePair<string, string>>();

            XMLTest testReq = new XMLTest();
            xmlInfo = testReq.parse(xmlString);
            if (xmlInfo == null)
                return null;
            else
            {
                foreach (XMLInfo x in xmlInfo)
                {
                    if (msgBody.Count == 0)
                    {
                        msgBody.Enqueue(new KeyValuePair<string, string>("author", x.author));
                    }
                    msgBody.Enqueue(new KeyValuePair<string, string>(x.testName, x.testDriver));
                    foreach (string code in x.testCode)
                    {
                        msgBody.Enqueue(new KeyValuePair<string, string>(x.testName, code));
                    }

                }
            }
            return msgBody;
        }


        static void Main(string[] args)
        {
            // Test Stub
            "XML Parser STUB ====>>>>>>>>> ".title('=');
            XMLParser xmlParser = new XMLParser();
            string message_content = ClientMessages.makeTestRequest();
            Queue<KeyValuePair<string, string>> xml_Info_queue = new Queue<KeyValuePair<string, string>>();

            xml_Info_queue = xmlParser.Parse(message_content);
            Console.WriteLine("Parsed Message: \n");
            foreach (KeyValuePair<string, string> x in xml_Info_queue)
            {
                Console.WriteLine("{0} -- {1}", x.Key, x.Value);
            }


            Console.ReadKey();
        }
    }
}

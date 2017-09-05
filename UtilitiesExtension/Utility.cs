////////////////////////////////////////////////////////////////////////////////////
// Utilities.cs - Extra Utilities for other modules                               //
// ver 2.0                                                                        //
// Application: CSE681 - Software Modelling and Analysis, Project #4              //
// Author:      Pranjul Arora, Syracuse University,                               //
//              parora01@syr.edu, (315) 949-9061                                  //
////////////////////////////////////////////////////////////////////////////////////

/*
 * Module Operations:
 * ================== 
 * The Task of this module is to provide display functionality to other modules, so that no redundant priting patterns 
 * needs to be included in the functions.
 * 
 * For Now, this has been used in:
 * -> Lodaer.cs
 * -> Client.cs
 * -> AppDomain<Manager.cs
 * -> Executor.cs
 * -> Utilities.cs -- In the stub
 * 
 * 
 * Function Operations:
 * ===================
 *  public static string ToXml(this object obj)
 *  
 *  converts string to a xml format and then returns that string
 *  
 *  static public T FromXml<T>(this string xml)
 *  
 *  converts xml format to string format.(as per usage in the Project #4)
 * 
 *  public static void title(this string aString, char underline = '-')
 *  
 *  This accepts a string as one of its parameters and Display the content with a certain pattern using '-'.
 *  This is the case when second argument is not given.
 *  With a second argument, it displays the pattern with '='.
 *  
 * Main() function:
 * Acts as a Test Stub.
 * 
 * Public Classes:
 * ===================
 * -- class ToAndFromXml --> define toXml() and FromXml()
 * -- Class Test_Utilities --> defines void title() function
 * -- Class Utilities --> defines a Main Function acting as a STUB
 * 
 * 
 * Maintanence History:
 * --------------------
 * ver 2.0 : 23 Nov 2016
 * Project #4 added class ToAndFromXml
 * ver 1.0 : 5 Oct 2016
 *  Project #2
 */



using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilitiesExtension
{
    public static class ToAndFromXml
    {
        public static string ToXml(this object obj)
        {
            XmlSerializerNamespaces nmsp = new XmlSerializerNamespaces();
            nmsp.Add("", "");

            var sb = new StringBuilder();
            try
            {
                var serializer = new XmlSerializer(obj.GetType());
                using (StringWriter writer = new StringWriter(sb))
                {
                    serializer.Serialize(writer, obj, nmsp);
                }
            }
            catch (Exception ex)
            {
                Console.Write("\n  exception thrown:");
                Console.Write("\n  {0}", ex.Message);
            }
            return sb.ToString();
        }

        static public T FromXml<T>(this string xml)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(new StringReader(xml));
            }
            catch (Exception ex)
            {
                Console.Write("\n  deserialization failed\n  {0}", ex.Message);
                return default(T);
            }
        }


    }
    public static class Test_Utilities
    {
        // Print the string in a specific format
        public static void title(this string aString, char underline = '-')
        {
            Console.WriteLine("\n{0}", new string(underline, aString.Length + 2));

            Console.WriteLine(" {0}", aString);

            Console.WriteLine("{0}", new string(underline, aString.Length + 2));

        }


    }
    class Utility
    {
        static void Main(string[] args)
        {
            // Test Stub
            "This is a title test without parameter".title();
            Console.Write("\n\n");
            "This is a title test with parameter".title('=');
            Console.Write("\n\n");

            List<string> dllList = new List<string>();
            dllList.Add("TestDriver2.dll");
            dllList.Add("CodeToTest2.dll");
            string message_content = dllList.ToXml();
            Console.WriteLine("After using ToXml--> \n{0}", message_content);

            HashSet<string> filesList = message_content.FromXml<HashSet<string>>();

            Console.WriteLine("\n\nAfter using FromXml() --> \n");
            foreach (var x in filesList)
            {
                Console.WriteLine(x);
            }
                Console.ReadKey();
        }
    }
}

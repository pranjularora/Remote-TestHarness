///////////////////////////////////////////////////////////////////////////////////
// Messages.cs - defines extension methods                                       //
// ver 1.0                                                                       //
// Application: CSE681 - Software Modelling and Analysis, Project #4             //
// Author:      Pranjul Arora, Syracuse University,                              //
//              parora01@syr.edu, (315) 949-9061                                 //
///////////////////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * Defines extension methods
 *
 * Public Classes:
 * ===================
 * -- class extMethods
 * --  class Messages
 * 
 *   Maintanence History:
 * --------------------
 * ver 1.0 : 23 Nov 2016
 * Project #4 extension message message
 */




using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MessageCreate
{
    public static class extMethods
    {
        // display function used when a message arrives
        public static void show(this Messages msg, int shift = 2)
        {
            Console.Write("\n  formatted message:");
            string[] lines = msg.ToString().Split(',');
            foreach (string line in lines)
                Console.Write("\n    {0}", line.Trim());
            Console.WriteLine();
        }
        // display function used when test harness sends test results, as special formatting is required
        public static void showLogs(this Messages msg, int shift = 2)
        {
            Console.Write("\n  formatted message:");
            string[] lines = msg.ToString().Split(',');
            int counter = 0;
            foreach (string line in lines)
            {
                if (line.Contains("body"))
                    counter = 1;
                if (counter == 0)
                    Console.Write("\n    {0}", line.Trim());
                
                
            }
            Console.WriteLine();
        }

       
    }

    // used for creating message elements
    [Serializable]
    public class Messages
    {
        public string toUrl { get; set; }
        public string fromUrl { get; set; }
        public string command { get; set; } = "default";
        public string author { get; set; } = "";
        public string message_content { get; set; } = "none";
        public DateTime time { get; set; } = DateTime.Now;

        public List<string> messageTypes { get; set; } = new List<string>();

        public Messages()
        {
            messageTypes.Add("TestRequest");
            message_content = "";
        }
        public Messages(string bodyStr)
        {
            messageTypes.Add("TestRequest");
            message_content = bodyStr;
        }

        public Messages fromString(string msgStr)
        {
            Messages msg = new Messages();
            try
            {
                string[] parts = msgStr.Split(',');
                for (int i = 0; i < parts.Count(); ++i)
                    parts[i] = parts[i].Trim();

                msg.toUrl = parts[0].Substring(4);
                msg.fromUrl = parts[1].Substring(6);
                msg.command = parts[2].Substring(6);
                msg.author = parts[3].Substring(8);
                msg.time = DateTime.Parse(parts[4].Substring(6));
                if (parts[5].Count() > 6)
                    msg.message_content = parts[5].Substring(6);
            }
            catch
            {
                Console.Write("\n  string parsing failed in Message.fromString(string)");
                return null;
            }
            return msg;
        }

        public override string ToString()
        {
            string temp = "to: " + toUrl;
            temp += ", from: " + fromUrl;
            temp += ", type: " + command;
            if (author != "")
                temp += ", author: " + author;
            temp += ", time: " + time;
            temp += ", body:\n" + message_content;
            return temp;
        }

        // Test Stub
        static void Main(string[] args)
        {
            Console.Write("\n  Testing Message Class");
            Console.Write("\n =======================\n");

            Messages msg = new Messages();
            msg.toUrl = "http://localhost:8080/ICommunicator";
            msg.fromUrl = "http://localhost:8081/ICommunicator";
            msg.author = "Pranjul";
            msg.command = "TestRequest";
            msg.show();
            Console.Write("\n\n");
            Console.ReadKey();
        }
    }
}

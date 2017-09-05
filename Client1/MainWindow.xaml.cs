///////////////////////////////////////////////////////////////////////////////////
// Client1.cs - client                                                            //
// ver 1.0                                                                       //
// Application: CSE681 - Software Modelling and Analysis, Project #4             //
// Author:      Pranjul Arora, Syracuse University,                              //
//              parora01@syr.edu, (315) 949-9061                                 //
///////////////////////////////////////////////////////////////////////////////////
/*
 *
 * 
 * 
 * Module Operations:
 * ==================
 * WPF CLient
 * The functionality of this module is:
 * ==> Opens a port for listening.
 * ==> send DLL to repository
 * ==> send test request to the test harness server
 * ==> displays message after receiving from repository or test harness server
 * 
 * Function Operations:
 * ===================
 * public MainWindow()
 * starting point of WPF client
 * make send button disabled
 * 
 * OnNewMessageHandler(string msg)
 * handles message from dispacher and updates the listbox2 which is on the UI thread 
 * 
 *  void ListenButton_Click(object sender, RoutedEventArgs e)
 *  listen button functionality
 *  
 *  void SendMessageButton_Click(object sender, RoutedEventArgs e)
 *  send button functionality
 * 
 *  public void wait()
 * wait for receiver thread to end

 * void rcvThreadProc()
 * After message gets dequeued, this function gets the meesage from repository or test harness server
 * check the type of message and perform the required functionality as per the type of the message.
 * 
 * Public Classes:
 * ===================
 * class MainWindow--
 *  defines above stated functions
 *  
 * 
 *
 * 
 * 
 *  
 *  Maintanence History:
 * --------------------
 * ver 1.0 : 23 Nov 2016
 * Project #4 WPF client
 * 
 *  
 */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ServiceModel;
using ClientCommunication;
using System.Threading;
using MessageCreate;
using Client_File_Manager;
using ClientMessage;
// client with end point 8084
namespace Client1
{
    public partial class MainWindow : Window
    {
        private Comm<MainWindow> comm { get; set; } = new Comm<MainWindow>();

        private string endPoint { get; } = Comm<MainWindow>.makeEndPoint("http://localhost", 8084);

        private Thread rcvThread = null;
        int MaxMsgCount = 100;
        static int position_listBox2 = 0;
        delegate void NewMessage(string msg);
        event NewMessage OnNewMessage;

        public MainWindow() // constructor
        {
            InitializeComponent();
            SendButton.IsEnabled = false;
            OnNewMessage += new NewMessage(OnNewMessageHandler);
        }

        void OnNewMessageHandler(string msg) // handles content of list box2 on the UI thread
        {
            var dict = msg.Split(';');
            string temp = null;
            foreach (string line in dict)
            {
                temp = line.Replace(",", ":  ");
                listBox2.Items.Insert(position_listBox2, temp);
                position_listBox2++;
            }

            if (listBox2.Items.Count > MaxMsgCount)
                listBox2.Items.RemoveAt(listBox2.Items.Count - 1);
        }
        // listen button fucntionality
        private void ListenButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                comm.rcvr.CreateRecvChannel(endPoint);
                rcvThread = comm.rcvr.start(rcvThreadProc);
                rcvThread.IsBackground = true;
                SendButton.IsEnabled = true;
                ListenButton.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Window temp = new Window();
                StringBuilder msg = new StringBuilder(ex.Message);
                msg.Append("\nport = ");
                // msg.Append(localPort.ToString());
                temp.Content = msg.ToString();
                temp.Height = 100;
                temp.Width = 500;
                temp.Show();
            }
        }
        // send message button fucntionality
        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SendButton.IsEnabled = false;
                ClientFileManager fileManager = new ClientFileManager();
                //////// Implementing code for sending DLL's to repository before sending the test request.

                fileManager.client_Manager("client1");

                // creating messages to send to Test Harness Server
                Messages msg = makeMessage("nik", endPoint, endPoint);
                string TestHarnessEndPoint = Comm<MainWindow>.makeEndPoint("http://localhost", 8080);
                msg.toUrl = TestHarnessEndPoint;
                msg.command = "TestRequest";
                msg.message_content = ClientMessages.makeTestRequestClient1();
                listBox1.Items.Insert(0, msg);
                if (listBox1.Items.Count > MaxMsgCount)
                    listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
                comm.sndr.PostMessage(msg);

            }
            catch (Exception ex)
            {
                Window temp = new Window();
                temp.Content = ex.Message;
                temp.Height = 100;
                temp.Width = 500;
            }
        }


        public Messages makeMessage(string author, string fromEndPoint, string toEndPoint)
        {
            Messages message = new Messages();
            message.author = author;
            message.fromUrl = fromEndPoint;
            message.toUrl = toEndPoint;
            return message;
        }

        public void wait()
        {
            rcvThread.Join();
        }
        // receiver thread..
        void rcvThreadProc()
        {
            while (true)
            {
                Messages msg = comm.rcvr.GetMessage();
                msg.time = DateTime.Now;
                string from = null;
                // this will check from which port the message arrived and add a string "from:" to the message.from
                if (msg.fromUrl.Contains("8080"))
                {
                    from = "from: TestHarness- " + msg.fromUrl;
                }
                // calling UI thread to update listbox2 ==> i.e. of received messages.
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, OnNewMessage, from);
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, OnNewMessage, msg.message_content);
            }
        }

    }




}

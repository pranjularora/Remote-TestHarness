/////////////////////////////////////////////////////////////////////////////
//  BlockingQueue.cs - demonstrate threads communicating via Queue         //
// ver 1.0                                                                 //
// Application: CSE681 - Software Modelling and Analysis, Project #4       //
// Author:      Pranjul Arora, Syracuse University,                        //
//              parora01@syr.edu, (315) 949-9061                           //
/////////////////////////////////////////////////////////////////////////////
/*
 *   Module Operations
 *   -----------------
 *   This package implements a generic blocking queue and demonstrates 
 *   communication between two threads using an instance of the queue. 
 *   If the queue is empty when a reader attempts to deQ an item then the
 *   reader will block until the writing thread enQs an item.  Thus waiting
 *   is efficient.
 * 
 *   Public Interface
 *   ----------------
 *   BlockingQueue<string> bQ = new BlockingQueue<string>();
 *   bQ.enQ(msg);
 *   string msg = bQ.deQ();
 * 
 */
/*
 *   Build Process
 *   -------------
 *   - Required files:   BlockingQueue.cs, Program.cs
 *   - Compiler command: csc BlockingQueue.cs Program.cs
 * 
 *  *  Maintanence History:
 * --------------------
 * ver 1.0 : 23 Nov 2016
 * Project #4 Blocking queue
 * 
 */

using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;
using static System.Console;

namespace BlockingQueue
{
    public class BlockingQueue<T>
    {
        private Queue blockingQ;
        object locker = new object();

        // --- Defining its constructor

        public BlockingQueue()
        {
            blockingQ = new Queue();
        }

        // --- < enqueue a string >
        public void enQ(T msg)
        {
            lock(locker)
            {
                blockingQ.Enqueue(msg);
                Monitor.Pulse(locker);
            }
        }


        //----< dequeue a T >
        public T deQ()
        {
            T msg = default(T);
            lock(locker)
            {
                while(this.size() == 0)
                {
                    Monitor.Wait(locker);
                }
                msg = (T)blockingQ.Dequeue();
                return msg;
            }
        }

        public int size()
        {
            int count;
            lock(locker)
            {
                count = blockingQ.Count;
            }
            return count;
        }

    }

    // Test Stub
    class stub
    {
        static void Main(string[] args)
        {
            Console.Write("\n  Testing Monitor-Based Blocking Queue");
            Console.Write("\n ======================================");

            BlockingQueue<string> q = new BlockingQueue<string>();

            Thread t = new Thread(() =>
            {
                string msg;
                while(true)
                {
                    msg = q.deQ();
                    WriteLine("\n child Thread received {0}", msg);
                    if (msg == "quit")
                        break;
                }
            });
            t.Start();

            string sendMessage = "msg #";
            for (int i = 0; i<20; i++)
            {
                string temp = sendMessage + i.ToString();
                Write("\n  main thread sending {0}", temp);
                q.enQ(temp);
            }
            q.enQ("quit");
            t.Join();
            ReadKey();


        }
    }
}

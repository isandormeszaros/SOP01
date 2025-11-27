using System;
using System.Collections.Generic;
using System.Threading;

namespace gyakorlas_Termelo_Fogyaszto
{
    internal class Program
    {
        static Queue<int> buffer = new Queue<int>();
        static object bufferLock = new object();
        static int bufferLimit = 5;

        static void Main(string[] args)
        {
            new Thread(Producer).Start();
            new Thread(Consumer).Start();
        }

        static void Producer()
        {
            Console.WriteLine("Producer indul");
            int termekID = 0;

            while (true)
            {
                lock (bufferLock)
                {
                    while (buffer.Count == bufferLimit)
                    {
                        Console.WriteLine("[PRODUCER] Buffer tele! Várakozok…");
                        Monitor.Wait(bufferLock);
                    }

                    buffer.Enqueue(termekID);
                    Console.WriteLine($"[PRODUCER] Beraktam: {termekID}");
                    termekID++;

                    Monitor.PulseAll(bufferLock);
                }

                Thread.Sleep(500);
            }
        }

        static void Consumer()
        {
            Console.WriteLine("Consumer indul");

            while (true)
            {
                lock (bufferLock)
                {
                    while (buffer.Count == 0)
                    {
                        Console.WriteLine("[CONSUMER] Buffer üres! Várakozok…");
                        Monitor.Wait(bufferLock);
                    }

                    int termek = buffer.Dequeue();
                    Console.WriteLine($"[CONSUMER] Kivettem: {termek}");

                    Monitor.PulseAll(bufferLock);
                }

                Thread.Sleep(800);
            }
        }
    }
}

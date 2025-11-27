using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gyakorlas_Lock
{
    internal class Program
    {
        static int sharedCounter = 0;
        static object sharedLock = new object();
        static void Main(string[] args)
        {
            Thread worker1 = new Thread(WorkerMethod);
            Thread worker2 = new Thread(WorkerMethod);
            worker1.Start();    
            worker2.Start();

            worker1.Join();
            worker2.Join(); //megvárja a főszál mig mindkét mellékszál befejezi a munkát
           

            Console.WriteLine("Ez itt a főszál.");
            Console.WriteLine($"A counter végső értéke: {sharedCounter}");

            Console.ReadKey();
        }

        static void WorkerMethod()
        {
            for (int i = 0; i < 10000000; i++) {
                //ezzel lockolom a műveletet
                lock (sharedLock) {
                    sharedCounter++;
                }
            }
        }
    }
}
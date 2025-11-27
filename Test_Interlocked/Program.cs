using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test_Interlocked
{
    internal class Program
    {
        static int counter = 0;
        static void Main(string[] args)
        {
            Thread worker1 = new Thread(WorkerMethod);
            Thread worker2 = new Thread(WorkerMethod); //létrehozom a workert
            worker1.Start(); 
            worker2.Start(); //elindítom a workert

            Console.WriteLine("Főszál fut");

            worker1.Join(); 
            worker2.Join(); //a főszáll megáll és megvárja míg a mellékszál befejezi a munkát

            Console.WriteLine(counter);
            Console.ReadKey();
        }

        static void WorkerMethod()
        {
            for (int i = 0; i < 10_000_000; i++) {
                // counter++; ez egy hibás működést eredményez: race condition
                Interlocked.Increment(ref counter);
            }
        }
    }
}

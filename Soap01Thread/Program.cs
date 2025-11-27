using System;
using System.Diagnostics.Metrics;
using System.Threading;

namespace Soap01Thread
{
    internal class Program
    {
        static ManualResetEvent mre = new ManualResetEvent(false);
        static volatile bool shouldStop = false;

        public static void WorkerMethod()
        {
            Console.WriteLine($"{Thread.CurrentThread.Name} megkezdi a munkát!");

            // Jelez a főszálnak, hogy elindult
            mre.Set();

            while (!shouldStop)
            {
                Console.WriteLine("Mellékszál dolgozik...");
                Thread.Sleep(500); // Szimulált munka
            }

            Console.WriteLine($"{Thread.CurrentThread.Name} befejezte a munkát!");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Fő szál indulása!");

            Thread workerThread = new Thread(WorkerMethod);
            workerThread.Name = "Worker1";

            workerThread.Start();

            // Vár, amíg a mellékszál készen áll
            mre.WaitOne();
            Console.WriteLine("A főszál megkapta, hogy a mellékszál készen áll a munkára!");

            // Várunk egy kicsit, majd leállítjuk a mellékszálat
            Thread.Sleep(2000);
            Console.WriteLine("A főszál vár a mellékszál befejezésére.");
            shouldStop = true;

            workerThread.Join();
            Console.WriteLine("Fő szál leáll");
            Console.ReadKey();
        }
    }
}

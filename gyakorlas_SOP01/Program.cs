using System;
using System.Threading;

namespace gyakorlas_SOP01
{
    internal class Program
    {
        //ez arra kell, hogy a worker tudjon jelezni a főszálnak, hogy már fut
        static ManualResetEvent mre = new ManualResetEvent(false);
        static volatile bool shouldStop = false; // azért kell ide a volatile, mert mindkét szálon fut majd és ezért a legfrissebb értéket kell megkapja


        static void Main(string[] args)
        {
            Console.WriteLine("A főszál elindult!");

            //itt hozzuk létre a mellékszálat
            Thread worker = new Thread(WorkerMethod);
            worker.Name = "Worker1";

            worker.Start(); // itt inditjuk el a workert

            mre.WaitOne(); //megvárja a worker jelzését, hogy elindult


            shouldStop = true;
            worker.Join(); // a főszálnak meg kell várnia, mig a worker tényleg leáll 

            Console.ReadKey();
        }

        static void WorkerMethod()
        {
            Console.WriteLine("A mellékszál elindult!");
            mre.Set(); //jelzek a főszálnak, hogy elindultam.

            while (!shouldStop)
            {
                Console.WriteLine("Most is folyamatosan dolgozom!");
                Thread.Sleep(500);
            }
        }
    }
}

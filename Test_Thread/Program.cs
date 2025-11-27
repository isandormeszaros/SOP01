using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gyakorlas_szalak_1
{
    internal class Program
    {

        static ManualResetEvent mre = new ManualResetEvent(false); //jelzés biztosítása a két szál között
        static volatile bool shouldStop = false;
        static void Main(string[] args)
        {

            Console.WriteLine("Főszál fut");

            Thread worker = new Thread(WorkerThread); //létrehozom a workert
            worker.Start(); //elindítom a workert


            mre.WaitOne(); //a főszál vár a jelzésre

            shouldStop = true;

            worker.Join(); // főszál: várj addig, amíg a worker ténylegesen be nem fejezte a munkát.
        }
        static void WorkerThread()
        {
            Console.WriteLine("A mellékszál is fut most!");
            
            mre.Set(); // a mellékszál jelez a főszál felé, hogy elindult

            while (!shouldStop) {
                Console.WriteLine("Most is folyamatosan dolgozom!");
                Thread.Sleep(500);
            }
        }
    }
}

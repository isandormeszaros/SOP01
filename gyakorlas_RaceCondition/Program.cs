using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gyakorlas_RaceCondition
{
    internal class Program
    {
        static int counter = 0; // A shared resource → több szál → közös változó ezért kell a static
        static void Main(string[] args)
        {
            Thread worker1 = new Thread(workerMethod); //létrehozzuk a workereket
            Thread worker2 = new Thread(workerMethod);
            worker1.Start(); //elinditjuk a workereket
            worker2.Start();    


            Console.WriteLine("A főszál elindult");

            worker1.Join();
            worker2.Join(); //a főszáll megáll és megvárja míg a mellékszál befejezi a munkát

            Console.WriteLine($"A counter végső értéke: {counter}");
            

            Console.ReadKey();
        }

        static void workerMethod()
        {
            for (int i = 0; i < 1000000; i++)
            {
                Interlocked.Increment(ref counter);
                //kiolvassa a counter értékét
                //megnöveli
                //visszaírja
                //és ezt 1 lépésben csinálja
                //NEM lehet közben szálváltás
                //Ezért nem jó a counter++ vagy a counter = counter + 1
            }
        }
    }
}

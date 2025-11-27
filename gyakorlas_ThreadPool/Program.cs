using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gyakorlas_ThreadPool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CountdownEvent countdown = new CountdownEvent(3);
            Console.WriteLine("A főszál elindult");

            ThreadPool.QueueUserWorkItem(WorkerMethod, countdown);
            ThreadPool.QueueUserWorkItem(WorkerMethod, countdown);
            ThreadPool.QueueUserWorkItem(WorkerMethod, countdown);


            Console.WriteLine("Főszál nem vár, megy tovább!");

            

            countdown.Wait();
            



            Console.ReadKey();
        }

        static void WorkerMethod(object state) // itt az objektum átadása kötelező még akkor is, ha nem használjuk 
        {

            // 1: visszaalakítjuk CountdownEventté
            CountdownEvent countdown = (CountdownEvent)state; //castoljuk CountdownEventté

            Console.WriteLine($"ThreadPool worker fut.");

            // 3: jelezzük a főszálnak, hogy ez a worker befejezte a munkát
            countdown.Signal();
        }
    }
}

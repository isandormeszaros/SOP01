using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test_ThreadPool
{
    internal class Program
    {
        static CountdownEvent countdown = new CountdownEvent(3);
        static void Main(string[] args)
        {
            ThreadPool.QueueUserWorkItem(WorkerMethod, countdown);
            ThreadPool.QueueUserWorkItem(WorkerMethod, countdown);
            ThreadPool.QueueUserWorkItem(WorkerMethod, countdown);

            Console.WriteLine("Főszál fut");

            countdown.Wait();

            Console.ReadKey();
        }

        static void WorkerMethod(object state) //ide kötelezően kell az object state, még akkor is ha nem adunk át neki semmit sem
        {
            // 1: visszaalakítjuk CountdownEventté
            CountdownEvent countdown = (CountdownEvent)state;

            Console.WriteLine($"ThreadPool worker fut.");

            // 3: jelezzük a főszálnak, hogy ez a worker befejezte a munkát
            countdown.Signal();
        }
    }
}

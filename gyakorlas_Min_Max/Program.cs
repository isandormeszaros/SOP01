using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gyakorlas_Min_Max
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<int> szamok = new List<int>();
            Random rnd = new Random();

            for (int i = 0; i < 1000; i++)
            {
                szamok.Add(rnd.Next(0, 1000));
            }

            int maxValue = int.MinValue;
            int maxCount = 0;

            int minValue = int.MaxValue;
            int minCount = 0;

            CountdownEvent countdown = new CountdownEvent(2);
            object[] maxCsomag = { szamok, countdown, maxValue, maxCount };
            object[] minCsomag = { szamok, countdown, minValue, minCount };

            ThreadPool.QueueUserWorkItem(MinWorker, minCsomag);
            ThreadPool.QueueUserWorkItem(MaxWorker, maxCsomag);

            countdown.Wait();

            minValue = (int)minCsomag[2];
            minCount = (int)minCsomag[3];

            maxValue = (int)maxCsomag[2];
            maxCount = (int)maxCsomag[3];

            Console.WriteLine($"Minimum: {minValue}, Előfordulás: {minCount}");
            Console.WriteLine($"Maximum: {maxValue}, Előfordulás: {maxCount}");

            Console.ReadKey();
        }

        static void MinWorker(object state) 
        { 
            object[] csomag = (object[])state;

            List<int> lista = (List<int>)csomag[0];
            CountdownEvent countdown = (CountdownEvent)csomag[1];
            int minValue = (int)csomag[2];
            int minCount = (int)csomag[3];

            Console.WriteLine("MinWorker fut");

            foreach (int szam in lista)
            {
                if (szam < minValue)
                {
                    minValue = szam;
                    minCount = 1;        // új minimum → számláló újra 1
                }
                else if (szam == minValue)
                {
                    minCount++;          // találtunk még egyet
                }
            }

            csomag[2] = minValue;
            csomag[3] = minCount;


            countdown.Signal();
        }
        static void MaxWorker(object state)
        {
            object[] csomag = (object[])state;

            List<int> lista = (List<int>)csomag[0];
            CountdownEvent countdown = (CountdownEvent)csomag[1];
            int maxValue = (int)csomag[2];
            int maxCount = (int)csomag[3];

            Console.WriteLine("MaxWorker fut");

            foreach (int szam in lista)
            {
                if (szam > maxValue)
                {
                    maxValue = szam;
                    maxCount = 1;        // új maximum
                }
                else if (szam == maxValue)
                {
                    maxCount++;          // találtunk még egyet
                }
            }

            csomag[2] = maxValue;
            csomag[3] = maxCount;


            countdown.Signal();
        }
    }
}

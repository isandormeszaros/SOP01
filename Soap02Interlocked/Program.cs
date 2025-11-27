using System;
using System.Collections.Generic;
using System.Threading;

namespace Soap02Interlocked
{
    internal class Program
    {
        static int counter = 0;

        static void IncrementCounter(int times)
        {
            for (int i = 0; i < times; i++)
            {
                //1. Counter érték kiolvasása counter == x = 0 
                //Szálváltás 
                //1. Counter érték kiolvasása counter == x 
                //2. counter növelése counter + 1 
                // Szálváltás Szál 1 
                //2. counter növelése counter + 1 
                //3. counter visszaírása conter = counter + 1 = 1 
                //Szálváltás 
                //3. counter visszaírása counter = counter + 1 = 1 
                
                //Versenyhelyzet - Race Condition

                Interlocked.Increment(ref counter);
                Console.WriteLine($"{Thread.CurrentThread.Name} növelte a számlálót. Új érték: {counter}");

                Thread.Sleep(1000); // csak demonstráció céljából
            }
        }

        static void Main(string[] args)
        {
            int threadCount = 5;
            int incrementPerThread = 30;

            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < threadCount; i++)
            {
                Thread t = new Thread(() => IncrementCounter(incrementPerThread));
                t.Name = $"Thread-{i + 1}";
                threads.Add(t);
            }

            // Szálak indítása
            foreach (var t in threads)
            {
                t.Start();
            }

            // Szálak befejezésének megvárása
            foreach (var t in threads)
            {
                t.Join();
            }

            Console.WriteLine($"A számláló végső értéke: {counter}");
            Console.WriteLine($"Elvárt eredmény: {threadCount * incrementPerThread}");
        }
    }
}

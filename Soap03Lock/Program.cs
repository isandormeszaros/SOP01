namespace Soap03Lock
{
    internal class Program
    {
        static int sharedCounter = 0;
        static object objectLock = new object();
        static void incrementWorker(int times)
        {
            for (int i = 0; i< times; i++)
			{
                lock (objectLock)
                {
                    sharedCounter++;
                    Console.WriteLine($"Szál {Thread.CurrentThread.Name} növelve +" +
                        $"Számláló értéke: {sharedCounter}");
                }

                Thread.Sleep( 20 );
            }

        }

        static void Main(string[] args)
        {
            int threadNumber = 5;
            int incerementsPerThread = 100;
            List<Thread> threads = new List<Thread>();

            Console.WriteLine($"Várható érték {threadNumber * incerementsPerThread}");

            for (int i = 0; i < threadNumber; i++)
            {
                Thread t = new Thread(() => incrementWorker(incerementsPerThread));
                    t.Name = i.ToString();
                threads.Add(t);

                t.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            Console.WriteLine($"Folyamat vége. Végső érték {sharedCounter}");
        }
    }
}

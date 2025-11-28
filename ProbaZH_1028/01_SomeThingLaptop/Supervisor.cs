using Microsoft.VisualBasic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_SomeThingLaptop
{
    class Supervisor
    {
        static int maxLaptopOnLine = 20;
        public static List<Laptop> laptops = new List<Laptop>();

        public static int stackNumber = 3;

        public static ConcurrentBag<List<Laptop>> boxedLaptops = new ConcurrentBag<List<Laptop>>();

        static List<Producer> producers = new List<Producer>();
        static List<Consumer> consumers = new List<Consumer>();

        public static List<Thread> producerConsumerThreads = new List<Thread>();

        public static volatile bool areProducersRunning = false;
        public static volatile bool areConsumersRunning = false;

        private static readonly object stateLock = new object();

        public static bool TryProduce(Producer producer, Laptop laptop)
        {
            bool success = false;
            Monitor.Enter(laptops);
            while (laptops.Count == maxLaptopOnLine)
            {
                Console.WriteLine($"{producer} is waiting...");
                Monitor.Wait(laptops);
            }

            if (laptops.Count < maxLaptopOnLine)
            {
                laptops.Add(laptop);
                Console.WriteLine($"{producer} has produced {laptop}...");
                success = true;
            }

            Monitor.PulseAll(laptops);
            Monitor.Exit(laptops);
            return success;
        }

        public static bool TryConsume(Consumer consumer, out Laptop? laptop)
        {
            bool success = false;
            laptop = null;

            Monitor.Enter(laptops);
            try
            {
                while (true)
                {
                    laptop = laptops.FirstOrDefault(l =>
                        l.LaptopBrand == consumer.consumeBrand &&
                        l.SerialNumber == consumer.currentSerial);

                    if (laptop != null)
                    {
                        laptops.Remove(laptop);
                        Console.WriteLine($"{consumer} has packed {laptop}...");
                        success = true;
                        Monitor.PulseAll(laptops);
                        break;
                    }

                    if (!areProducersRunning)
                    {
                        break;
                    }

                    Console.WriteLine($"{consumer} is waiting (no match found)...");
                    Monitor.Wait(laptops);
                }
            }
            finally
            {
                Monitor.Exit(laptops);
            }
            return success;
        }

        public static void CheckProducerWorkingState()
        {
            bool producersHaveStopped = false;
            lock (stateLock)
            {
                if (!areProducersRunning)
                {
                    return;
                }

                foreach (Producer producer in producers)
                {
                    if (producer.IsWorking)
                    {
                        return;
                    }
                }

                areProducersRunning = false;
                producersHaveStopped = true;
            }

            if (producersHaveStopped)
            {
                Monitor.Enter(laptops);
                try
                {
                    Monitor.PulseAll(laptops);
                }
                finally
                {
                    Monitor.Exit(laptops);
                }
            }
        }

        public static void CheckConsumerWorkingState()
        {
            lock (stateLock)
            {
                if (!areConsumersRunning)
                {
                    return;
                }

                foreach (Consumer consumer in consumers)
                {
                    if (consumer.IsWorking)
                    {
                        return;
                    }
                }
                Console.WriteLine("Everything has stopped...");
                areConsumersRunning = false;
            }
        }

        public static void WriteOutOneStack()
        {
            List<Laptop> onePack = boxedLaptops.Take(1).First();
            foreach (var laptop in onePack)
            {
                Console.WriteLine($"ID: {laptop.Id}, Brand: {laptop.LaptopBrand}{laptop.SerialNumber}");
            }
        }

        public static void CreateConsumers(int amount, Brand brand, int serialNumber)
        {
            for (int i = 0; i < amount; i++)
            {
                consumers.Add(new Consumer(brand, serialNumber));
            }
        }
        public static void CreateProducers(int amount, int amountToProduce, Brand brand, int serialNumber)
        {
            for (int i = 0; i < amount; i++)
            {
                producers.Add(new Producer(amountToProduce, brand, serialNumber));
            }
        }

        public static void StartProcess()
        {
            areProducersRunning = true;
            foreach (var producer in producers)
            {
                Thread p = new Thread(producer.Work);
                producerConsumerThreads.Add(p);
                p.Start();
            }
            areConsumersRunning = true;
            foreach (var consumer in consumers)
            {
                Thread c = new Thread(consumer.Work);
                producerConsumerThreads.Add(c);
                c.Start();
            }
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_EveryThingPhone
{
    class Supervisor
    {
        // Kiegészitendő
        static int maxPhoneInBuffer = 30; //legyen 30
        public static List<Phone> phones = new List<Phone>();

        public static int stackNumber = 8;

        public static ConcurrentBag<List<Phone>> boxedPhones = new ConcurrentBag<List<Phone>>();

        static List<Producer> producers = new List<Producer>();
        static List<Consumer> consumers = new List<Consumer>();

        public static List<Thread> producerConsumerThreads = new List<Thread>();

        public static volatile bool areProducersRunning = false;
        public static volatile bool areConsumersRunning = false;

        public static bool TryProduce(Producer producer, Phone phone)
        {
            bool success = false;
            Monitor.Enter(phones);
            try
            {
                while (phones.Count == maxPhoneInBuffer)
                {
                    Console.WriteLine($"{producer} is waiting...");
                    Monitor.Wait(phones);
                }
                if (phones.Count < maxPhoneInBuffer)
                {
                    phones.Add(phone);
                    success = true;
                    Monitor.PulseAll(phones);
                }  
            }
            finally
            {
                Monitor.Exit(phones);
            }
            return success;
        }

        public static bool TryConsume(Consumer consumer, out Phone? phone)
        {
            bool success = false;
            Monitor.Enter(phones);
            try
            {
                while (true)
                {
                    phone = phones.FirstOrDefault(p => p.PhoneBrand == consumer.consumeBrand && p.PhoneSpec == consumer.currentSpec);
                    if (phone != null)
                    {
                        phones.Remove(phone);
                        success = true;
                        Monitor.PulseAll(phones);    
                        break;
                    }
                    if (!areProducersRunning) break;

                    Console.WriteLine($"{consumer} is waiting...");
                    Monitor.Wait(phones);
                }
            }
            finally
            {
                Monitor.Exit(phones);
            }
            return success;
        }

        public static void CheckProducerWorkingState()
        {
            Monitor.Enter(producers);
            try
            {
                bool isAnyProducerWorking = producers.Any(p => p.IsWorking);
                if (!isAnyProducerWorking)
                {
                    Console.WriteLine("*** ALL PRODUCERS STOPPED ***");

                    Monitor.Enter(phones);
                    Monitor.PulseAll(phones);
                    Monitor.Exit(phones);
                }
            }
            finally
            {
                Monitor.Exit(producers);
            }
        }

        public static void CheckConsumerWorkingState()
        {
            Monitor.Enter(consumers);
            try
            {
                bool isAnyConsumerWorking = consumers.Any(p => p.IsWorking);
                if (!isAnyConsumerWorking)
                {
                    Console.WriteLine("*** ALL CONSUMERS STOPPED ***");

                    Monitor.Enter(phones);
                    Monitor.PulseAll(phones);
                    Monitor.Exit(phones);
                }
            }
            finally
            {
                Monitor.Exit(consumers);
            }
        }

        public static void WriteOutOneStack()
        {
            if (boxedPhones.TryTake(out List<Phone>? oneBox))
            {
                Console.WriteLine($"*** RANDOM BOX CONTENT ***");
                foreach (var phone in oneBox)
                    Console.WriteLine($"ID:{phone.Id} Brand: {phone.PhoneBrand} Spec:{phone.PhoneSpec}");
            }
            else Console.WriteLine("Box packing unsuccessful.");
        }

        public static void CreateConsumers(int amount, Brand brand, Spec spec)
        {
            for (int i = 0; i < amount; i++)
            {
                consumers.Add(new Consumer(brand, spec));
            }
        }
        public static void CreateProducers(int amount, int amountToProduce, Brand brand, Spec spec)
        {
            for (int i = 0; i < amount; i++)
            {
                producers.Add(new Producer(amountToProduce, brand, spec));
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
                Thread p = new Thread(consumer.Work);
                producerConsumerThreads.Add(p);
                p.Start();
            }
        }
    }
}

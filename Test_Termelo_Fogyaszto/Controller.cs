using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test_Termelo_Fogyaszto
{
    internal static class Controller
    {
        public static List<Product> products = new List<Product>();

        static List<Producer> producers = new List<Producer>();
        static List<Consumer> consumers = new List<Consumer>();

        static int maxLimit = 10; 
        public volatile static bool areProducersWorking = false;


        public static bool TryProduce(Producer producer, Product product)
        {
            bool success = false;

            Monitor.Enter(products);

            while (products.Count == maxLimit) 
            {
                Console.WriteLine($"{producer} is waiting");

                Monitor.Wait(products);
            }

            if (products.Count < maxLimit)
            {
                products.Add(product);
                success = true;
            }

            Monitor.PulseAll(products);



            Monitor.Exit(products);

            return success;
        }

        public static bool TryConsume(Consumer consumer, out Product product)
        {
            bool success  = false;
            product = null;

            Monitor.Enter(products);

            while (products.Count == 0 && areProducersWorking) 
            {
                Monitor.Wait(products);
            }

            if (products.Count > 0)
            { 
                product = products.First();
                products.Remove(product);
                success = true;
            }

            Monitor.PulseAll(products);

            Monitor.Exit(products);

            return success;
        }

        public static void CheckProducerWorkingState()
        {
            foreach (Producer producer in producers)
            {
                if (producer.IsWorking)
                {
                    return;
                }
            }
            areProducersWorking = false;
        }

        public static void CreateProducers(int amount, int amountToProduce)
        {
            for (int i = 0; i < amount; i++)
            {
                producers.Add(new Producer(amountToProduce));
            }
        }

        public static void CreateConsumers(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                consumers.Add(new Consumer());
            }
        }

        public static void StartThreads()
        {
            Console.WriteLine("--- INDÍTÁS ---");
            areProducersWorking = true;

            foreach (Producer producer in producers)
            {
                new Thread(producer.Work).Start();
            }

            foreach (Consumer consumer in consumers)
            {
                new Thread(consumer.Work).Start();
            }
        }
    }
}
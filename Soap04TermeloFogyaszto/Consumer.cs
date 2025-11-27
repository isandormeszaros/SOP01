using System;

namespace Soap04TermeloFogyaszto
{
    internal class Consumer
    {
        private static uint ID;
        private uint id;

        public Consumer()
        {
            id = ID++;
        }

        public void Work()
        {
            Product product = null;

            while (Controller.products.Count > 0 || Controller.areProducersWorking)
            {
                if (Controller.TryConsume(this, out product))
                {
                    Console.WriteLine($"{this} has consumed {product}");
                    product = null;
                }
                else
                {
                    // Ha nincs elérhető termék, kis várakozás elkerülve a busy wait-et
                    Thread.Sleep(100);
                }
            }

            Console.WriteLine($"{this} has finished working.");
        }

        public override string ToString()
        {
            return $"Consumer {id}";
        }
    }
}

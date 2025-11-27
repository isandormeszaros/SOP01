using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test_Termelo_Fogyaszto
{
    internal class Consumer
    {
        static int ID = 0;
        int id;

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
                    Thread.Sleep(100);
                    product = null;
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

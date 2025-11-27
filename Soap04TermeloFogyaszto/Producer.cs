using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soap04TermeloFogyaszto
{
    internal class Producer
    {
        static uint ID;
        uint id;

        int amountProduce;

        int productCount;

        bool isWorking;

        public bool IsWorking
        {
            get => isWorking;
            set => isWorking = value;
        }

        public Producer(int amountToProduce) 
        {
            id = ID;
            this.amountProduce = amountToProduce;
            isWorking = false;
        }

        public void Work() 
        {
            productCount = 0;
            isWorking = true;
            Product product = null;

            while (productCount < amountProduce)
            {
                if (product == null)
                {
                    product = new Product();
                }

                if (Controller.TryProduce(this, product))
                {
                    productCount++;
                    Console.WriteLine($"{this} has producted {product}...");
                    product = null;
                }
            }
            IsWorking = false;
            Controller.CheckProducerWorkingState();
        }

        public override string ToString() 
        {
            return $"Producer {id}";
        }
    }
}

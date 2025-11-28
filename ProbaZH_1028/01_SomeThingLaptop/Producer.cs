using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_SomeThingLaptop
{
    class Producer
    {
        static uint ID;
        uint id;
        int amountToProduce;
        int productCount;

        Brand produceBrand;
        int serialNumber;

        bool isWorking;
        public bool IsWorking { get => isWorking; set => isWorking = value; }

        public Producer(int amountToProduce, Brand produceBrand, int serialNumber)
        {
            id = ID++;
            this.amountToProduce = amountToProduce;
            this.produceBrand = produceBrand;
            this.serialNumber = serialNumber;
            isWorking = false;
        }

        public void Work()
        {
            Random rnd = new Random();
            productCount = 0;
            isWorking = true;
            Laptop laptop = null;
            while (productCount < amountToProduce)
            {
                if (laptop == null)
                {
                    laptop = new Laptop(produceBrand, serialNumber);
                }
                if (Supervisor.TryProduce(this, laptop))
                {
                    productCount++;
                    Console.WriteLine($"{this} has produced {laptop}.");
                    laptop = null;
                }
                Thread.Sleep(10);
                //Thread.Sleep(rnd.Next(1, 3) * 1000);
            }
            isWorking = false;
            Supervisor.CheckProducerWorkingState();
            Console.WriteLine($"{this} has stopped producing.");
        }

        public override string ToString()
        {
            return $"Producer {id}";
        }
    }
}

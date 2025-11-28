using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_EveryThingPhone
{
    class Producer
    {
        static uint ID;
        uint id;
        int amountToProduce;
        int productCount;

        Brand produceBrand;
        Spec produceSpec;

        bool isWorking;
        public bool IsWorking { get => isWorking; set => isWorking = value; }

        public Producer(int amountToProduce, Brand produceBrand, Spec produceSpec)
        {
            id = ID++;
            this.amountToProduce = amountToProduce;
            this.produceBrand = produceBrand;
            this.produceSpec = produceSpec;
            isWorking = false;
        }

        public void Work()
        {
            Random rnd = new Random();
            isWorking = true;
            productCount = 0;
            Phone phone = null;
            while (productCount < amountToProduce)
            {
                if (phone == null) phone = new Phone(produceBrand, produceSpec);
                if (Supervisor.TryProduce(this, phone))
                {
                    productCount++;
                    Console.WriteLine($"{this} has produced {phone}...");
                    phone = null;

                    if (produceBrand == Brand.X1)
                    {
                        produceSpec = (produceSpec == Spec.PRO) ? Spec.ALAP : Spec.PRO;
                    }
                    Thread.Sleep(rnd.Next(1, 3) * 1000);
                } 
            }
            isWorking = false;
            Console.WriteLine($"{this} has stopped producing...");
            Supervisor.CheckProducerWorkingState();
        }

        public override string ToString()
        {
            return $"Producer {id}";
        }
    }
}

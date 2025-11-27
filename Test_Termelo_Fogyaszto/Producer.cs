using System;

namespace Test_Termelo_Fogyaszto
{
    internal class Producer
    {
        static int ID = 0;
        int id;
        readonly int amountProduce;
        bool isWorking;
        public bool IsWorking
        {
            get => isWorking;
            set => isWorking = value;
        }

        public Producer(int amountToProduce)
        {
            id = ID++;
            amountProduce = amountToProduce;
            isWorking = false;

        }

        public void Work()
        {
            IsWorking = true;

            int productCount = 0;

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
                    Console.WriteLine($"{this} has produced {product}");
                    product = null;
                }
            }

            IsWorking = false;

            Controller.CheckProducerWorkingState();
        }




    }

}

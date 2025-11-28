using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_SomeThingLaptop
{
    class Consumer
    {
        static uint ID;
        uint id;

        public Brand consumeBrand { get; private set; }
        public int currentSerial { get; private set; }

        bool isWorking;
        public bool IsWorking { get => isWorking; set => isWorking = value; }

        public Consumer(Brand consumeBrand, int currentSerial)
        {
            isWorking = false;
            this.consumeBrand = consumeBrand;
            this.currentSerial = currentSerial;
            id = ID++;
        }

        public void Work()
        {
            Laptop? laptop = null;
            List<Laptop>? newBox = null;
            Random rnd = new Random();

            isWorking = true;
            while (Supervisor.laptops.Count > 0 || Supervisor.areProducersRunning)
            {
                if (newBox == null) newBox = new List<Laptop>();
                if (Supervisor.TryConsume(this, out laptop))
                {
                    newBox.Add(laptop);
                }
                if (newBox.Count == Supervisor.stackNumber)
                {
                    Supervisor.boxedLaptops.Add(newBox);
                    Console.WriteLine($"{this} has stacked {laptop.LaptopBrand}{laptop.SerialNumber}s...");
                    newBox = null;
                    if (consumeBrand == Brand.A) 
                        if (currentSerial == 10)
                            currentSerial = 20;
                        else
                            currentSerial = 10;
                }
                if (newBox != null && newBox.Count != 0 && Supervisor.laptops.Count == 0 && !Supervisor.areProducersRunning)
                    Console.WriteLine($"{this} cannot finish the batch... ({newBox.Count} / {Supervisor.stackNumber})");
                laptop = null;
                Thread.Sleep(20);
                //Thread.Sleep(rnd.Next(1500, 2501));
            }

            isWorking = false;
            Supervisor.CheckConsumerWorkingState();
            Console.WriteLine($"{this} has stopped consuming...");
        }

        public override string ToString()
        {
            return $"Consumer {id}";
        }
    }
}

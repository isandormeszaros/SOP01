using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_EveryThingPhone
{
    class Consumer
    {
        static uint ID;
        uint id;

        public Brand consumeBrand { get; private set; }
        public Spec currentSpec { get; private set; }

        bool isWorking;
        public bool IsWorking { get => isWorking; set => isWorking = value; }

        public Consumer(Brand consumeBrand, Spec currentSpec)
        {
            isWorking = false;
            this.consumeBrand = consumeBrand;
            this.currentSpec = currentSpec;
            id = ID++;
        }

        public void Work()
        {
            IsWorking = true;
            Phone phone = null;
            List<Phone> newBox = null;
            Random rnd = new Random();
            while (Supervisor.phones.Count > 0 || Supervisor.areProducersRunning)
            {
                if (newBox == null) newBox = new List<Phone>();
                if (Supervisor.TryConsume(this, out phone))
                {
                    newBox.Add(phone);
                    Console.WriteLine($"{this} has consumed {phone}...");
                }
                if (newBox.Count == Supervisor.stackNumber)
                {
                    Supervisor.boxedPhones.Add(newBox);
                    Console.WriteLine($"{this} has packed a box of {newBox[0].PhoneBrand} {newBox[0].PhoneSpec}");
                    newBox = null;

                    if (consumeBrand == Brand.X1)
                    {
                        currentSpec = (currentSpec == Spec.PRO) ? Spec.ALAP : Spec.PRO;
                    }
                }
                if (newBox != null && newBox.Count != 0 && Supervisor.phones.Count == 0 && !Supervisor.areProducersRunning)
                    Console.WriteLine($"{this} cannot finish batch...\n({newBox.Count} / {Supervisor.stackNumber})");
                phone = null;
                Thread.Sleep(rnd.Next(1000, 1501));
            }
            IsWorking = false;
            Console.WriteLine($"{this} has stopped consuming...");
            Supervisor.CheckConsumerWorkingState();
        }

        public override string ToString()
        {
            return $"Consumer {id}";
        }
    }
}

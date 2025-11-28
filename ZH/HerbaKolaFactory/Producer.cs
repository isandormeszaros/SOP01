using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColaFactory
{
    class Producer
    {
        static uint ID;
        uint id;

        int sleepTime;
        ColaType colaType;
        BottleSize bottleSize;
        public bool isWorking { get; private set; }


        public Producer(ColaType colaType, BottleSize bottleSize, int sleepTime = 1000)
        {
            id = ID++;

            this.colaType = colaType;
            this.bottleSize = bottleSize;
            this.sleepTime = sleepTime;

            isWorking = false;
        }

        // Kidolgozandó
        // Kidolgozandó rész a Producer osztályban
        public void Work()
        {
            isWorking = true;
            Random rnd = new Random();

            // Addig dolgozom, amíg a FOGYASZTÓK igénylik (és a főkapcsoló be van kapcsolva)
            while (Controller.areConsumersRunning)
            {
                // 1. Gyártás
                Cola newCola = new Cola(this.colaType, this.bottleSize);

                // 2. Próbálkozás a berakással (ez blokkolhat, ha tele a buffer)
                bool success = Controller.TryProduce(this, newCola);

                if (!success)
                {
                    // Ha nem sikerült (pl. leálltak a fogyasztók), kilépünk a ciklusból
                    break;
                }

                // 3. Várakozás (1-2 mp)
                // A feladat szerint a termelők 1 vagy 2 másodpercet várnak.
                // A sleepTime paramétert használjuk, vagy véletlent.
                Thread.Sleep(rnd.Next(1000, 2001));
            }

            isWorking = false;
            Controller.CheckProducerWorkingState();
            Console.WriteLine($"{this} has stopped producing...");
        }
        public override string ToString()
        {
            return $"Producer {id} ({colaType}, {Controller.BottleSizeToLiter[bottleSize]}l)";
        }
    }
}

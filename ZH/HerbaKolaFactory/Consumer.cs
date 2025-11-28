using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColaFactory
{
    class Consumer
    {
        static uint ID;
        uint id;

        int sleepTime;

        public ColaType colaType { get; private set; }
        public BottleSize bottleSize { get; private set; }

        public bool isWorking { get; private set; }

        public Consumer(ColaType colaType, BottleSize bottleSize, int sleepTime = 1000)
        {
            isWorking = false;
            this.colaType = colaType;
            this.bottleSize = bottleSize;
            id = ID++;
            this.sleepTime = sleepTime;
        }

        // Kidolgozandó
        // Kidolgozandó rész a Consumer osztályban
        public void Work()
        {
            isWorking = true;
            Random rnd = new Random();
            int packagesCreated = 0;
            int targetPackages = 10; // Feladat: 10 csomag

            List<Cola> currentPackage = new List<Cola>();

            // Addig megyünk, amíg el nem érjük a 10 csomagot
            // VAGY amíg van esély kólát szerezni (termelők futnak vagy van még a pulton)
            while (packagesCreated < targetPackages &&
                   (Controller.areProducersRunning || Controller.colas.Count > 0))
            {
                Cola? cola;
                // Megpróbálunk kivenni EGYET
                if (Controller.TryConsume(this, out cola) && cola != null)
                {
                    // Ha sikerült, betesszük a kezünkben lévő csomagba
                    currentPackage.Add(cola);

                    // Ha összegyűlt a 2 db a csomaghoz
                    if (currentPackage.Count == 2)
                    {
                        // Csomag kész!
                        Controller.stackedColas.Add(new List<Cola>(currentPackage));
                        currentPackage.Clear(); // Ürítjük a kezet
                        packagesCreated++;
                        Console.WriteLine($"\t>>> {this} elkészített egy csomagot! ({packagesCreated}/10)");
                    }

                    // Várakozás (1-2 mp)
                    Thread.Sleep(rnd.Next(1000, 2001));
                }
                else
                {
                    // Ha nem sikerült kivenni (üres a pult, de még várni kell), picit alszunk
                    Thread.Sleep(100);
                }
            }

            isWorking = false;
            Controller.CheckConsumerWorkingState();
            Console.WriteLine($"{this} has stopped consuming... (Total packs: {packagesCreated})");
        }

        public override string ToString()
        {
            return $"Consumer {id} ({colaType}, {Controller.BottleSizeToLiter[bottleSize]}l)";
        }
    }
}

using System;
using System.Threading;

namespace TrainLines
{
    class Controller
    {
        private static readonly object trainOnSwitch = new object();
        private static Random rnd = new Random(); // Kell a véletlenhez

        public static void TrainSwitch(string vonatNev, int menet)
        {
            lock (trainOnSwitch)
            {
                Console.WriteLine($"*** {vonatNev} belépett a váltóra. [Menet {menet}]");

                // JAVÍTÁS: 0.5 - 1.5 másodperc (500 - 1500 ms)
                int delay = rnd.Next(500, 1501);
                Thread.Sleep(delay);

                Console.WriteLine($"*** {vonatNev} elhagyta a váltót ({delay}ms). [Menet {menet}]");
            }
        }
    }
}
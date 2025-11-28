using System;
using System.Diagnostics;
using System.Threading;

namespace TrainLines
{
    class Train
    {
        public string Nev { get; }
        private string Start;
        private string Cel;

        // JAVÍTÁS: Kell a menetidő!
        private int TravelTime; // milliszekundumban

        public Train(string nev, string start, string cel)
        {
            Nev = nev;
            Start = start;
            Cel = cel;

            // JAVÍTÁS: Beállítjuk az időket a célváros alapján
            if (cel == "Fekrecen") TravelTime = 7000;
            else if (cel == "Szöged") TravelTime = 6000;
            else if (cel == "Ugar") TravelTime = 4000; // Ugri vonal -> Ugar
            else TravelTime = 5000; // Alapértelmezett, ha elírták
        }

        public void Travel()
        {
            for (int i = 1; i <= 4; i++)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                Console.WriteLine($"{Nev} elindult {Start}-ból {Cel} felé. [Menet {i}]");

                // --- 1. ÚT A VÁLTÓIG ---
                // A váltó Babarest határában van (1 mp-re).
                // Ha Babarestről indulunk: 1 mp út -> Váltó -> (Maradék) út.
                // Ha Vidékről jövünk: (Teljes - 1) út -> Váltó -> 1 mp út.

                int timeToSwitch;
                int timeAfterSwitch;

                if (Start == "Babarest")
                {
                    timeToSwitch = 1000; // 1 mp a váltóig
                    timeAfterSwitch = TravelTime - 1000; // A maradék
                }
                else
                {
                    timeToSwitch = TravelTime - 1000; // Majdnem végigjön
                    timeAfterSwitch = 1000; // Az utolsó 1 mp Babarest előtt
                }

                Thread.Sleep(timeToSwitch); // Utazás a váltóig

                // --- 2. VÁLTÓ ---
                // Mérjük, mennyit álltunk a váltón!
                long timeBeforeSwitch = sw.ElapsedMilliseconds;
                Controller.TrainSwitch(Nev, i);
                long switchDelay = sw.ElapsedMilliseconds - timeBeforeSwitch; // Ez a váltó okozta késés (+ áthaladás)

                // --- 3. ÚT A VÉGÉIG ---
                Thread.Sleep(timeAfterSwitch); // Utazás a célig

                sw.Stop();
                double teljesIdo = sw.ElapsedMilliseconds / 1000.0;

                // JAVÍTÁS: A késés = Tényleges idő - Elméleti idő (TravelTime)
                // Vagy egyszerűbben: A váltón töltött idő az maga a késés (a feladat szerint "a váltó által okozott késés").
                // De a váltón áthaladás (0.5-1.5) az maga az út része? A feladat azt írja: "Ennek meghibásodása miatt..."
                // Tehát a normál áthaladás 0 lenne? Tegyük fel, hogy a váltón töltött teljes idő a késés.

                double keses = teljesIdo - (TravelTime / 1000.0);

                Console.WriteLine($"{Nev} megérkezett {Cel}-ba. Teljes idő: {teljesIdo:F2} mp, Késés: {keses:F2} mp [Menet {i}]");

                // Fordulás
                string tmp = Start;
                Start = Cel;
                Cel = tmp;

                // Kis pihenő a fordulás előtt (opcionális, a kódban benne volt)
                Thread.Sleep(100);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading; // Fontos a CancellationToken miatt!
using System.Threading.Tasks;

namespace QualifyingSimulation
{
    class QualifyingController
    {
        public List<Driver> drivers { get; private set; }

        public QualifyingController(List<Driver> drivers)
        {
            this.drivers = drivers;
        }

        public async Task StartAsync()
        {
            Console.WriteLine("--- START QUALIFYING (1 minute) ---");

            // 1. Létrehozzuk az időzítőt (Cancellation Source)
            // A "using" miatt automatikusan törlődik a memóriából a végén
            using CancellationTokenSource cts = new CancellationTokenSource();

            // 2. Beállítjuk, hogy 1 perc (60.000 ms) után lőjön le mindent
            cts.CancelAfter(60000);

            // 3. Elindítjuk az összes pilótát
            List<Task> driverTasks = new List<Task>();

            foreach (Driver driver in drivers)
            {
                // Minden pilótának indítunk egy aszinkron feladatot
                // Átadjuk a tokent (cts.Token), hogy tudják, mikor van vége
                driverTasks.Add(RunDriverAsync(driver, cts.Token));
            }

            // 4. Megvárjuk, amíg mindenki befejezi (a leintés utáni utolsó körét is)
            await Task.WhenAll(driverTasks);

            Console.WriteLine("--- QUALIFYING ENDED ---");
        }

        // Kiegészítettem paraméterekkel, mert tudnia kell, ki vezet és mi a jel
        private async Task RunDriverAsync(Driver driver, CancellationToken token)
        {
            Random rnd = new Random(); // Minden szálnak saját random generátor (biztonságosabb)

            // Addig megyünk, amíg a versenyigazgató nem intette le (token)
            while (!token.IsCancellationRequested)
            {
                // Generálunk egy köridőt: 18.000 és 25.000 ms között
                int currentLapTime = rnd.Next(18000, 25001);

                // SZIMULÁCIÓ (Maga a vezetés)
                // Fontos: Itt NEM adjuk át a tokent a Delay-nek!
                // Miért? Mert a feladat azt mondta: "befejezhetik a körüket".
                // Ha átadnánk, a leintés pillanatában azonnal exceptiont dobna és megállna.
                await Task.Delay(currentLapTime);

                // Ha lefutott a Delay, beírjuk az időt
                driver.AddLap(currentLapTime);

                // Kiírjuk az eredményt
                Console.WriteLine($"Driver #{driver.DriverNumber} completed lap: {FormatTime(currentLapTime)}");
            }
        }

        public void PrintResults()
        {
            Console.WriteLine("\n--- FINAL RESULTS ---");

            // Rendezzük a pilótákat a legjobb körük alapján (növekvő sorrend)
            // Azokat vesszük előre, akiknek van érvényes körük (BestLap != null)
            var sortedDrivers = drivers
                .Where(d => d.BestLap.HasValue)
                .OrderBy(d => d.BestLap)
                .ToList();

            int position = 1;
            foreach (var driver in sortedDrivers)
            {
                // Biztonságosan lekérjük az értéket (.Value), mert fent már szűrtük
                Console.WriteLine($"{position}. Driver #{driver.DriverNumber} - Best: {FormatTime(driver.BestLap.Value)}");
                position++;
            }

            // Ha valaki nem ért körbe (bár 1 perc alatt illene), jelezzük
            var noLapDrivers = drivers.Where(d => !d.BestLap.HasValue).ToList();
            if (noLapDrivers.Count > 0)
            {
                Console.WriteLine("No valid time set: " + string.Join(", ", noLapDrivers.Select(d => "#" + d.DriverNumber)));
            }
        }

        // Kiegészítettem bemenő paraméterrel (int ms)
        private static string FormatTime(int milliseconds)
        {
            // TimeSpan segítségével formázzuk: perc:másodperc.ezred
            TimeSpan ts = TimeSpan.FromMilliseconds(milliseconds);
            // "m" = perc, "ss" = kétjegyű másodperc, "fff" = háromjegyű ezred
            return ts.ToString(@"m\:ss\.fff");
        }
    }
}
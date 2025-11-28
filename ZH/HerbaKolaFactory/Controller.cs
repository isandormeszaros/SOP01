using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading; // Fontos a Threading!

namespace ColaFactory
{
    class Controller
    {
        public static readonly Dictionary<BottleSize, string> BottleSizeToLiter = new()
        {
            { BottleSize.QUARTER, "0,33" },
            { BottleSize.HALF, "0,5" },
            { BottleSize.ONE, "1" }
        };

        // LIMIT: Max ennyi kóla lehet a szalagon
        static int maxColasOnLine = 50;

        // CSERE: List-re, hogy ki tudjuk keresni a megfelelőt (Normal/Zero/stb)
        public static List<Cola> colas = new List<Cola>();

        // A kész csomagok gyűjtőhelye (ConcurrentBag szálbiztos)
        public static ConcurrentBag<List<Cola>> stackedColas = new ConcurrentBag<List<Cola>>();

        static List<Producer> producers = new List<Producer>();
        static List<Consumer> consumers = new List<Consumer>();

        public static volatile bool areProducersRunning = false;
        public static volatile bool areConsumersRunning = false;

        // --- TERMELÉS ---
        public static bool TryProduce(Producer producer, Cola cola)
        {
            // ZÁROLJUK a listát (Monitor.Enter)
            lock (colas)
            {
                // VÁRAKOZÁS: Ha tele a pult, addig várunk, amíg nem lesz hely.
                // DE: Ha a fogyasztók már leálltak, akkor mi is kilépünk.
                while (colas.Count >= maxColasOnLine && areConsumersRunning)
                {
                    Monitor.Wait(colas); // Elengedjük a zárat és várunk a jelzésre
                }

                // Ha van hely, betesszük
                if (colas.Count < maxColasOnLine)
                {
                    colas.Add(cola);
                    Console.WriteLine($"[+] {producer} betett egyet. (Pulton: {colas.Count})");

                    // ÉBRESZTŐ: Szólunk a fogyasztóknak, hogy van áru!
                    Monitor.PulseAll(colas);
                    return true;
                }
                return false;
            }
        }

        // --- FOGYASZTÁS ---
        public static bool TryConsume(Consumer consumer, out Cola? cola)
        {
            cola = null;
            lock (colas)
            {
                // KERESÉS: Van-e olyan kóla, ami ennek a fogyasztónak kell?
                // (Típus és Méret egyezzen)
                Cola? matchingCola = colas.FirstOrDefault(c => c.type == consumer.colaType && c.size == consumer.bottleSize);

                // VÁRAKOZÁS: Ha nincs megfelelő kóla, ÉS még dolgoznak a termelők, akkor várunk.
                while (matchingCola == null && areProducersRunning)
                {
                    Monitor.Wait(colas);
                    // Újra keressük, hátha most lett (mert felébredtünk)
                    matchingCola = colas.FirstOrDefault(c => c.type == consumer.colaType && c.size == consumer.bottleSize);
                }

                // Ha találtunk
                if (matchingCola != null)
                {
                    cola = matchingCola;
                    colas.Remove(matchingCola); // Kivesszük a listából
                    Console.WriteLine($"[-] {consumer} kivett egyet. (Pulton: {colas.Count})");

                    // ÉBRESZTŐ: Szólunk a termelőknek, hogy lett hely!
                    Monitor.PulseAll(colas);
                    return true;
                }
                return false;
            }
        }

        // --- SEGÉDMETÓDUSOK (Inicializálás) ---
        public static void CreateConsumers(int amount, ColaType colaType, BottleSize bottleSizeLiter)
        {
            for (int i = 0; i < amount; i++)
            {
                consumers.Add(new Consumer(colaType, bottleSizeLiter));
            }
        }

        public static void CreateProducers(int amount, int sleepTime, ColaType colaType, BottleSize bottleSizeLiter)
        {
            for (int i = 0; i < amount; i++)
            {
                producers.Add(new Producer(colaType, bottleSizeLiter, sleepTime));
            }
        }

        // --- ÁLLAPOT ELLENŐRZÉS ---
        // Ha minden fogyasztó végzett, leállítjuk a flag-et
        public static void CheckConsumerWorkingState()
        {
            // Ha találunk akár egyet is, aki még dolgozik, visszatérünk
            if (consumers.Any(c => c.isWorking)) return;

            // Ha senki nem dolgozik:
            areConsumersRunning = false;
            // Ébresszük fel a termelőket, hogy ők is észrevegyék a leállást!
            lock (colas) { Monitor.PulseAll(colas); }
        }

        public static void CheckProducerWorkingState()
        {
            if (producers.Any(p => p.isWorking)) return;

            areProducersRunning = false;
            lock (colas) { Monitor.PulseAll(colas); }
        }

        // --- INDÍTÁS ---
        public static void StartProcess()
        {
            areProducersRunning = true;
            areConsumersRunning = true;

            // Szálak indítása
            foreach (var p in producers) new Thread(p.Work).Start();
            foreach (var c in consumers) new Thread(c.Work).Start();
        }
    }
}
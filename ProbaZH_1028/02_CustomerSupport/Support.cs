using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_CustomerSupport
{
    class Support
    {
        // Egy szálbiztos gyűjtemény, ami ideális producer-consumer feladathoz.
        // Kezeli a várakozást, ha a sor üres.
        private BlockingCollection<Ticket> ticketQueue = new BlockingCollection<Ticket>(new ConcurrentQueue<Ticket>());

        // A műszak állapotát jelzi (a billentyűfigyelő ciklus futásához)
        private volatile bool supportActive = true;

        // Az agent-ek állapotát jelzi (hogy ne lehessen feleslegesen kapcsolgatni)
        private volatile bool agentsActive = true;

        // Ez az esemény vezérli az agent-eket.
        private ManualResetEvent agentActiveEvent = new ManualResetEvent(true); // Kezdetben aktívak

        // Ezzel állítjuk le a ticket-generátort (producert)
        private CancellationTokenSource producerCts = new CancellationTokenSource();

        private Task producerTask;
        private List<Task> agentTasks = new List<Task>();

        // A szimuláció indítása. Elindítja a producert, agent-eket és a billentyűfigyelőt.
        public void Start()
        {
            Console.WriteLine("Support Központ elindult. 'a' (aktív), 's' (szünet), 'k' (kilépés)");
            supportActive = true;
            agentsActive = true;
            agentActiveEvent.Set(); // Biztosítjuk, hogy "Aktív" állapotban induljanak

            // Producer indítása (Task-on)
            producerTask = Task.Run(() => TicketProducer(producerCts.Token));

            // 2 Agent indítása (Task-on)
            agentTasks.Add(Task.Run(() => AgentWork("Agent 001")));
            agentTasks.Add(Task.Run(() => AgentWork("Agent 002")));

            // Billentyűfigyelő indítása a fő szálon
            InputLoop();

            // A program itt vár, amíg minden befejeződik
            Console.WriteLine("Várakozás a producer és az agent-ek leállására...");
            try
            {
                // Megvárjuk, amíg a producer leáll (a 'k' lenyomása után)
                Task.WaitAll(producerTask);
                // Megvárjuk, amíg az agent-ek feldolgozzák a maradék ticketet és leállnak
                Task.WaitAll(agentTasks.ToArray());
            }
            catch (AggregateException ex)
            {
                // Kezeli az esetleges Task-megszakadási hibákat
                Console.WriteLine($"Task hiba történt: {ex.InnerException.Message}");
            }

            Console.WriteLine("Minden agent végzett. A központ bezár.");
        }

        // PRODUCER: Folyamatosan generálja a ticketeket, amíg a CancellationToken nem jelez.
        private async void TicketProducer(CancellationToken token)
        {
            int ticketId = 0;
            Random rnd = new Random();
            Console.WriteLine("[PRODUCER] Ticket-generátor elindult.");

            try
            {
                while (true)
                {
                    // A CancellationToken.ThrowIfCancellationRequested() elegánsabb,
                    // mert a Task.Delay és a .Add is dobhat OperationCanceledException-t.
                    token.ThrowIfCancellationRequested();

                    var ticket = new Ticket
                    {
                        Id = ++ticketId,
                        Priority = rnd.Next(1, 4)
                    };

                    ticketQueue.Add(ticket, token);
                    Console.WriteLine($"[PRODUCER] Új ticket érkezett: {ticket}");

                    await Task.Delay(rnd.Next(300, 1000), token);
                }
            }
            catch (OperationCanceledException)
            {
                // Ez a normál leállási útvonal, ha a 'k' billentyűt lenyomják.
            }
            finally
            {
                // KRITIKUS: Jelezzük az agent-eknek (consumereknek), hogy több ticket már nem fog érkezni.
                ticketQueue.CompleteAdding();
                Console.WriteLine("[PRODUCER] Ticket-generátor leállt.");
            }
        }

        // CONSUMER: Az agent-ek munkáját szimuláló metódus.
        private void AgentWork(string agentName)
        {
            Random rnd = new Random();
            Console.WriteLine($"\t[{agentName}] Munkába állt.");

            try
            {
                // Végtelen ciklus, ami csak akkor szakad meg, ha a ticketSor "Completed" és üres.
                while (true)
                {
                    // 1. VÁRAKOZÁS AZ ÁLLAPOTRA
                    // Ez a ManualResetEvent kapuja. Ha 'Reset' (zárt) állapotban van, a szál itt várakozik ("Szünet"-en van).
                    // Ezt a ticket elvétele előtt kell ellenőrizni.
                    agentActiveEvent.WaitOne();

                    Ticket ticket;
                    try
                    {
                        // 2. TICKET ELVÉTELE
                        // A Take() blokkolja a szálat, amíg nincs új elem VAGY dob egy InvalidOperationException-t, ha a sort lezárták (CompleteAdding) ÉS már üres.
                        ticket = ticketQueue.Take();
                    }
                    catch (InvalidOperationException)
                    {
                        // Ez a normál leállási jelzés. Nincs több ticket.
                        break;
                    }

                    // 3. FELDOLGOZÁS
                    // Megvan a ticket, feldolgozzuk.
                    Console.WriteLine($"\t[{agentName}] Feladat felvéve: {ticket}");
                    Task.Delay(rnd.Next(1000, 3000)).Wait(); // Szimulált munka
                    Console.WriteLine($"\t\t[{agentName}] Feladat kész: {ticket}");

                    // A ciklus újraindul. Ha közben lenyomták az 's'-t, az agentActiveEvent.WaitOne() meg fogja fogni.
                }
            }
            catch (Exception ex)
            {
                // Váratlan hiba
                Console.WriteLine($"HIBA az agentben ({agentName}): {ex.Message}");
            }

            Console.WriteLine($"\t[{agentName}] Műszak vége, agent leáll.");
        }

        // A billentyűleütéseket figyelő ciklus.
        private void InputLoop()
        {
            while (supportActive)
            {
                // Az ReadKey(true) nem írja ki a konzolra a lenyomott billentyűt.
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.KeyChar)
                {
                    case 'a':
                        Activate();
                        break;
                    case 's':
                        Pause();
                        break;
                    case 'k':
                        Exit();
                        break;
                }
            }
        }

        public void Activate()
        {
            if (agentsActive)
            {
                Console.WriteLine(">> Agentek már aktívak.");
                return;
            }

            Console.WriteLine(">> AGENTEK AKTIVÁLVA (a)");
            agentsActive = true;
            agentActiveEvent.Set(); // Kinyitja az MRE kaput, a várakozó agent-ek továbbmennek.
        }

        public void Pause()
        {
            if (!agentsActive)
            {
                Console.WriteLine(">> Agentek már szüneten vannak.");
                return;
            }

            Console.WriteLine(">> AGENTEK SZÜNETRE KÜLDVE (s)");
            agentsActive = false;
            agentActiveEvent.Reset(); // Bezárja az MRE kaput.
        }

        public void Exit()
        {
            Console.WriteLine(">> BEZÁRÁS KEZDEMÉNYEZVE (k)");
            supportActive = false; // Leállítja az InputLoop-ot

            // 1. Leállítja a ticket-generátort (producer-t)
            // Ez fogja aktiválni a CancellationToken-t.
            producerCts.Cancel();

            // 2. Felébreszti az agent-eket
            // Ez KRITIKUS! Ha "Szünet" állapotban (WaitOne) várakoznak, fel kell őket ébreszteni (Set), hogy észrevegyék,
            // hogy a ticketSor lezárult (CompleteAdding) és leállhassanak.
            Console.WriteLine(">> Agentek ébresztése a maradék ticketek feldolgozásához...");
            agentActiveEvent.Set();
        }
    }
}

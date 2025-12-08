using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server
{
    internal class Controll
    {
        public static List<Controll> controlls = new List<Controll>();
        public static void KillAllControlls()
        {
            foreach (var client in new List<Controll>(Controll.controlls))
            {
                client.Close();
            }
        }

        private static Random rnd = new Random();
        StreamReader reader;
        StreamWriter writer;
        TcpClient client;

        // Kidolgozandó
        Controller currentController = null;

        public Controll(TcpClient client)
        {
            this.client = client;

            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream()) { AutoFlush = true };

            controlls.Add(this);

            ThreadPool.QueueUserWorkItem(_ => FetchCommands());

            Console.WriteLine("Új kliens csatlakozott");
        }

        private void FetchCommands()
        {
            // Kidolgozandó
            try
            {
                while (true)
                {
                    string command = reader.ReadLine();
                    Console.WriteLine($"Parancs érkezett: {command}");
                    ProcessCommand(command);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                Close();
                Console.WriteLine("Hiba a kliens kapcsolatában... :((");
            }
        }

        private void ProcessCommand(string command)
        {
            // Kidolgozandó
            string[] processedData = command.Split(' ');
            processedData[0] = processedData[0].ToUpper();

            switch (processedData[0])
            {
                case "EXIT":
                    Close(); // Kilépés
                    break;
                case "ENABLE":
                    Enable(processedData); // Bejelentkezés
                    break;
                case "DISABLE":
                    Disable(processedData); // Kijelentkezés
                    break;
                case "PROBES":
                    Probes(processedData); // Összes szonda listázása
                    break;
                case "STATUS":
                    Status(processedData); // Egy adott szonda állapota
                    break;
                case "CREATE":
                    Create(processedData);
                    break;
                case "REFILL":
                    Refill(processedData);
                    break;
                case "UP":
                    Up(processedData); // Mozgás fel
                    break;
                case "DOWN":
                    Down(processedData); // Mozgás le
                    break;
                case "LEFT":
                    Left(processedData); // Mozgás balra
                    break;
                case "RIGHT":
                    Right(processedData); // Mozgás jobbra
                    break;
                default:
                    SendInformationToClient($"Ismeretlen parancs: {processedData[0]}");
                    break;
            }
        }

        // ENABLE parancs: Bejelentkezés (Felhasználónév + Jelszó)
        void Enable(string[] data)
        {
            if (data.Length != 3)
            {
                SendInformationToClient("Hibás paraméterszám!");
                return;
            }
            if (currentController != null)
            {
                SendInformationToClient("Már be vagy jelentkezve");
                return;
            }
            currentController = Controller.TryEnable(data[1], data[2]);
            if (currentController == null)
            {
                SendInformationToClient("Hibás bejelentkezési adatok!");
            }
            else
            {
                SendInformationToClient($"Űrkutató {currentController.Id} sikeresen bejelentkezett!");
            }
        }

        // DISABLE parancs: Kijelentkezés
        void Disable(string[] notNeededData)
        {
            if (notNeededData.Length != 1)
            {
                SendInformationToClient("Nem engedélyezett paraméterek...");
                return;
            }
            if (currentController != null)
            {
                SendInformationToClient("Űrkutató kijelentkezett...");
                currentController = null;
            }
            else
            {
                SendInformationToClient("Nincs bejelentkezve...");
            }
        }

        // PROBES parancs: Listázza az összes szondát, ha van energiájuk
        void Probes(string[] notNeededData)
        {
            if (notNeededData.Length != 1)
            {
                SendInformationToClient("Nem engedélyezett paraméterek...");
                return;
            }
            if (currentController == null)
            {
                SendInformationToClient("Nincs bejelentkezve...");
                return;
            }

            foreach (var p in Probe.probes)
            {
                if (p.Energy > 0)
                {
                    SendInformationToClient($"Űrszonda {p.Id}\tX:{p.X}\tY:{p.Y}");
                    p.Consume(1);
                }
            }
        }

        // STATUS parancs: Saját vagy egy konkrét szonda adatait kéri le
        void Status(string[] data)
        {
            if (data.Length != 1 && data.Length != 2)
            {
                SendInformationToClient("Hibás paraméterszám...");
                return;
            }
            if (currentController == null)
            {
                SendInformationToClient("Nincs bejelentkezve...");
                return;
            }

            Probe probe;
            if (data.Length == 1)
            {
                probe = Probe.probes.Find(p => p.Cid == currentController.Id);

                if (probe.Energy - 2 < 0)
                {
                    SendInformationToClient("Űrszonda nem tud státuszt jelenteni, mert alacsony az energiaszintje...");
                    return;
                }
                SendInformationToClient($"Űrszonda {probe.Id}: X: {probe.X}\tY: {probe.Y}");
                probe.Consume(2);
                return;
            }

            probe = Probe.probes.Find(p => p.Id == data[1]);
            if (probe == null)
            {
                SendInformationToClient("Nincs ilyen űrszonda...");
                return;
            }

            if (probe.Energy - 2 < 0)
            {
                SendInformationToClient("Űrszonda nem tud státuszt jelenteni, mert alacsony az energiaszintje...");
                return;
            }
            SendInformationToClient($"Űrszonda {probe.Id}: X: {probe.X}\tY: {probe.Y}");
            probe.Consume(2);
        }

        // UP parancs: Mozgás felfelé
        void Up(string[] notNeededData)
        {
            if (notNeededData.Length != 1)
            {
                SendInformationToClient("Nem engedélyezett paraméterek...");
                return;
            }
            if (currentController == null)
            {
                SendInformationToClient("Nincs bejelentkezve...");
                return;
            }
            Probe currentProbe = Probe.probes.Find(p => p.Cid == currentController.Id);

            if (currentProbe.Energy - 1 < 0)
            {
                SendInformationToClient("Űrszonda nem tud mozogni...");
                return;
            }
            Probe potentialCrashProbe = Probe.probes.Find(p => p.X == currentProbe.X && p.Y == currentProbe.Y + 1);
            currentProbe.Y++; 

            if (potentialCrashProbe != null)
            {
                currentProbe.Energy -= 10;
                potentialCrashProbe.Energy -= 10;
                SendInformationToClient("///ERROR: Ütközés");
                Disable(notNeededData);
                currentController = null;
                return;
            }
            SendInformationToClient($"Űrszonda új pozíciója \t X: {currentProbe.X} Y: {currentProbe.Y}");
        }

        // DOWN parancs: Mozgás lefelé (logika ugyanaz, mint UP-nál, csak Y csökken)
        void Down(string[] notNeededData)
        {
            if (notNeededData.Length != 1)
            {
                SendInformationToClient("Nem engedélyezett paraméterek...");
                return;
            }
            if (currentController == null)
            {
                SendInformationToClient("Nincs bejelentkezve...");
                return;
            }
            Probe currentProbe = Probe.probes.Find(p => p.Cid == currentController.Id);
            if (currentProbe.Energy - 1 < 0)
            {
                SendInformationToClient("Űrszonda nem tud mozogni...");
                return;
            }
            Probe potentialCrashProbe = Probe.probes.Find(p => p.X == currentProbe.X && p.Y == currentProbe.Y - 1);
            currentProbe.Y--;
            if (potentialCrashProbe != null)
            {
                currentProbe.Energy -= 10;
                potentialCrashProbe.Energy -= 10;
                SendInformationToClient("///ERROR: Ütközés");
                Disable(notNeededData);
                return;
            }
            SendInformationToClient($"Űrszonda új pozíciója \t X: {currentProbe.X} Y: {currentProbe.Y}");
        }

        // LEFT parancs: Mozgás balra (X koordináta csökken)
        void Left(string[] notNeededData)
        {
            if (notNeededData.Length != 1)
            {
                SendInformationToClient("Nem engedélyezett paraméterek...");
                return;
            }
            if (currentController == null)
            {
                SendInformationToClient("Nincs bejelentkezve...");
                return;
            }
            Probe currentProbe = Probe.probes.Find(p => p.Cid == currentController.Id);
            if (currentProbe.Energy - 1 < 0)
            {
                SendInformationToClient("Űrszonda nem tud mozogni...");
                return;
            }
            Probe potentialCrashProbe = Probe.probes.Find(p => p.X == currentProbe.X - 1 && p.Y == currentProbe.Y);
            currentProbe.X--;
            if (potentialCrashProbe != null)
            {
                currentProbe.Energy -= 10;
                potentialCrashProbe.energy -= 10;
                SendInformationToClient("///ERROR: _U~tk_ozs.///");
                Disable(notNeededData);
                return;
            }
            SendInformationToClient($"Űrszonda új pozíciója \t X: {currentProbe.X} Y: {currentProbe.Y}");
        }

        // RIGHT parancs: Mozgás jobbra (X koordináta nő)
        void Right(string[] notNeededData)
        {
            if (notNeededData.Length != 1)
            {
                SendInformationToClient("Nem engedélyezett paraméterek...");
                return;
            }
            if (currentController == null)
            {
                SendInformationToClient("Nincs bejelentkezve...");
                return;
            }
            Probe currentProbe = Probe.probes.Find(p => p.Cid == currentController.Id);
            if (currentProbe.Energy - 1 < 0)
            {
                SendInformationToClient("Űrszonda nem tud mozogni...");
                return;
            }
            Probe potentialCrashProbe = Probe.probes.Find(p => p.X == currentProbe.X + 1 && p.Y == currentProbe.Y);
            currentProbe.X++;
            if (potentialCrashProbe != null)
            {
                currentProbe.Energy -= 10;
                potentialCrashProbe.energy -= 10;
                SendInformationToClient("///ERROR: _U~tk_ozs.///");
                Disable(notNeededData);
                return;
            }
            SendInformationToClient($"Űrszonda új pozíciója \t X: {currentProbe.X} Y: {currentProbe.Y}");
        }

        void Create(string[] data)
        {
            if (data.Length != 1)
            {
                SendInformationToClient("Nem engedélyezett paraméterek...");
                return;
            }

            string generatedId;
            Controller.TryCreate("defaultPass", out generatedId);

            currentController = Controller.controllers.Find(c => c.Id == generatedId);

            if (currentController == null)
            {
                SendInformationToClient("Nem sikerült létrehozni a kontrollert!");
                return;
            }

            SendInformationToClient($"Új kontroller létrehozva! ID: {currentController.Id}");

            SendInformationToClient($"Sikeres belépés az új kontrollerrel: {currentController.Id}");
        }


        void Refill(string[] data)
        {
            if (data.Length != 1)
            {
                SendInformationToClient("Nem engedélyezett paraméterek...");
                return;
            }

            if (currentController == null)
            {
                SendInformationToClient("Nincs bejelentkezve...");
                return;
            }

            // Saját szonda kikeresése
            Probe currentProbe = Probe.probes.Find(p => p.Cid == currentController.Id);

            if (currentProbe == null)
            {
                SendInformationToClient("Nincs szonda a kontrollerhez kötve...");
                return;
            }

            // Random töltés 20–70 között
            int addEnergy = rnd.Next(20, 71);

            currentProbe.Energy = currentProbe.Energy + addEnergy;

            SendInformationToClient($"Energia feltöltve: +{addEnergy}. Jelenlegi energia: {currentProbe.Energy}");
        }



        private void SendInformationToClient(string msg)
        {
            // Kidolgozandó
            writer.WriteLine(msg);
        }

        public void Close()
        {
            // Kidolgozandó
            reader.Close();
            writer.Close();
            client.Close();
            controlls.Remove(this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server
{
    internal class Probe
    {
        string id;
        int x;
        int y;
        string cid;
        public int energy;
        public int Energy
        {
            get => energy;
            set
            {
                if (value > 100)
                    energy = 100;
                else if (value < 0)
                    energy = 0;
                else
                    energy = value;
            }
        }


        public int X { 
            get 
            {
                return x;
            }
            set
            {
                x = value;
            } 
        }
        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public string Cid { 
            get 
            {
                return cid;
            }
            set
            {
                cid = value;
            }
        }

        public static List<Probe> probes = new List<Probe>();

        // Segítség: Ha fájlból olvasunk be, akkor nem szabad ID-t generálni! (És vakon sem szabad létrehozni egyet.)
        private Probe(string id, int x, int y, int energy, string cid)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            this.energy = energy;
            this.cid = cid;
        }

        public Probe()
        {
            this.id = GenerateId();
            this.x = 0;
            this.y = 0;
            this.cid = "C-" + this.id.Split('-')[1];
            this.energy = 100;

            probes.Add(this);
        }

        string GenerateId()
        {
            Random rnd = new Random();
            string generatedId;

            do
            {
                // P-xxxxx-F vagy S
                string number = rnd.Next(10000, 100000).ToString();
                string type = rnd.Next(0, 2) == 0 ? "F" : "S";
                generatedId = $"P-{number}-{type}";
            }
            while (probes.Any(p => p.id == generatedId));

            return generatedId;
        }

        public static void LoadProbes(string fileName)
        {
            probes.Clear();

            if (!File.Exists(fileName))
            {
                Console.WriteLine("Űrszonda fájl nem található...");
                return;
            }

            XDocument xml = XDocument.Load(fileName);

            foreach (XElement probe in xml.Descendants("probe"))
            {
                Probe p = new Probe(
                    (string)probe.Attribute("id"),
                    (int)probe.Attribute("X"),
                    (int)probe.Attribute("Y"),
                    (int)probe.Attribute("energy"),
                    (string)probe.Attribute("sid")
                );

                probes.Add(p);
            }

            Console.WriteLine("Űrszondák sikeresen beolvasva.");
        }

        public static void SaveProbes(string fileName)
        {
            XElement root = new XElement("probes");

            foreach (var p in probes)
            {
                root.Add(
                    new XElement("probe",
                        new XAttribute("id", p.Id),
                        new XAttribute("X", p.X),
                        new XAttribute("Y", p.Y),
                        new XAttribute("energy", p.Energy),
                        new XAttribute("sid", p.Cid)
                    )
                );
            }

            XDocument xml = new XDocument(root);
            xml.Save(fileName);

            Console.WriteLine("Űrszondák sikeresen elmentve.");
        }

        public void Consume(int amount)
        {
            int cost = Id.EndsWith("F") ? amount * 2 : amount;
            Energy = Energy - cost;
        }
    }
}

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server
{
    internal class Controller
    {
        string id;
        string passcode;
        string pId;
        public static List<Controller> controllers = new List<Controller>();

        // Kidolgozandó
        private Controller(string id, string passcode, string pId)
        {
            this.id = id;
            this.passcode = passcode;
            this.pId = pId;
        }

        private Controller(string passcode)
        {
            Probe probe = new Probe(); 
            this.id = probe.Cid;       
            this.passcode = passcode;
            this.pId = probe.Id;      
        }

        public string Id { get => id; set => id = value; }
        public string Passcode { get => passcode; set => passcode = value; }
        public string PId { get => pId; set => pId = value; }

        public static Controller TryEnable(string id, string passcode)
        {
            // Kidolgozandó
            return controllers.Find(c => c.Id == id && c.Passcode == passcode);
        }
        public static bool TryCreate(string passcode, out string id)
        {
            Controller c = new Controller(passcode);
            controllers.Add(c);
            id = c.id;
            return true;
        }

        public static void LoadControllers(string fileName)
        {
            // Kidolgozandó
            controllers.Clear();

            if (!File.Exists(fileName))
            {
                Console.WriteLine("Kontroller fájl nem található...");
                return;
            }

            XDocument xml = XDocument.Load(fileName);

            foreach (XElement ctrl in xml.Descendants("controller"))
            {
                Controller c = new Controller(
                    (string)ctrl.Attribute("id"),
                    (string)ctrl.Attribute("passcode"),
                    (string)ctrl.Attribute("pid")
                );
                controllers.Add(c);
            }

            Console.WriteLine("Kontrollerek sikeresen beolvasva.");
        }

        public static void SaveControllers(string fileName)
        {
            // Kidolgozandó
            XElement root = new XElement("controllers");

            foreach (var c in controllers)
            {
                root.Add(
                    new XElement("controller",
                        new XAttribute("id", c.Id),
                        new XAttribute("passcode", c.Passcode),
                        new XAttribute("pid", c.PId)
                    )
                );
            }

            XDocument xml = new XDocument(root);
            xml.Save(fileName);

            Console.WriteLine("Kontrollerek sikeresen elmentve.");
        }
    }
}

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
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

        private Controller(string id, string passcode, string pId)
        {
            this.id = id;
            this.passcode = passcode;
            this.pId = pId;
        }

        // Kidolgozandó
        private Controller()
        {

        }

        public string Id { get => id; set => id = value; }
        public string Passcode { get => passcode; set => passcode = value; }
        public string PId { get => pId; set => pId = value; }

        public static Controller TryEnable(string loginId, string loginPass)
        {

            // Kidolgozandó
            return controllers.FirstOrDefault(c => c.Id == loginId && c.Passcode == loginPass);

        }
        public static bool TryCreate()
        {
            // Kidolgozandó
            return false;
        }

        public static void LoadControllers(string fileName)
        {
            // Kidolgozandó
            // Controller(string id, string passcode, string pId)
            // < controller id = "C-156459" passcode = "pass3" pid = "P-156459-S" />
            controllers.Clear();
            if (File.Exists(fileName))
            {
                XDocument xml = XDocument.Load(fileName);

                foreach (XElement controller in xml.Descendants("controller"))
                {
                    Controller c = new Controller(
                        (string)controller.Attribute("id"),
                        (string)controller.Attribute("passcode"),
                        (string)controller.Attribute("pid")
                    );
                    controllers.Add(c);
                }

                Console.WriteLine("Kontrollerek sikeresen betöltve");
            }
            else Console.WriteLine("Nem található Kontroller!");


        }

        public static void SaveControllers(string fileName)
        {
            // Kidolgozandó
            XElement root = new XElement("controllers");
            foreach (var c in controllers)
            {
                root.Add(
                new XElement("controller",
                new XAttribute((XName)"id", c.Id),
                new XAttribute((XName)"passcode", c.Passcode),
                new XAttribute((XName)"pid", c.PId))    
                );
            }
            XDocument xml = new XDocument(root);
            xml.Save(fileName);
            Console.WriteLine("Kontroller sikeresen elmentve");
        }
    }
}

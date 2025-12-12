using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
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
        int energy;
        public int Energy
        {
            get
            {
                return energy;
            }
            set
            {
                if (energy + value > 100)
                {
                    energy = 100;
                }
                else if (energy + value < 0)
                {
                    energy = 0;
                }
                else energy = value;
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
        public Probe(string id, int x, int y, int energy, string cid)
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
        }

        string GenerateId()
        {
            return "";
        }

        public static void LoadProbes(string fileName)
        {
    
        }

        public static void SaveProbes(string fileName)
        {
       
       
        }
    }
}

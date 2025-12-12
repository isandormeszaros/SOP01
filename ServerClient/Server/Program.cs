using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Server
{
    internal class Program
    {
        // Kidolgozandó
        static bool listen = true;
        static TcpListener server;

        private const string controllersFile = "Controllers.xml";
        private const string probesFile = "Probes.xml";

        static void Main(string[] args)
        {

            // Kidolgozandó
            Controller.LoadControllers(controllersFile);
            Probe.LoadProbes(probesFile);

            IPAddress ip = IPAddress.Parse(ConfigurationManager.AppSettings["ip"].ToString());
            int port = int.Parse(ConfigurationManager.AppSettings["port"].ToString());

            server = new TcpListener(ip, port);
            server.Start();

            new Thread(WaitForClients).Start();

            Console.WriteLine("Press enter to shut down the server...");
            Console.ReadLine();

            // Kidolgozandó
            listen = false;
            Controll.KillAllControlls();
            server.Stop();


            //mentés
            Controller.SaveControllers(controllersFile);
            Probe.SaveProbes(probesFile);

        }

        static void WaitForClients()
        {
            while (listen)
            {
                if (server.Pending())
                {
                    // Kidolgozandó
                    TcpClient client = server.AcceptTcpClient();
                    new Controll(client);
                }
            }
        }
    }
}

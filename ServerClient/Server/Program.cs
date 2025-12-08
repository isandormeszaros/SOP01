using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

            Console.WriteLine($"Loaded {Controller.controllers.Count} controllers and {Probe.probes.Count} probes.");

            IPAddress ip = IPAddress.Parse(ConfigurationManager.AppSettings["ip"]);
            Console.WriteLine("IP raw: " + ConfigurationManager.AppSettings["ip"]);

            int port = int.Parse(ConfigurationManager.AppSettings["port"]);

            Console.WriteLine($"Starting server on {ip}:{port} ...");

            server = new TcpListener(ip, port);
            server.Start();

            new Thread(WaitForClients).Start();

            Console.WriteLine("Server is running. Press ENTER to shut down...");

            Console.ReadLine();

            listen = false;
            Controll.KillAllControlls(); 
            server.Stop();

            Controller.SaveControllers(controllersFile);
            Probe.SaveProbes(probesFile);

            Console.WriteLine("Server stopped.");

        }

        static void WaitForClients()
        {
            while (listen)
            {
                if (server.Pending())
                {
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Client connected.");

                    new Controll(client);
                }
                else
                {
                    Thread.Sleep(20);
                }
            }
        }
    }
}

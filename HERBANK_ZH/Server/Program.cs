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
        // Szerver állapota és csatorna definiálása
        static bool listen = true;
        static TcpListener server;

        private const string accountsFile = "";
        private const string debitCardsFile = "";

        static void Main(string[] args)
        {
            // Szerver indítási folyamatok

            Console.WriteLine("Press enter to shut down the server...");
            Console.ReadLine();

            // Leállítási folyamatok

        }

        // Segítség: várakozás kliens kapcsolatokra, külön szálon
        static void WaitForClients()
        {
            while (listen)
            {
                // Kiegészítendő
            }
        }
    }
}
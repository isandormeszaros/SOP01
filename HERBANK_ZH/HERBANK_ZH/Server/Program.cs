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

        private const string accountsFile = "accounts.xml";
        private const string debitCardsFile = "debitcards.xml";

        static void Main(string[] args)
        {
            // Szerver indítási folyamatok

            Account.LoadAccounts(accountsFile);
            DebitCard.LoadDebitCards(debitCardsFile);

            IPAddress ip = IPAddress.Parse(ConfigurationManager.AppSettings["ip"].ToString());
            int port = int.Parse(ConfigurationManager.AppSettings["port"].ToString());

            server = new TcpListener(ip, port);
            server.Start();

            new Thread(WaitForClients).Start();

            Console.WriteLine("Press enter to shut down the server...");
            Console.ReadLine();

            // Leállítási folyamatok
            listen = false;
            BankClient.KillAllClients();
            server.Stop();

            Account.SaveAccounts(accountsFile);
            DebitCard.SaveDebitCards(debitCardsFile);

        }

        // Segítség: várakozás kliens kapcsolatokra, külön szálon
        static void WaitForClients()
        {
            while (listen)
            {
                // Kiegészítendő
                if (server.Pending())
                {
                    TcpClient client = server.AcceptTcpClient();
                    new BankClient(client);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    internal class BankClient
    {

        static List<BankClient> clients = new List<BankClient>();

        // Kiegészítendő
        public static void KillAllClients()
        {
            foreach (var client in new List<BankClient>(clients))
            {
                client.Close();
            }
        }

        StreamReader reader;
        StreamWriter writer;
        TcpClient client;
        Account currentController = null;

        // Kiegészítendő
        public BankClient()
        {


            Console.WriteLine("Új kliens csatlakozott...");
        }

        private void FetchCommands()
        {
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
                Console.WriteLine("Hiba a kliens kapcsolatában...");
            }
        }

        
        private void ProcessCommand(string command)
        {
            // Kiegészítendő
        }

        // Segítség: segédfüggvények a parancsok feldolgozásához

        // Kiegészítendő
        private void SendInformationToClient()
        {

        }

        // Kiegészítendő
        public void Close()
        {

        }
    }
}

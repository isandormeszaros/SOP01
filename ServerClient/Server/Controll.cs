using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

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

        // Kidolgozandó
        StreamReader reader;
        StreamWriter writer;
        TcpClient client;


        Controller currentController = null;

        public Controll(TcpClient client)
        {
            // Kidolgozandó
            this.client = client;

            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());

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
                    ProcessCommand( command );
                }
            }
            catch (Exception)
            {

                
            }
            finally 
            { 
                Close();
                Console.WriteLine("Hiba a kliens kapcsolat közben...");
            }
       
        }   

        private void ProcessCommand(string command)
        {
            // Kidolgozandó
            string[] processData = command.Split(' ');
            processData[0] = processData[0].ToUpper();

            switch (processData[0])
            {
                default:
                    Close();
                    SendInformationToClient("Ismeretlen parancsot adott meg a User");
                    break;
                case "LOGIN":
                    Login(processData);
                    break;
            }


        }

        void Login(string[] data)
        {
            if (data.Length != 3)
            {
                SendInformationToClient("Hibás paraméterszám");
                return;
            }

            if (currentController != null) 
            {
                SendInformationToClient("Hiba, már be van jelentkezve");
                return;
            }
            Controller talalat = Controller.TryEnable(data[1], data[2]);
            if (talalat != null)
            {
                currentController = talalat;
                SendInformationToClient("Sikeres bejelentkezés");
            }
            else
            {
                SendInformationToClient("Hiba történt, próbálja meg újra!");
            }
        }

        private void SendInformationToClient(string msg)
        {
            // Kidolgozandó
            writer.WriteLine(msg);
            writer.Flush();
        }

        public void Close()
        {
            // Kidolgozandó
            reader.Close();
            writer.Close();
            client.Close();

        }
    }
}



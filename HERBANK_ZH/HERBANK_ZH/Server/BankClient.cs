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


        Account currentController = null;
        StreamReader reader;
        StreamWriter writer;
        TcpClient client;

        // Kiegészítendő
        public BankClient(TcpClient client)
        {
            this.client = client;

            writer = new StreamWriter(client.GetStream());
            reader = new StreamReader(client.GetStream());

            ThreadPool.QueueUserWorkItem(_ => FetchCommands());

            clients.Add(this);


            Console.WriteLine("Új kliens csatlakozott...");
        }

        private void FetchCommands()
        {
            // Kiegészítendő
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
                Console.WriteLine("Hiba a kliens kapcsolata közben!");
            }
        }

        
        private void ProcessCommand(string command)
        {
            string[] processedData = command.Split(' ');
            processedData[0] = processedData[0].ToUpper();

            switch (processedData[0])
            {
                case "ACCESS":
                    Login(processedData);
                    break;

                case "LOGOUT":
                    Logout();
                    break;

                case "DETAILS":
                    Details();
                    break;

                case "TRANSACTION":
                    Transaction(processedData);
                    break;
                case "TRANSFER":
                    Transfer(processedData);
                    break;

                case "QUIT":
                    SendInformationToClient("Viszlát!");
                    Close(); 
                    break;
                default:
                    break;
            }
        }

        // Segítség: segédfüggvények a parancsok feldolgozásához
        void Login(string[] data)
        {
            if (data.Length != 3)
            {
                SendInformationToClient("Hibás paraméterszám");
                return;
            }
            if (currentController != null)
            {
                SendInformationToClient("Már be van jelentkezve!");
                return;
            }
            Account talalat = Account.AccountExists(data[1], int.Parse(data[1]));

            if (talalat!= null) 
            {
                currentController = talalat;
                SendInformationToClient($"SIKER: Üdvözöljük, {talalat.AccountHolder}!");
            }
            else 
            {
                SendInformationToClient("HIBA: Hibás kártyaszám vagy PIN kód, vagy a kártya nem létezik.");
            }
        }

        void Logout()
        {
            if (currentController == null)
            {
                SendInformationToClient("HIBA: Nincs bejelentkezve senki.");
                return;
            }

            currentController = null;
            SendInformationToClient("SIKER: Sikeres kijelentkezés.");
        }

        void Details()
        {
            if (currentController == null)
            {
                SendInformationToClient("HIBA: Ehhez a parancshoz be kell jelentkezni!");
                return;
            }

            string info = $"Tulajdonos: {currentController.AccountHolder}, " +
                          $"Számlaszám: {currentController.AccountNumber}, " +
                          $"Egyenleg: {currentController.Balance} Ft, " +
                          $"Típus: {currentController.AccountType}";

            SendInformationToClient(info);
        }

        void Transaction(string[] data)
        {
            if (data.Length != 3)
            {
                SendInformationToClient("HIBA: Hibás paraméterek! Használat: TRANSACTION <DEPOSIT/WITHDRAW> <összeg>");
                return;
            }

            if (currentController == null)
            {
                SendInformationToClient("HIBA: Be kell jelentkezni!");
                return;
            }

            string type = data[1].ToUpper();
            if (!int.TryParse(data[2], out int amount) || amount <= 0)
            {
                SendInformationToClient("HIBA: Az összegnek pozitív számnak kell lennie!");
                return;
            }

            if (type == "DEPOSIT")
            {
                currentController.Balance += amount;
                SendInformationToClient($"SIKER: Befizetés sikeres! Új egyenleg: {currentController.Balance} Ft");
            }
            else if (type == "WITHDRAW")
            {
                if (currentController.Balance >= amount)
                {
                    currentController.Balance -= amount;
                    SendInformationToClient($"SIKER: Pénzfelvétel sikeres! Új egyenleg: {currentController.Balance} Ft");
                }
                else
                {
                    SendInformationToClient("HIBA: Nincs fedezet a tranzakcióhoz!");
                }
            }
            else
            {
                SendInformationToClient("HIBA: Ismeretlen tranzakció típus (csak DEPOSIT vagy WITHDRAW)!");
            }
        }

        void Transfer(string[] data)
        {
            if (data.Length != 3)
            {
                SendInformationToClient("HIBA: Hibás paraméterek! Használat: TRANSFER <számlaszám> <összeg>");
                return;
            }

            if (currentController == null) { SendInformationToClient("HIBA: Be kell jelentkezni!"); return; }

            string targetAccountNumber = data[1];
            if (!int.TryParse(data[2], out int amount) || amount <= 0)
            {
                SendInformationToClient("HIBA: Érvénytelen összeg!");
                return;
            }

            Account targetAccount = Account.accounts.FirstOrDefault(x => x.AccountNumber == targetAccountNumber); 

            if (targetAccount == null)
            {
                SendInformationToClient("HIBA: A célszámla nem létezik!");
                return;
            }

            double fee = 0;
            if (currentController.AccountType == AccountType.Normal)
            {
                fee = amount * 0.05; // 5%
            }

            int totalDeduct = amount + (int)fee;

            if (currentController.Balance < totalDeduct)
            {
                SendInformationToClient($"HIBA: Nincs elég fedezet! (Utalás: {amount} + Díj: {fee})");
                return;
            }

            currentController.Balance -= totalDeduct;
            targetAccount.Balance += amount;

            SendInformationToClient($"SIKER: Utalás elküldve! Levonva: {totalDeduct} Ft (Díj: {fee} Ft)");
        }

        // Kiegészítendő
        private void SendInformationToClient(string msg)
        {
            writer.Flush();
            writer.WriteLine(msg);
        }

        // Kiegészítendő
        public void Close()
        {
            reader.Close();
            writer.Close();
            client.Close();
        }
    }
}

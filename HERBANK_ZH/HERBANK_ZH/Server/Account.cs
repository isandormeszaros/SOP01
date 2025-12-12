using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server
{
    internal class Account
    {
        string accountNumber;
        string accountHolder;
        int balance;
        AccountType accountType;

        static public List<Account> accounts = new List<Account>();

        public string AccountNumber { get => accountNumber; private set => accountNumber = value; }
        public string AccountHolder { get => accountHolder; private set => accountHolder = value; }
        public int Balance 
        { 
            get => balance; 
            set 
            {
                if (value < 0)
                {
                    balance = 0;
                }
                else
                {
                    balance = value;
                }
            } 
        }
        public AccountType AccountType { get => accountType; private set => accountType = value; }

        // Segítség: Ez szükséges beolvasáshoz
        private Account(string accountNumber, string accountHolder, int balance, AccountType accountType)
        {
            this.accountNumber = accountNumber;
            this.accountHolder = accountHolder;
            this.balance = balance;
            this.accountType = accountType;
        }

        // Kiegészítendő
        public Account()
        {
            // Segítség: Először a számlászámot érdemes megcsinálni, hiszen az szükséges lesz az új bankkártya készítéshezéséhez is
        }

        // Kiegészítendő
        public static Account AccountExists(string accountHolder, int pin)
        {
            //AccNumber, Pin
            DebitCard card = DebitCard.RequestCardDetails(accountHolder, pin);
            if (card == null) return null;

            return accounts.FirstOrDefault(a => a.AccountNumber == card.AccountNumber);
        }

        // Kiegészítendő
        public bool TryCreate()
        {
            return false;
        }

        public static void LoadAccounts(string fileName)
        {
            // Kiegészítendő
            //Account(string accountNumber, string accountHolder, int balance, AccountType accountType)
            accounts.Clear();
            if (File.Exists(fileName))
            {
                XDocument xml = XDocument.Load(fileName);
                foreach (XElement account in xml.Descendants("account"))
                {
                    Account a = new Account(
                        (string)account.Attribute("number"),
                        (string)account.Attribute("owner"),
                        (int)account.Attribute("balance"),
                        (AccountType)Enum.Parse(typeof(AccountType), (string)account.Attribute("type"))
                    );

                    accounts.Add(a);
                }
                Console.WriteLine("DebitCard sikeresen beolvasva!");
            }
            else Console.WriteLine("Hiba a debitCard beolvasása során!");
        }


        public static void SaveAccounts(string fileName)
        {
            // Kiegészítendő
            XElement root = new XElement("accounts");
            foreach (var account in accounts)
            {
                root.Add(
                    new XElement("account",
                    new XAttribute((XName)"number", account.AccountNumber),
                    new XAttribute((XName)"owner", account.AccountHolder),
                    new XAttribute((XName)"balance", account.Balance),
                    new XAttribute((XName)"type", account.AccountType))
                    );
            }
            XDocument xml = new XDocument(root);
            xml.Save(fileName);
            Console.WriteLine("Account sikeresen elmentve!");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Account
    {
        string accountNumber;
        string accountHolder;
        int balance;
        AccountType accountType;

        static List<Account> accounts = new List<Account>();

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
        public static Account AccountExists()
        {
            DebitCard card = DebitCard.RequestCardDetails();
            return null;
        }

        // Kiegészítendő
        public bool TryCreate()
        {
            return false;
        }

        public static void LoadAccounts(string fileName)
        {
            // Kiegészítendő
        }


        public static void SaveAccounts(string fileName)
        {
            // Kiegészítendő
        }
    }
}

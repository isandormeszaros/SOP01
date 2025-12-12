using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Server
{
    internal class DebitCard
    {
        string cardNumber;
        int pin;
        string accountNumber;

        public string CardNumber { get => cardNumber; private set => cardNumber = value; }
        public int PIN { get => pin; private set => pin = value; }
        public string AccountNumber { get => accountNumber; private set => accountNumber = value; }

        static List<DebitCard> debitCards = new List<DebitCard>();

        // Segítség: Ez szükséges beolvasáshoz
        private DebitCard(string cardNumber, int pin, string accountNumber)
        {
            this.cardNumber = cardNumber;
            this.pin = pin;
            this.accountNumber = accountNumber;
        }

        // Kiegészítendő
        public DebitCard()
        {
            
        }

        // Kiegészítendő
        // Segítség: Ezzel ellenőrizni lehet, hogy létezik-e a kártya (hasznos a bejelentkezéshez)
        public static DebitCard RequestCardDetails() 
        { 
            return null;
        }

        public static void LoadDebitCards(string fileName)
        {
            // Kiegészítendő
        }

        
        public static void SaveDebitCards(string fileName)
        {
            // Kiegészítendő
        }
    }
}

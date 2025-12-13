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
        public static DebitCard RequestCardDetails(string inputAccNumber, int inputPin) 
        { 
            return debitCards.FirstOrDefault(x => x.AccountNumber == inputAccNumber && x.PIN == inputPin);
        }

        public static void LoadDebitCards(string fileName)
        {
            // Kiegészítendő
            // DebitCard(string cardNumber, int pin, string accountNumber)
            debitCards.Clear();

            if (File.Exists(fileName))
            {
                XDocument xml = XDocument.Load(fileName);
                foreach (XElement debitcard in xml.Descendants("card"))
                {
                    DebitCard c = new DebitCard(
                        (string)debitcard.Attribute("number"),
                        (int)debitcard.Attribute("pin"),
                        (string)debitcard.Attribute("owner"));

                    debitCards.Add(c);
                }
                Console.WriteLine("DebitCard sikeresen beolvasva!");
            }
            else Console.WriteLine("Hiba a debitCard beolvasása során!");
        }

        
        public static void SaveDebitCards(string fileName)
        {
            // Kiegészítendő
            XElement root = new XElement("debitcards");
            foreach (var dc in debitCards)
            {
                root.Add(new XElement("debitcard",
                    new XAttribute((XName)"number", dc.CardNumber),
                    new XAttribute((XName)"pin", dc.PIN),
                    new XAttribute((XName)"owner", dc.AccountNumber)
                    )
                );
            }
            XDocument xml = new XDocument(root);
            xml.Save(fileName);
            Console.WriteLine("DebitCard sikeresen elmentve!");
        }
    }
}

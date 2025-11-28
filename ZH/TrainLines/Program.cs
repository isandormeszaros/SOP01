using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace TrainLines
{
    class Program
    {
        static void Main()
        {
            Train v1 = new Train("Vonat-1", "Babarest", "Fekrecen");
            Train v2 = new Train("Vonat-2", "Babarest", "Ugar");
            Train v3 = new Train("Vonat-3", "Babarest", "Szöged");

            Thread t1 = new Thread(v1.Travel);
            Thread t2 = new Thread(v2.Travel);
            Thread t3 = new Thread(v3.Travel);

            t1.Start();
            t2.Start();
            t3.Start();

            t1.Join();
            t2.Join();
            t3.Join();

            Console.WriteLine("Minden vonat teljesítette a 4 menetet!");
        }
    } 
}

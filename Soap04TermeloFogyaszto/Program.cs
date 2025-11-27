using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soap04TermeloFogyaszto
{
    internal class Program
    {
        static void Main(string[] args) 
        {
            Controller.CreateProducers(10, 10);
            Controller.CreateConsumer(10);
            Controller.StartThreads();
            Console.ReadLine();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test_Termelo_Fogyaszto
{
    internal class Product
    {
        static int nextId = 0;
        readonly int id;

        public Product()
        {
            id = Interlocked.Increment(ref nextId);
        }

        public override string ToString()
        {
            return $"Termék ID: {id}";
        }
    }
}
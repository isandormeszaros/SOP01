using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _02_CustomerSupport
{
    public class Ticket
    {
        public int Id { get; set; }
        public int Priority { get; set; }

        public override string ToString()
        {
            return $"Ticket #{Id} (Prioritás: {Priority})";
        }
    }
}

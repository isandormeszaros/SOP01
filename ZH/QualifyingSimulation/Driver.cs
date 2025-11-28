using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QualifyingSimulation
{
    class Driver
    {
        public int DriverNumber { get; }
        public List<int> LapTimes { get; }

        public Driver(int number)
        { 
            DriverNumber = number;
            LapTimes = new List<int>();
        }

        public void AddLap(int lapMs)
        {
            lock (LapTimes)
            {
                LapTimes.Add(lapMs);
            }
        }

        public int? BestLap =>
            LapTimes.Count > 0 ? LapTimes.Min() : (int?)null;
    }
}

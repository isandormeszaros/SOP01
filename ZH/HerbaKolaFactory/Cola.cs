using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColaFactory
{
    class Cola
    {
        static uint ID;
        public uint id { get; private set; }

        public ColaType type { get; private set; }
        public BottleSize size { get; private set; }

        public Cola(ColaType type = ColaType.NORMAL, BottleSize size = BottleSize.ONE)
        {
            this.type = type;
            this.size = size;

            id = ID++;
        }

        public override string ToString()
        {
            return $"{Controller.BottleSizeToLiter[size]}l " +
                $"{type} cola";
        }
    }
}

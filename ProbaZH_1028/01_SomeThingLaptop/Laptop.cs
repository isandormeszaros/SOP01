using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_SomeThingLaptop
{
    class Laptop
    {
        static uint ID;
		private uint id;

		public uint Id
		{
			get { return id; }
			private set { id = value; }
		}

		private Brand laptopBrand;

		public Brand LaptopBrand
		{
			get { return laptopBrand; }
			private set { laptopBrand = value; }
		}

		private int serialNumber;

		public int SerialNumber
		{
			get { return serialNumber; }
			private set { serialNumber = value; }
		}

		public Laptop(Brand laptopBrand, int serialNumber)
        {
            Interlocked.Increment(ref ID);
			id = ID;
			this.laptopBrand = laptopBrand;
            this.serialNumber = serialNumber;
        }

        public override string ToString()
        {
            return $"ID: {id}, Brand and Serial: {laptopBrand}{serialNumber}";
        }
    }
}

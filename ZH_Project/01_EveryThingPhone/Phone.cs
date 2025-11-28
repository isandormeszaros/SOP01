using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_EveryThingPhone
{
    class Phone
    {
        static uint ID;
        private uint id;

        public uint Id
        {
            get { return id; }
            private set { id = value; }
        }

        private Brand phoneBrand;

        public Brand PhoneBrand
        {
            get { return phoneBrand; }
            private set { phoneBrand = value; }
        }

        private Spec phoneSpec;

        public Spec PhoneSpec
        {
            get { return phoneSpec; }
            private set { phoneSpec = value; }
        }

        public Phone(Brand phoneBrand, Spec phoneSpec)
        {
            Interlocked.Increment(ref ID);
            id = ID;
            this.phoneBrand = phoneBrand;
            this.phoneSpec = phoneSpec;
        }

        public override string ToString()
        {
            return $"ID: {id}, Brand and Serial: {phoneBrand}{phoneSpec}";
        }
    }
}

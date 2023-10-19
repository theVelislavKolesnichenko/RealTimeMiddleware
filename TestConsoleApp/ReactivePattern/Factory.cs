using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsoleApp
{
    public class Factory
    {
        public Workshop Workshop1 { get; set;}

        public Workshop Workshop2 { get; set; }

        public int Sum
        {
            get
            {
                int v1 = 0;
                int v2 = 0;
                if (Workshop1 != null)
                {
                    v1 = Workshop1.Value;
                }
                if (Workshop2 != null)
                {
                    v2 = Workshop2.Value;
                }
                return v1 + v2;
            }
        }

        internal bool HasWorkshop => Workshop1 != null && Workshop2 != null;

        internal Workshop GetByClient(string guId)
        {
            if (Workshop1.Name == guId)
                return Workshop1;

            if (Workshop2.Name == guId)
                return Workshop2;

            return null;
        }

        internal void Add(string guId, int value)
        {
            bool isAdd = false;

            if (Workshop1 == null)
            {
                isAdd = true;
                Workshop1 = new Workshop { Name = guId, Value = value };
            }

            if (!isAdd && Workshop2 == null)
                Workshop2 = new Workshop { Name = guId, Value = value }; ;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    namespace DO
    {
        public struct Customer
        {
            //bound of id
            public static readonly int LowerBoundId = (int)1e8;
            public static readonly int UpperBoundId = (int)1e9;
            public int Id { get; set; }//id number with 9 digit
            public string Name { get; set; }
            public string Phone { get; set; }
            public Location Location { set; get; }
            public override string ToString()
            {
                return Print.print<Customer>(this);
            }
            public bool Check()
            {
                return this.Id >= LowerBoundId && this.Id < UpperBoundId;
            }
        };
    }
}

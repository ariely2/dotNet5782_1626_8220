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
            public int Id { get; set; }//id number with 9 digit
            public string Name { get; set; }
            public string Phone { get; set; }
            public Location location { set; get; }
            public override string ToString()
            {
                return Print.print<Customer>(this);
            }
        };
    }
}

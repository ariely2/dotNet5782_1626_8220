using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class Customer
        {
            public int Id { get; set; }//id number with 9 digit
            public string Name { get; set; }
            public string Phone { get; set; }
            public Location location { set; get; }

            public List<ParcelAtCustomer> To { set; get; }//list of parcel to the customer
            public List<ParcelAtCustomer> From { set; get; }//list of parcel from the cutomer 

            public override string ToString()
            {
                return Print.print<Customer>(this);

            }
        };
    }
}
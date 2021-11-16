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
            public GeoCoordinate Location { set; get; }

            public List<ParcelAtCustomer> To { set; get; }
            public List<ParcelAtCustomer> From { set; get; }

            public override string ToString()
            {
                return $"Id:        {Id}\n" +
                       $"Name:      {Name}\n" +
                       $"Phone:     {Phone}\n" +
                       $"Location:  {Location}\n" +
                       $"From:      " + string.Join("\n\t\t   ", (IEnumerable<ParcelAtCustomer>)From.ToArray()) +
                       $"To:        " + string.Join("\n\t\t   ", (IEnumerable<ParcelAtCustomer>)To.ToArray());

            }
        };
    }
}
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
            public GeoCoordinate Coordinate { set; get; }

            //parcel or parcelTL
            public List<Parcel> To { set; get; }
            public List<Parcel> From { set; get; }
            public override string ToString()
            {
                return $"Id:        {Id}\n" +
                       $"Name:      {Name}\n" +
                       $"Phone:     {Phone}\n" +
                       $"{Coordinate}";
            }
        };
    }
}
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
            public int Id { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public double Longitude { get; set; }
            public double Latitude { get; set; }
            public override string ToString()
            {
                return $"Customer Id:        {Id}" +
                       $"Customer Name:      {Name}" +
                       $"Customer Phone:     {Phone}" +
                       $"Customer Longitude: {Longitude}" +
                       $"Customer Latitude:  {Latitude}";
            }
        };
    }
}

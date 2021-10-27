using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    namespace DO
    {
        public struct Station
        {
            public int Id { get; set; }
            public string Name{ get; set; }
            public double Longitude { get; set; }
            public double Latitude { get; set; }
            public int ChargeSlots { get; set; }

            public override string ToString()
            {
                return $"Id:          {Id}\n" +
                       $"Name:        {Name}\n" +
                       $"Longitude:   {Longitude}\n" +
                       $"Latitude:    {Latitude}\n" +
                       $"ChargeSlots: {ChargeSlots}\n";
            }
        }
    }
}

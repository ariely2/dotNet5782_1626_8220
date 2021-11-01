using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace IDAL
{
    namespace DO
    {
        /// <summary>
        /// the struct represent a station
        /// </summary>
        public struct Station
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public GeoCoordinate Coordinate { set; get; }
            public int ChargeSlots { get; set; }
            public override string ToString()
            {
                return $"Id:          {Id}\n" +
                       $"Name:        {Name}\n" +
                       $"{Coordinate}" +
                       $"ChargeSlots: {ChargeSlots}\n";
            }
        }
    }
}

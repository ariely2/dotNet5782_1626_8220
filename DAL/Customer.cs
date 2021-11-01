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
            public GeoCoordinate Coordinate { set; get; }
            public override string ToString()
            {
                return $"Id:        {Id}\n" +
                       $"Name:      {Name}\n" +
                       $"Phone:     {Phone}\n" +
                       $"{Coordinate.ToString()}";
            }
        };
    }
}

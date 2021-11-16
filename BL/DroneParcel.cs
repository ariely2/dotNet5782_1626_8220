using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class DroneParcel
    {
        public int Id { set; get; }
        public double Baterry { set; get; }
        public GeoCoordinate Location { set; get; }
        public override string ToString()
        {
            return $"Id:       {Id}\n" +
                   $"Batterry: {Baterry}\n" +
                   $"Location: {Location}";
        }
    }

}

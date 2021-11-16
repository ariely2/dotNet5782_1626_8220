using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace IBL.BO
{
        /// <summary>
        /// the class represents a station
        /// </summary>
        public class Station
        {
            public int Id { get; set; }//id with 3 digits
            public string Name { get; set; }
            public GeoCoordinate Location { set; get; }
            public int ChargeSlots { get; set; }
            public List<DroneCharge> Charging { get; set; }
            public override string ToString()
            {
            return $"Id:                {Id}\n" +
                   $"Name:              {Name}\n" +
                   $"Location:          {Location}\n" +
                   $"ChargeSlots:       {ChargeSlots}\n" +
                   "Charging Drones:    " + string.Join("\n\t\t\t\t\t ", (IEnumerable<DroneCharge>)Charging.ToArray());
                       
            }
        }
}

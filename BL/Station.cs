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
            public Location location { set; get; }
            public int AvailableSlots { get; set; }
            public List<DroneCharge> Charging { get; set; }
            public override string ToString()
            {
            return Print.print<Station>(this);

        }
        }
}

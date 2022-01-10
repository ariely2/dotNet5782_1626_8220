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
        /// <summary>
        /// id of the station
        /// consist of 3 digits
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// name of the station
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// location of the station
        /// </summary>
        public Location location { set; get; }

        /// <summary>
        /// available slots in the station (in initialization it's the number of charge slots in the station)
        /// </summary>
        public int AvailableSlots { get; set; }

        /// <summary>
        /// list of drone that currently charging in the station
        /// </summary>
        public IEnumerable<DroneCharge> Charging { get; set; }

        /// <summary>
        /// override the function ToString
        /// </summary>
        /// <returns>a string that consists all the info about the station</returns>
        public override string ToString()
        {
            return Print.print<Station>(this);


        }
    }
}

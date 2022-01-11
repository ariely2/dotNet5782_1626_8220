using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// the class that represents a drone
    /// </summary>
    public class Drone
    {
        /// <summary>
        /// drone's id
        /// consist of 5 digits
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// drone's model
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// max weight of a parcel that the drone can carry (light, medium, heavy)
        /// </summary>
        public WeightCategories MaxWeight { get; set; }

        /// <summary>
        /// drone's battery
        /// </summary>
        public double Battery { get; set; }

        /// <summary>
        /// status of the drone (available, maintenance, delivery)
        /// </summary>
        public DroneStatuses Status { get; set; }

        /// <summary>
        /// parcel that the drone carry
        /// </summary>
        public ParcelDeliver Parcel { set; get; }

        /// <summary>
        /// current location of the drone
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// override the function ToString
        /// </summary>
        /// <returns>string with the drone's info</returns>
        public override string ToString()
        {
            return Print.print<Drone>(this);
        } 
    }
}

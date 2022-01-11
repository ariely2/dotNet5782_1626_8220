using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// the class represent a drone in a list
    /// </summary>
    public class DroneToList
    {
        /// <summary>
        /// drone's id
        /// consist of 5 digits
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// drone's model
        /// </summary>
        public string Model { set; get; }
        
        /// <summary>
        /// max weight that the drone can carry (light, medium, heavy)
        /// </summary>
        public WeightCategories MaxWeight { set; get; }

        /// <summary>
        /// battery of the drone
        /// </summary>
        public double Battery { set; get; }

        /// <summary>
        /// status of the drone( available, maintenance, delivery)
        /// </summary>
        public DroneStatuses Status { set; get; }

        /// <summary>
        /// current location of the drone
        /// </summary>
        public Location Location { set; get; }

        /// <summary>
        /// id of the parcel that the drone carry right now
        /// null, if the drone isn't carry a parcel
        /// </summary>
        public int? ParcelId { set; get; }

        /// <summary>
        /// override the functino ToString
        /// </summary>
        /// <returns>string with the drone's info</returns>
        public override string ToString()
        {
            return Print.print<DroneToList>(this);
        }
    }
}

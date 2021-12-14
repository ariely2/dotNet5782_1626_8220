using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace IBL.BO
{
    /// <summary>
    /// the class represent a drone in a parcel object
    /// </summary>
    public class DroneParcel
    {
        /// <summary>
        /// id of the drone
        /// consist of 5 digit
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// battery of the drone
        /// </summary>
        public double Battery { set; get; }

        /// <summary>
        /// current location of the drone
        /// </summary>
        public Location Location { set; get; }

        /// <summary>
        /// override the function ToString
        /// </summary>
        /// <returns>string with the drone's info</returns>
        public override string ToString()
        {
            return Print.print<DroneParcel>(this);
        }
    }

}

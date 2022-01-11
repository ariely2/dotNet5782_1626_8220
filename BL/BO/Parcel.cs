using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// class that represent parcel
    /// </summary>
    public class Parcel
    {
        /// <summary>
        /// id of the parcel
        /// consist of 8 digits
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// sender of the parcel
        /// </summary>
        public CustomerParcel Sender { get; set; }

        /// <summary>
        /// receiver of the parcel
        /// </summary>
        public CustomerParcel Receiver { get; set; }

        /// <summary>
        /// weight of the parcel (light, medium, heavy)
        /// </summary>
        public WeightCategories Weight { get; set; }

        /// <summary>
        /// priority of the parcel (normal, fast, emergency)
        /// </summary>
        public Priorities Priority { get; set; }

        /// <summary>
        /// the drone which deliver the parcel
        /// </summary>
        public DroneParcel Drone { get; set; }

        /// <summary>
        /// time of create the parcel
        /// </summary>
        public DateTime? Requested { get; set; }

        /// <summary>
        /// time of assign the parcel to a drone
        /// null, if it didn't happen yet
        /// </summary>
        public DateTime? Scheduled { get; set; }

        /// <summary>
        /// time of pick up the parcel
        /// null, if the drone didn't pick up the parcel
        /// </summary>
        public DateTime? PickedUp { get; set; }

        /// <summary>
        /// time of deliver the parcel
        /// null, if the drone didn't pick up the parcel yet
        /// </summary>
        public DateTime? Delivered { get; set; }

        /// <summary>
        /// override the function ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()//tostring also here or only in TL classes?
        {
            return Print.print<Parcel>(this);
        }
    }
}

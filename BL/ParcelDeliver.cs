using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    /// <summary>
    /// the class represent a parcel in delivery
    /// </summary>
    public class ParcelDeliver
    {
        /// <summary>
        /// id of the parcel
        /// consist of 8 digits
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// status of the parcel
        /// 0 -> wait for pick up
        /// 1-> on the way to receiver
        /// </summary>
        public EnumParcelDeliver Status { set; get; }
        
        /// <summary>
        /// priority of the parcel (normal, fast, emergency)
        /// </summary>
        public Priorities Priority { set; get; }

        /// <summary>
        /// weight of the parcel (light, medium, heavy)
        /// </summary>
        public WeightCategories Weight { set; get; }
       
        /// <summary>
        /// sender details
        /// </summary>
        public CustomerParcel Sender { set; get; }

        /// <summary>
        /// reciever details
        /// </summary>
        public CustomerParcel Receiver { set; get; }
        
        /// <summary>
        /// location of picked up
        /// </summary>
        public Location Source { set; get; }

        /// <summary>
        /// location of destination
        /// </summary>
        public Location Destination { set; get; }

        /// <summary>
        /// transport distance
        /// </summary>
        public double Distance { set; get; }

        /// <summary>
        /// override the function ToString
        /// </summary>
        /// <returns>string with info about the parcel</returns>
        public override string ToString()
        {
            return Print.print<ParcelDeliver>(this);

        }
    }
}

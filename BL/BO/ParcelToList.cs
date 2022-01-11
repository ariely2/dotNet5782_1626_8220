using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// class that represent parcel in a list
    /// </summary>
    public class ParcelToList
    {
        /// <summary>
        /// id of the parcel
        /// consist of 8 digits
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// name of the sender 
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// name of the receiver
        /// </summary>
        public string ReceiverName { get; set; }

        /// <summary>
        /// weight of the parcel (light, medium, heavy)
        /// </summary>
        public WeightCategories Weight { set; get; }

        /// <summary>
        /// priority of the parcel (normal, fast, emergency)
        /// </summary>
        public Priorities Priority { set; get; }

        /// <summary>
        /// status of the parcel (created, assigned, picked up, delivered)
        /// </summary>
        public ParcelStatuses Status { set; get; }

        /// <summary>
        /// override the function ToString
        /// </summary>
        /// <returns>string with parcel info</returns>
        public override string ToString()
        {
            return Print.print<ParcelToList>(this);
        }
    }
}

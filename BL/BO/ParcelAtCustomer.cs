using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// class that represent parcel at customer
    /// </summary>
    public class ParcelAtCustomer
    {
        /// <summary>
        /// id of the parcel
        /// consist of 8 digits
        /// </summary>
        public int Id { set; get; }

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
        /// sender/receiver of the parcel
        /// receiver - when the id is of the sender and vice versa
        /// </summary>
        public CustomerParcel Customer { set; get; }

        /// <summary>
        /// override the fucntion ToString
        /// </summary>
        /// <returns>string with info about the parcel</returns>
        public override string ToString()
        {
            return Print.print<ParcelAtCustomer>(this);
        }
    }
}

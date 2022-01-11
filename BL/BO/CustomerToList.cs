using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// class that represent customer in a list
    /// </summary>
    public class CustomerToList
    {
        /// <summary>
        /// id of the customer
        /// consist of 9 digits
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// name of the customer
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// phone of the customer
        /// </summary>
        public string Phone { set; get; }

        /// <summary>
        /// number of parcels that the customer send arrived to destination
        /// </summary>
        public int Delivered { set; get; }

        /// <summary>
        /// nubmer of parcels that the customer send and didn't arrive yet to destination
        /// </summary>
        public int NoDelivered { set; get; }

        /// <summary>
        /// number of parcels, the customer already received
        /// </summary>
        public int Received { set; get; }

        /// <summary>
        /// nubmer of parcels that are on the way to customer
        /// </summary>
        public int NoReceived { set; get; }

        /// <summary>
        /// override the function ToString
        /// </summary>
        /// <returns>string with customer info</returns>
        public override string ToString()
        {
            return Print.print<CustomerToList>(this);
        }

    }
}

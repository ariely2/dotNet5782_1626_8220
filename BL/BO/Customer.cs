using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// class that represent a customer
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// id of the custoemr
        /// consist of 9 digits
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// name of the customer
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// phone of the customer
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// location of the customer
        /// </summary>
        public Location location { set; get; }

        /// <summary>
        /// list of parcels that sent to the customer
        /// </summary>
        public IEnumerable<ParcelAtCustomer> To { set; get; }

        /// <summary>
        /// list of parcels that send from the customer
        /// </summary>
        public IEnumerable<ParcelAtCustomer> From { set; get; }

        /// <summary>
        /// override the function ToString
        /// </summary>
        /// <returns>string with customer info</returns>
        public override string ToString()
        {
            return Print.print<Customer>(this);

        }
    };

}
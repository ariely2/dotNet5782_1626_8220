using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// class that represetn customer in parcel object
    /// </summary>
    public class CustomerParcel
    {
        /// <summary>
        /// id of the object
        /// consist of 9 digits
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// name of the customer
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// override the function ToString
        /// </summary>
        /// <returns>string with customer info</returns>
        public override string ToString()
        {
            return Print.print<CustomerParcel>(this);
        }
    }
}

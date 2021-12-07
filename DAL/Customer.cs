using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    namespace DO
    {
        /// <summary>
        /// the struct represent a customer in the database
        /// </summary>
        public struct Customer
        {
            //bounds of id
            public static readonly int LowerBoundId = (int)1e8;
            public static readonly int UpperBoundId = (int)1e9;

            //id of the customer
            //id is 9 digit number
            public int Id { get; set; }

            //name of the customer
            public string Name { get; set; }

            //phone number of the customer
            public string Phone { get; set; }

            //customer location
            public Location Location { set; get; }

            /// <summary>
            /// Override the function ToString in class Object
            /// </summary>
            /// <returns>return a string in this format:
            /// Id: Id.ToString()
            /// Name: Name.ToString()
            /// Phone: Phone.ToString()
            /// Location: Location.ToString()
            /// </returns>
            public override string ToString()
            {
                return Print.print<Customer>(this);
            }

            /// <summary>
            /// the function check if the id is in bounds
            /// </summary>
            /// <returns>return true/false, depends if the id is valid</returns>
            public bool Check()
            {
                return this.Id >= LowerBoundId && this.Id < UpperBoundId;
            }
        };
    }
}

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
        /// the struct represent a station in the database
        /// </summary>
        public struct Station
        {
            //bounds of id
            public static readonly int LowerBoundId = (int)1e2;
            public static readonly int UpperBoundId = (int)1e3;

            //id of the station, 3 digit
            public int Id { get; set; }

            //name of the station
            public string Name { get; set; }

            //location of the station
            public Location Location { set; get; }

            //available chargeslots in the station
            public int ChargeSlots { get; set; }

            /// <summary>
            /// override the function ToString in class Object
            /// </summary>
            /// <returns>return a string in this format:
            /// Id: Id.ToString()
            /// Name: Name.ToString()
            /// Location: Location.ToString()
            /// ChargeSlots: ChargeSlots.ToString()</returns>
            public override string ToString()
            {
                return Print.print<Station>(this);
            }


            /// <summary>
            /// function to check if the id is in bounds
            /// </summary>
            /// <returns>return true/false, depends on the id</returns>
            public bool Check()
            {
                return this.Id >= LowerBoundId && this.Id < UpperBoundId;
            }
        }
    }
}

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
        /// the struct represent a drone that is charging in a station
        /// </summary>
        public struct DroneCharge
        {
            //drone id
            public int DroneId { get; set; }
            
            //Station id
            public int StationId { get; set; }

            /// <summary>
            /// Override the function ToString in class Object
            /// </summary>
            /// <returns>string with drone charge info </returns>
            public override string ToString()
            {
                return Print.print<DroneCharge>(this);
            }

        }
    }
}

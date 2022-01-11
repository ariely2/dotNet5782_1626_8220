using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// this class represents a drone that's currently charging
    /// </summary>
    public class DroneCharge
    {
        /// <summary>
        /// id of the drone
        /// consist of 5 digits
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// battery of the drone
        /// </summary>
        public double Battery { get; set; }

        /// <summary>
        /// override the fucntion ToString
        /// </summary>
        /// <returns>rstring of the drone info</returns>
        public override string ToString()
        {
            return Print.print<DroneCharge>(this);
        }
    }
}

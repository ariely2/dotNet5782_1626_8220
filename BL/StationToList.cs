using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class StationToList
    {
        /// <summary>
        /// id of the station
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// name of the station
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// available slots in the station
        /// </summary>
        public int Available { set; get; }

        /// <summary>
        /// occupied slots in the station
        /// </summary>
        public int Occupied { set; get; }

        /// <summary>
        /// override the function ToString
        /// </summary>
        /// <returns>return a string with all the info about the station</returns>
        public override string ToString()
        {
            return Print.print<StationToList>(this);
        }
    }
}

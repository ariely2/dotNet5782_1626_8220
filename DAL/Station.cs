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
        /// the struct represent a station
        /// </summary>
        public struct Station
        {
            public int Id { get; set; }//id with 3 digit
            public string Name { get; set; }
            public Location location { set; get; }
            public int ChargeSlots { get; set; }
            public override string ToString()
            {
                return Print.print<Station>(this);
            }
        }
    }
}

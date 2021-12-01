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
            //to check if the id is in bound
            public static readonly int LowerBoundId = (int)1e2;
            public static readonly int UpperBoundId = (int)1e3;
            public int Id { get; set; }//id with 3 digit
            public string Name { get; set; }
            public Location Location { set; get; }
            public int ChargeSlots { get; set; }
            public override string ToString()
            {
                return Print.print<Station>(this);
            }
            public bool Check()
            {
                return this.Id >= LowerBoundId && this.Id < UpperBoundId;
            }
        }
    }
}

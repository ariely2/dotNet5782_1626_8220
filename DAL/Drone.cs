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
        /// the class represent a drone
        /// </summary>
        public struct Drone
        {

            public static readonly int LowerBoundId = (int)1e4;
            public static readonly int UpperBoundId = (int)1e5;

            //move the id check from dalObject to set function?
            public int Id { get; set; }//id with 5 digit
            public string Model { get; set; }
            public WeightCategories MaxWeight { get; set; }

            public override string ToString()
            {
                return Print.print<Drone>(this);
            }
            public bool Check()
            {
                return this.Id >= LowerBoundId && this.Id < UpperBoundId;
            }
        }
    }
}

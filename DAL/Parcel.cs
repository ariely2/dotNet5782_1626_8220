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
        /// the struct represent a parcel
        /// </summary>
        public struct Parcel
        {
            public int Id { get; set; }//id with 8 digit
            public int SenderId { get; set; }
            public int TargetId { get; set; }
            public WeightCategories Weight { get; set; }
            public Priorities Priority { get; set; }
            public int DroneId { get; set; }
            public DateTime Requested { get; set; }
            public DateTime Scheduled { get; set; }//min value if not scheduled
            public DateTime PickedUp { get; set; }//min value if not picked up
            public DateTime Delivered { get; set; }//min value if not delivered
            public override string ToString()
            {
                return Print.print<Parcel>(this);
            }

        } 
    }
}

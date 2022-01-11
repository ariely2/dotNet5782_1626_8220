using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DO
{
    /// <summary>
    /// the struct represent a parcel in the database
    /// </summary>
    public struct Parcel
    {
        //bounds of id
        public static readonly int LowerBoundId = (int)1e8;
        public static readonly int UpperBoundId = (int)1e9;

        //id of the parcel, 8 digit number
        public int Id { get; set; }

        //sender id
        public int SenderId { get; set; }

        //receiver id
        public int ReceiverId { get; set; }

        //weight of the parcel
        public WeightCategories Weight { get; set; }

        //priority of the parcel
        public Priorities Priority { get; set; }

        //id of the drone. if the drone isn't assigned yet, then the value is null
        public int? DroneId { get; set; }

        //time to requested the parcel
        public DateTime? Requested { get; set; }

        //time to assign a drone to this parcel. if it didn't happen, then the value is null
        public DateTime? Scheduled { get; set; }

        //time the drone picked up the parcel. if it didn't happen, then the value is null
        public DateTime? PickedUp { get; set; }

        //time the drone delivered the parcel to the target. if it didn't happen, then the value is null
        public DateTime? Delivered { get; set; }

        /// <summary>
        /// override the function ToString in class Object
        /// </summary>
        /// <returns>string with parcel info</returns>
        public override string ToString()
        {
            return Print.print<Parcel>(this);
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


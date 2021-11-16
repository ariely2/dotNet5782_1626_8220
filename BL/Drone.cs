using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{ 
        /// <summary>
        /// the class that represents a drone
        /// </summary>
        public class Drone
        {
            public int Id { get; set; }
            public string Model { get; set; }
            public WeightCategories MaxWeight { get; set; }
            public double Battery { get; set; }
            public DroneStatuses Status { get; set; }
            public ParcelDeliver Parcel { set; get; }
            public  Location Location { get; set; } //current location
            public override string ToString()
            {
            return $"Id:        {Id}\n" +
                   $"Model:     {Model}\n" +
                   $"MaxWeight: {MaxWeight}\n" +
                   $"Battery:   {Battery}\n" +
                   $"Status:    {Status}\n" +
                   $"Parcel:    {Parcel}" +
                   $"Location:  {Location}";
            }
    }
}

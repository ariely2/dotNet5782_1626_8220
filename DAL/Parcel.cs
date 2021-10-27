using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    namespace DO
    {
        public struct Parcel
        {
            public int Id { get; set; }
            public int SenderId { get; set; }
            public int TargetId { get; set; }
            public WeightCategories Weight { get; set; }
            public Priorities Priority { get; set; }
            public int DroneId { get; set; } //check if default value is 0
            public DateTime Requested { get; set; }
            public DateTime Scheduled { get; set; }
            public DateTime PickedUp { get; set; }
            public DateTime Delivered { get; set; }
            public override string ToString()
            {
                return $"Parcel Id:      {Id}\n" +
                       $"Sender Id:      {SenderId}\n" +
                       $"Target Id:      {TargetId}\n" +
                       $"Weight:         {Weight}\n" +
                       $"Priority:       {Priority}\n" +
                       $"Drone Id:       {DroneId}\n" +
                       $"Request Time:   {Requested}\n" +
                       $"Scheduled Time: {Scheduled}\n" +
                       $"Pick-up Time:   {PickedUp}\n" +
                       $"Delivery Time:  {Delivered}\n"; 
            }

        } 
    }
}

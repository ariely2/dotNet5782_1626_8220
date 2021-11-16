using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class Parcel
    {
        public int Id { get; set; }
        public CustomerParcel Sender { get; set; }
        public CustomerParcel Receiver { get; set; }
        public WeightCategories Weight { get; set; }
        public Priorities Priority { get; set; }
        public DroneParcel Drone { get; set; }
        public DateTime Requested { get; set; }
        public DateTime Scheduled { get; set; }//min value if not scheduled
        public DateTime PickedUp { get; set; }//min value if not picked up
        public DateTime Delivered { get; set; }//min value if not delivered
        public override string ToString()//tostring also here or only in TL classes?
        {
            return $"Parcel Id:      {Id}\n" +
                   $"Sender:         {Sender}" +
                   $"receiver:       {Receiver}" +
                   $"Weight:         {Weight}\n" +
                   $"Priority:       {Priority}\n" +
                   $"Drone:          {Drone}" +
                   $"Request Time:   {Requested}\n" +
                   $"Scheduled Time: {Scheduled}\n" +
                   $"Pick-up Time:   {PickedUp}\n" +
                   $"Delivery Time:  {Delivered}\n";
        }
    }
}

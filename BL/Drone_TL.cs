using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class Drone_TL
    {
        public int Id { set; get; }
        public string Model { set; get; }
        public WeightCategories Weight { set; get; }
        public double Battery { set; get; }
        public DroneStatuses Status { set; get; }
        public Location Location { set; get; }
        public int ParcelId { set; get; }

        public override string ToString()
        {
            return $"Id:       {Id}\n" +
                   $"Model:    {Model}\n" +
                   $"Weight:   {Weight}\n" +
                   $"Battery:  {Battery}\n" +
                   $"Status:   {Status}\n" +
                   $"Location: {Location}" +
                   $"ParcelId: {ParcelId}";
        }
    }
}

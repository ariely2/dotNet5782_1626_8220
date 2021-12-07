using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class DroneToList
    {
        public int Id { set; get; }
        public string Model { set; get; }
        public WeightCategories MaxWeight { set; get; }
        public double Battery { set; get; }
        public DroneStatuses Status { set; get; }
        public Location Location { set; get; }
        public int? ParcelId { set; get; }//null, if the drone isn't assigned yet

        public override string ToString()
        {
            return Print.print<DroneToList>(this);
        }
    }
}

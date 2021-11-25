using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace IBL.BO
{
    public class DroneParcel
    {
        public int Id { set; get; }
        public double Baterry { set; get; }
        public Location Location { set; get; }
        public override string ToString()
        {
            return Print.print<DroneParcel>(this);
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    /// <summary>
    /// this class represents a drone that's currently charging
    /// </summary>
    public class DroneCharge
    {
        public int Id { get; set; }
        public double Battery { get; set; }
        public override string ToString()
        {
            return $"Id:       {Id}\n" +
                   $"Battery:  {Battery}\n";
            }
    }
}

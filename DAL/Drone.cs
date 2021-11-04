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
            public int Id { get; set; }//id with 5 digit
            public string Model { get; set; }
            public WeightCategories MaxWeight { get; set; }
            //public DroneStatuses Status { get; set; }
            //public double Battery { get; set; }

            public override string ToString()
            {
                return $"Id:        {Id}\n" +
                       $"Model:     {Model}\n" +
                       $"MaxWeight: {MaxWeight}\n";
                       //$"Status:    {Status}\n" +
                       //$"Battery:   {Battery}\n";
            }
        }
    }
}

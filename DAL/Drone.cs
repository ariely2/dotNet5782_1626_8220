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
        /// the class represent a drone in the database
        /// </summary>
        public struct Drone
        {
            //Bounds of id
            public static readonly int LowerBoundId = (int)1e4;
            public static readonly int UpperBoundId = (int)1e5;

            //Id of the drone, id is 5 digit number
            public int Id { get; set; }

            //Drone model
            public string Model { get; set; }

            //The heaviest package the drone can carry
            public WeightCategories MaxWeight { get; set; }

            /// <summary>
            /// Override the function ToString in class Object
            /// </summary>
            /// <returns>return a string in this format:
            /// Id: Id.ToString()
            /// Model: Model.ToString()
            /// MaxWeight: MaxWeight.ToString()
            /// </returns>
            public override string ToString()
            {
                return Print.print<Drone>(this);
            }

            /// <summary>
            /// the function check if the id is in bounds
            /// </summary>
            /// <returns>return true/false depends on the id </returns>
            public bool Check()
            {
                return this.Id >= LowerBoundId && this.Id < UpperBoundId;
            }
        }
    }
}

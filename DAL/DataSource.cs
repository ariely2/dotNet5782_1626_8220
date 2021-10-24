using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL.DO;

namespace DalObject
{
    public class DataSource
    {
        internal static Customer[] Customers = new Customer[100];
        internal static Parcel[] Parcels = new Parcel[1000];
        internal static Drone[] Drones = new Drone[10];
        //internal static Station[] Stations = new Station[5];
        internal class Config
        {
            internal static int FreeC = 0; ///index of first free index in Customers
            internal static int FreeP = 0; ///index of first free index in Parcels
            internal static int FreeD = 0; ///index of first free index in Drones
            internal static int FreeS = 0; ///index of first free index in Stations
            //some int
            // what type is initialize? 
        }
    }
}

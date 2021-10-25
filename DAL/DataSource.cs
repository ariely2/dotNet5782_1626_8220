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

        internal static List<Customer> Customers = new List<Customer>(100); 
        internal static List<Parcel> Parcels = new List<Parcel>(1000);
        internal static List<Drone> Drones = new List<Drone>(10);
        internal static List<Station> Stations = new List<Station>(5);
        internal class Config
        {
            static Random R = new Random();

            //some int?
            internal static void Initialize()
            {
                string[] CNames = new string[] { "Mario", "Carmen", "Tamar", "Daniel", "Adam", "Rosie", "Paul", "Jessie", "Leslie", "Charlie" };
                for (int i = 0; i < 10; i++) /// Random Id between 1 and 1000000000. Name from Name array. Random phone number (adding zeroes in the front). Random Longitude between -180 and 80. Random Latitude between -90 and 90.
                    Customers.Add(new Customer() { Id = R.Next(1000000000), Name = CNames[i], Phone = R.Next(111111, 999999).ToString("000000"), Longitude = (R.NextDouble() * 260) - 180, Latitude = (R.NextDouble() * 180) - 90 });
                string[] SNames = new string[] { "Florentin", "Mamilla", "Hulon" };
                for (int i = 0; i < 3; i++) ///Random Id with 3 digits. Name from Name array. Random Longitude between -180 and 80. Random Latitude between -90 and 90. Random number of available slots between 0 and 3.
                    Stations.Add(new Station() { Id = R.Next(100, 1000), Name = SNames[i], Longitude = (R.NextDouble() * 260) - 180, Latitude = (R.NextDouble() * 180) - 90, ChargeSlots = R.Next(4) });

            }

        }
    }
}

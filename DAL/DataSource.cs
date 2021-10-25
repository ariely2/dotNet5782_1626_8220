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
            static int IdOfParcel = 1;
            //some int?
            internal static void Initialize()
            {
                ///initialize 10 customers
                string[] CNames = new string[] { "Mario", "Carmen", "Tamar", "Daniel", "Adam", "Rosie", "Paul", "Jessie", "Leslie", "Charlie" };
                for (int i = 0; i < 10; i++) /// Random Id between 1 and 1000000000. Name from Name array. Random phone number (adding zeroes in the front). Random Longitude between -180 and 80. Random Latitude between -90 and 90.
                    Customers.Add(new Customer() { Id = R.Next(1000000000), Name = CNames[i], Phone = R.Next(111111, 999999).ToString("000000"), Longitude = (R.NextDouble() * 260) - 180, Latitude = (R.NextDouble() * 180) - 90 });
                
                ///initialize 3 stations
                string[] SNames = new string[] { "Florentin", "Mamilla", "Hulon" };
                for (int i = 0; i < 3; i++) ///Random Id with 3 digits. Name from Name array. Random Longitude between -180 and 80. Random Latitude between -90 and 90. Random number of available slots between 0 and 3.
                    Stations.Add(new Station() { Id = R.Next(100, 1000), Name = SNames[i], Longitude = (R.NextDouble() * 260) - 180, Latitude = (R.NextDouble() * 180) - 90, ChargeSlots = R.Next(4) });

                ///initialize 5 drones
                //need to add Model Names
                string[] MNames = new string[] { };
                for (int i = 0; i < 5; i++)///Random Id with 5 digit. Name from Model Array. Random Max Weight. Random Status. Random battery between 0 to 100.
                    Drones.Add(new Drone() { Id = R.Next(10000, 100000),Model = MNames[i], MaxWeight = EnumExtension.RandomEnumValue<WeightCategories>(),Status = EnumExtension.RandomEnumValue<DroneStatuses>() , Battery = R.Next(0, 101) });
                
                ///initialize 10 parcel
                // need to add DateTime Variables: Delivered, PickedUp, Scheduled, Requested
                for(int i = 0; i < 10; i++)
                {
                    int j = 0;
                    WeightCategories ParcelMaxWeight = EnumExtension.RandomEnumValue<WeightCategories>();

                    ///looking for an avaliable drone with enough battery and can carry the parcel
                    for (; j < Drones.Count() && (Drones[j].Status != DroneStatuses.Avaliable || Drones[j].Battery == 0 || Drones[j].MaxWeight < ParcelMaxWeight); j++) ;

                    ///Id equals to IdOfParcle. SenderId from customers list. TragetId from customers list. random max Weight. DroneId from list of Drones (only if there is exist such a drone). Random Priority. 
                    Parcels.Add(new Parcel() { Id = IdOfParcel++, SenderId = Customers[R.Next(Customers.Count())].Id, TargetId = Customers[R.Next(Customers.Count())].Id,Weight = ParcelMaxWeight,DroneId = (j==5?0:Drones[j].Id),Priority = EnumExtension.RandomEnumValue<Priorities>()});
                }
                
            }

        }
    }
}

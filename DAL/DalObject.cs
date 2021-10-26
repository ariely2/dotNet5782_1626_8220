using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL.DO;
namespace IDAL
{
    namespace DalObject
    {
        public class DalObject
        {
            public DalObject()
            {
                DataSource.Config.Initialize();
            }
            public static void AddStation(int id, string name, int chargeSlots, int longitude, int latitude)
            {
                DataSource.Stations.Add(new Station()
                {
                    Id = id,
                    Name = name,
                    ChargeSlots = chargeSlots,
                    Longitude = longitude,
                    Latitude = latitude
                });
            }
            public static void AddDrone(int id,string name, WeightCategories weight, int battery, DroneStatuses status)
            {
                DataSource.Drones.Add(new Drone()
                {
                    Id = id,
                    Model = name,
                    MaxWeight = weight,
                    Battery = battery,
                    Status = status
                }) ;
            }
            public static void AddCustomer(int id, string name, string phone, int longtitude, int latitude)
            {
                DataSource.Customers.Add(new Customer()
                {
                    Id = id,
                    Name = name,
                    Phone = phone,
                    Longitude = longtitude,
                    Latitude = latitude
                });
            }
            public static void AddParcel(int senderId, int recieverId, WeightCategories weight, Priorities priority, int droneId)
            {

            }
        }
    }
}


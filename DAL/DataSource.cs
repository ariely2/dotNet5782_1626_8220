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
        public class DataSource
        {

            internal static List<Customer> Customers = new List<Customer>(100);
            internal static List<Parcel> Parcels = new List<Parcel>(1000);
            internal static List<Drone> Drones = new List<Drone>(10);
            internal static List<Station> Stations = new List<Station>(5);
            internal static List<DroneCharge> DroneCharges = new List<DroneCharge>();
            internal class Config
            {
                public static int IdOfParcel = 1000000;
                static Random R = new Random();

                /// <summary>
                /// the function Initial the database.
                /// 10 customers, 3 stations, 5 drones and 10 parcels
                /// </summary>
                internal static void Initialize()
                {
                    ///initialize 10 customers
                    string[] CNames = new string[] { "Mario", "Carmen", "Tamar", "Daniel", "Adam", "Rosie", "Paul", "Jessie", "Leslie", "Charlie" };
                    for (int i = 0; i < 10; i++)
                        Customers.Add(new Customer()
                        {
                            Id = R.Next(1000000000),///Random Id between 1 and 1000000000
                            Name = CNames[i],///Name from the array
                            Phone = R.Next(111111, 999999).ToString("000000"),///Random phone number (adding zeroes in the front)
                            Longitude = (R.NextDouble() * 260) - 180,///Random Longitude between -180 and 80
                            Latitude = (R.NextDouble() * 180) - 90///Random latitude between -90 and 90
                        });

                    ///initialize 3 stations
                    string[] SNames = new string[] { "Florentin", "Mamilla", "Hulon" };
                    for (int i = 0; i < 3; i++)
                        Stations.Add(new Station()
                        {
                            Id = R.Next(100, 1000),///Random Id with 3 digits
                            Name = SNames[i],///Name from Name array.
                            Longitude = (R.NextDouble() * 260) - 180,///Random Longitude between -180 and 80
                            Latitude = (R.NextDouble() * 180) - 90,///Random Latitude between -90 and 90
                            ChargeSlots = R.Next(1,4)///Random number of available slots between 1 and 3
                        });

                    ///initialize 5 drones
                    string[] MNames = new string[] { "A", "B", "C", "D", "E" };
                    for (int i = 0; i < 5; i++)
                    {
                        //if the status is assigned, then we change it to available
                        //becauce it can't be assigned if there is no parcels
                        DroneStatuses status = EnumExtension.RandomEnumValue<DroneStatuses>();
                        status = status == DroneStatuses.Assigned ? DroneStatuses.Available : status;

                        Drones.Add(new Drone()
                        {
                            Id = R.Next(10000, 100000),///Random Id with 5 digits
                            Model = MNames[i],///Name from model array
                            MaxWeight = EnumExtension.RandomEnumValue<WeightCategories>(),///Random MaxWeight
                            Status = status,///Random Status
                            Battery = R.Next(0, 101)///Random battery percentage between 0 and 100
                        });

                        //if the drone status is maintenance, then it must be charge in some station
                        if (status == DroneStatuses.Maintenance)
                        {
                            //seach for station with available charge slots
                            Station s = Stations.Find(x => x.ChargeSlots != 0);
                            Stations.Add(new Station()
                            {
                                Id = s.Id,
                                ChargeSlots = s.ChargeSlots - 1,
                                Latitude = s.Latitude,
                                Longitude = s.Longitude,
                                Name = s.Name
                            });
                            DroneCharges.Add(new DroneCharge() { DroneId = Drones[i].Id, StationId = s.Id }); ;

                            Stations.Remove(s); 
                        }
                    }

                    ///initialize 10 parcels
                    for (int i = 0; i < 10; i++)
                    {
                        //need to use function Find to make it look better
                        int j = 0;
                        WeightCategories ParcelMaxWeight = EnumExtension.RandomEnumValue<WeightCategories>();
                        ///looking for an avaliable drone with enough battery and can carry the parcel
                        for (; j < Drones.Count() && (Drones[j].Status != DroneStatuses.Available || Drones[j].Battery == 0 || Drones[j].MaxWeight > ParcelMaxWeight); j++) ;

                        if (j != Drones.Count())
                        {
                            Drones[j] = new Drone()
                            {
                                Id = Drones[j].Id,
                                Model = Drones[j].Model,///same Model
                                Battery = Drones[j].Battery,///same Battery
                                MaxWeight = Drones[j].MaxWeight,///same MaxWeight
                                Status = DroneStatuses.Delivery///change the drone's status from avaliable to delivery
                            };
                        }
                        int randomCustomer = R.Next(Customers.Count());
                        Parcels.Add(new Parcel()
                        {
                            Id = IdOfParcel++,///Id equals to IdOfParcle
                            SenderId = Customers[randomCustomer].Id,///Random Id from customers list 
                            TargetId = Customers[(randomCustomer + 1) % Customers.Count()].Id,///Random Id from customers list, cant be equal to sendId
                            Weight = ParcelMaxWeight,///Random MaxWeight
                            DroneId = (j == Drones.Count() ? 0 : Drones[j].Id),///Id from drones's list (only if there is a drone that match the requirements)
                            Priority = EnumExtension.RandomEnumValue<Priorities>(),///Random priority
                            Requested = DateTime.Now,///the person requested the parcel now
                            Scheduled = (j == Drones.Count() ? DateTime.MinValue : DateTime.Now),///time that we assign a drone to the parcel, MinValue - if we didn't find a drone
                            PickedUp = DateTime.MinValue,
                            Delivered = DateTime.MinValue
                        });

                    }

                }

            }
        }
    }
}
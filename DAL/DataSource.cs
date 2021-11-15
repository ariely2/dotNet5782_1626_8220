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
                //id of parcel - number with 8 digit
                public static int IdOfParcel { private set; get; } = 1000000;
                public static double AvailableUse = 10; //an available drone uses 10% battery per kilometer
                public static double LightUse = 15; //a drone carrying light weight uses 15% battery per kilometer
                public static double MediumUse = 20; //a drone carrying medium weight uses 20% battery per kilometer
                public static double HeavyUse = 25; //a drone carrying heavy weight uses 25% battery per kilometer
                public static double ChargeRate = 15; //a drone charges 15% per hour

                private static Random random = new Random();

                /// <summary>
                /// the function Initial the database.
                /// 10 customers, 3 stations, 5 drones and 10 parcels
                /// </summary>
                internal static void Initialize()
                {
                    //initialize 10 customers
                    string[] CNames = new string[] { "Mario", "Carmen", "Tamar", "Daniel", "Adam", "Rosie", "Paul", "Jessie", "Leslie", "Charlie" };
                    for (int i = 0; i < 10; i++)
                        Customers.Add(new Customer()
                        {
                            Id = random.Next(100000000,1000000000),
                            Name = CNames[i],//Name from the array
                            Phone = random.Next(111111, 999999).ToString("000000"),//Random phone number (adding zeroes in the front)
                            Coordinate = new GeoCoordinate()
                            {
                                Longitude = (random.NextDouble() * 260) - 180,//Random Longitude between -180 and 80
                                Latitude = (random.NextDouble() * 180) - 90//Random latitude between -90 and 90
                            }
                        }); 

                    //initialize 3 stations
                    string[] SNames = new string[] { "Florentin", "Mamilla", "Hulon" };
                    for (int i = 0; i < 3; i++)
                        Stations.Add(new Station()
                        {
                            Id = random.Next(100, 1000),
                            Name = SNames[i],//Name from Name array.
                            Coordinate = new GeoCoordinate()
                            {
                                Longitude = (random.NextDouble() * 260) - 180,//Random Longitude between -180 and 80
                                Latitude = (random.NextDouble() * 180) - 90//Random Latitude between -90 and 90
                            },
                            ChargeSlots = random.Next(1, 4)//Random number of available slots between 1 and 3
                        });

                    ///initialize 5 drones
                    string[] MNames = new string[] { "A", "B", "C", "D", "E" };
                    for (int i = 0; i < 5; i++)
                    {
                        Drones.Add(new Drone()
                        {
                            Id = random.Next(10000, 100000),
                            Model = MNames[i],//Name from model array
                            MaxWeight = EnumExtension.RandomEnumValue<WeightCategories>(),//Random MaxWeight
                        });
                    }

                    //initialize 10 parcels
                    for (int i = 0; i < 10; i++)
                    {
                        WeightCategories ParcelMaxWeight = EnumExtension.RandomEnumValue<WeightCategories>();
                        //looking for an avaliable drone with enough battery and can carry the parcel
                        Drone d = Drones.Find(x => /*x.Status == DroneStatuses.Available && x.Battery != 0 && */x.MaxWeight >= ParcelMaxWeight);
                        int randomCustomer = random.Next(Customers.Count());
                        Parcels.Add(new Parcel()
                        {
                            Id = IdOfParcel++,//Id equals to IdOfParcle
                            SenderId = Customers[randomCustomer].Id,//Random Id from customers list 
                            TargetId = Customers[(randomCustomer + 1) % Customers.Count()].Id,//Random Id from customers list, cant be equal to sendId
                            Weight = ParcelMaxWeight,//Random Weight
                            DroneId = (d.Equals(default(Drone)) ? 0 : d.Id),//Id from drones's list (only if there is a drone that match the requirements)
                            Priority = EnumExtension.RandomEnumValue<Priorities>(),//Random priority
                            Requested = DateTime.Now,//the person requested the parcel now
                            Scheduled = (d.Equals(default(Drone)) ? DateTime.MinValue : DateTime.Now),//time that we assign a drone to the parcel, MinValue - if we didn't find a drone
                            PickedUp = DateTime.MinValue,
                            Delivered = DateTime.MinValue
                        });
                        if (!d.Equals(default(Drone)))
                        {
                            Drones.Add(new Drone()
                            {
                                Id = d.Id,//same id
                                Model = d.Model,//same Model
                                //Battery = d.Battery,//same Battery
                                MaxWeight = d.MaxWeight,//same MaxWeight
                                //Status = DroneStatuses.Assigned//change the drone's status from avaliable to delivery
                            });
                            Drones.Remove(d);
                        }
                    }
                }
            }
        }
    }
}
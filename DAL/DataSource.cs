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
                //longitude between 35 to 35.1
                //latitude between 37 to 37.1
                private const double lowerLongitude = 35;
                private const double upperLongitude = 35.1;
                private const double lowerLatitude = 37;
                private const double upperLatitude = 37.1;

                //id of parcel - number with 8 digit
                public static int IdOfParcel {set; get; } = Parcel.LowerBoundId;
                public static double AvailableUse = 1; //an available drone uses 1% battery per kilometer
                public static double LightUse = 2; //a drone carrying light weight uses 2% battery per kilometer
                public static double MediumUse = 2.5; //a drone carrying medium weight uses 2.5% battery per kilometer
                public static double HeavyUse = 3; //a drone carrying heavy weight uses 3% battery per kilometer
                public static double ChargeRate = 25; //a drone charges 25% per hour

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
                    {
                        //check that there is no customer with the same id
                        int id;
                        do
                        {
                            id = random.Next(Customer.LowerBoundId, Customer.UpperBoundId);
                        } while (Customers.Exists(x => x.Id == id));

                        //check that there is no 2 customers with the same phone number
                        string phone = random.Next(111111, 999999).ToString("000000");

                        Customers.Add(new Customer()
                        {
                            Id = id,//calculate before
                            Name = CNames[i],//Name from the array
                            Phone = phone,//calculate before
                            Location = new Location()
                            {
                                Longitude = (random.NextDouble() * 0.1) + lowerLongitude,//Random Longitude between 35 - 35.1
                                Latitude = (random.NextDouble() * 0.1) + lowerLatitude//Random latitude between 37 - 37.1
                            }
                        });
                    }

                    //initialize 3 stations
                    string[] SNames = new string[] { "Florentin", "Mamilla", "Hulon" };
                    for (int i = 0; i < 3; i++)
                    {
                        //check that there is no 2 station with the same id
                        int id;
                        do
                        {
                            id = random.Next(Station.LowerBoundId, Station.UpperBoundId);
                        } while (Stations.Exists(x => x.Id == id));

                        Stations.Add(new Station()
                        {
                            Id = id,
                            Name = SNames[i],//Name from Name array.
                            Location = new Location()
                            {
                                Longitude = (random.NextDouble() * 0.1) + lowerLatitude,//Random Longitude between 35 - 35.1
                                Latitude = (random.NextDouble() * 0.1) + lowerLongitude//Random Latitude between 37 - 37.1
                            },
                            ChargeSlots = random.Next(1, 4)//Random number of available slots between 1 and 3
                        });
                    }

                    //initialize 5 drones
                    //model names
                    string[] MNames = new string[] { "A", "B", "C", "D", "E" };

                    for (int i = 0; i < 5; i++)
                    {
                        //check that there is no 2 drone with the same id
                        int id;
                        do
                        {
                            id = random.Next(Drone.LowerBoundId, Drone.UpperBoundId);
                        } while (Drones.Exists(x => x.Id == id));

                        Drones.Add(new Drone()
                        {
                            Id = id,
                            Model = MNames[i],//Name from model array
                            MaxWeight = EnumExtension.RandomEnumValue<WeightCategories>(),//Random MaxWeight
                        });
                    }

                    //initialize 10 parcels
                    //array to check if we already used this drone
                    bool[] used = new bool[5] { false, false, false, false, false };

                    for (int i = 0; i < 10; i++)
                    {
                        //random weight
                        WeightCategories ParcelMaxWeight = EnumExtension.RandomEnumValue<WeightCategories>();

                        //search for a drone available which can carry the parcel 
                        int j;
                        for (j = 0; j < 5 && !(!used[j] && Drones[j].MaxWeight >= ParcelMaxWeight); j++) ;
                        if(j!=5)
                            used[j] = true;

                        //random picked( true or false)
                        bool picked = (random.Next(2) == 0);
                        //random customer index
                        int randomCustomer = random.Next(Customers.Count());

                        
                        Parcels.Add(new Parcel()
                        {
                            Id = IdOfParcel++,//Id equals to IdOfParcle
                            SenderId = Customers[randomCustomer].Id,//Random Id from customers list 
                            ReceiverId = Customers[(randomCustomer + 1) % Customers.Count()].Id,//Random Id from customers list, cant be equal to sendId
                            Weight = ParcelMaxWeight,
                            DroneId = (j != 5 ? Drones[j].Id : null),
                            Priority = EnumExtension.RandomEnumValue<Priorities>(),//Random priority
                            Requested = DateTime.Now,//the person requested the parcel now
                            Scheduled = (j != 5 ? DateTime.Now : null),
                            PickedUp = (picked && j!=5 ? DateTime.Now : null),
                            Delivered = (j!=5&&picked && random.Next(2) == 0 ? DateTime.Now : null)
                        });
                    }
                }
            }
        }
    }
}
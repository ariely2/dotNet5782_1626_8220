﻿using System;
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
                public static int IdOfParcel {set; get; } = Parcel.LowerBoundId;
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
                    {
                        //check that there is no customer with the same id
                        int id;
                        do
                        {
                            id = random.Next(Customer.LowerBoundId, Customer.UpperBoundId);
                        } while (Customers.Exists(x => x.Id == id));

                        //check that there is no 2 customers with the same phone number
                        string phone;
                        do
                        {
                            phone = random.Next(111111, 999999).ToString("000000");
                        } while (Customers.Exists(x => x.Phone == phone));

                        Customers.Add(new Customer()
                        {
                            Id = id,//calculate before
                            Name = CNames[i],//Name from the array
                            Phone = phone,//calculate before
                            Location = new Location()
                            {
                                Longitude = (random.NextDouble() * 260) - 180,//Random Longitude between -180 and 80
                                Latitude = (random.NextDouble() * 180) - 90//Random latitude between -90 and 90
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
                                Longitude = (random.NextDouble() * 260) - 180,//Random Longitude between -180 and 80
                                Latitude = (random.NextDouble() * 180) - 90//Random Latitude between -90 and 90
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
                            PickedUp = (picked ? DateTime.Now : null),
                            Delivered = (picked && random.Next(2) == 0 ? DateTime.Now : null)
                        });
                    }
                }
            }
        }
    }
}
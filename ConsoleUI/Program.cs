using System;
using System.Collections.Generic;
using IDAL.DalObject;
using IDAL.DO;
namespace ConsoleUI
{
    class Program
    {
        /// <summary>
        /// the function get an int number from user,
        /// and check if it valid (not have letter for example)
        /// </summary>
        /// <returns>
        /// return the input from the user
        /// </returns>
        static int InputInt()
        {
            int num;
            bool valid = false;
            do
            {
                valid = Int32.TryParse(Console.ReadLine(), out num);
                if (!valid)
                {
                    Console.WriteLine("Invalid value,try again");
                }
            } while (!valid);
            return num;
        }

        /// <summary>
        /// the function get an double number from user,
        /// and check if it valid (not have letter for example)
        /// </summary>
        /// <returns>
        /// return the input from the user
        /// </returns>
        static double InputDouble()
        {
            double num;
            bool valid = false;
            do
            {
                valid = Double.TryParse(Console.ReadLine(), out num);
                if (!valid)
                {
                    Console.WriteLine("Invalid value,try again");
                }
            } while (!valid);
            return num;
        }

        /// <summary>
        /// print general option
        /// </summary>
        static void PrintGeneralOption()
        {
            Console.WriteLine(@"choose an option between 1 to 5:
1)Add
2)Update
3)Display
4)Display list
5)exit");
        }
        /// <summary>
        /// main function
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            DalObject DataBase = new DalObject();
            int option;
            do
            {
                PrintGeneralOption();
                option = InputInt();
                Console.Clear();
                switch (option)
                {
                    case ((int)Option.Add):
                        SwitchAddOption();
                        break;
                    case ((int)Option.Update):
                        SwitchUpdateOption();
                        break;
                    case ((int)Option.Display):
                        SwitchDisplayOption();
                        break;
                    case ((int)Option.DisplayList):
                        SwitchDisplayListOption();
                        break;
                    case ((int)Option.Exit):
                        break;
                }
            } while (option != 5);
        }

        /// <summary>
        /// print add options
        /// </summary>
        static void PrintAddOption()
        {
            Console.WriteLine(@"choose an option between 1 to 4:
1)Add station
2)Add Drone
3)Add customer
4)Add parcel");
        }

        /// <summary>
        /// switch about add option
        /// </summary>
        static void SwitchAddOption()
        {
            PrintAddOption();
            int option = InputInt();
            Console.Clear();
            switch (option)
            {

                case ((int)Add.AddStation):

                    Console.WriteLine("Enter Id, StationName, NumberOfAvaliableChargeSlots, Longitude and Latitude");
                    DalObject.AddStation(new Station()
                    {
                        Id = InputInt(),
                        Name = Console.ReadLine(),
                        ChargeSlots = InputInt(),
                        Coordinate = new GeoCoordinate()
                        {
                            Latitude = InputDouble(),
                            Longitude = InputDouble()
                        }
                    });
                    break;

                case ((int)Add.AddDrone):
                    Console.WriteLine("Enter Id, Drone's model, max weight, battery and drone's status");
                    DalObject.AddDrone(new Drone()
                    {
                        Id = InputInt(),
                        Model = Console.ReadLine(),
                        MaxWeight = EnumExtension.InputEnum<WeightCategories>(),
                        Battery = InputInt(),
                        Status = EnumExtension.InputEnum<DroneStatuses>()
                    });
                    break;

                case ((int)Add.AddCustomer):
                    Console.WriteLine("Enter Id, name, phone number, longitude and latitude ");
                    DalObject.AddCustomer(new Customer()
                    {
                        Id = InputInt(),
                        Name = Console.ReadLine(),
                        Phone = Console.ReadLine(),
                        Coordinate = new GeoCoordinate()
                        {
                            Latitude = InputDouble(),
                            Longitude = InputDouble()
                        }
                    });
                    break;

                case ((int)Add.AddParcel):
                    Console.WriteLine("Enter sender id, reciever id, weight, priority, drone id(if not then 0)");
                    DalObject.AddParcel(new Parcel()
                    {
                        SenderId = InputInt(),
                        TargetId = InputInt(),
                        Weight = EnumExtension.InputEnum<WeightCategories>(),
                        Priority = EnumExtension.InputEnum<Priorities>(),
                        DroneId = InputInt(),
                        Requested = DateTime.Now,
                        Delivered = DateTime.MinValue,
                        PickedUp = DateTime.MinValue
                    });
                    break;
                default:
                    Console.WriteLine("Invalid input, try again");
                    break;

            }
        }

        /// <summary>
        /// print update option
        /// </summary>
        static void PrintUpdateOption()
        {
            Console.WriteLine(@"choose an option between 1 to 5:
1)Assign a parcel to a drone
2)Pick up a parcel by a drone
3)deliver a parcel to a customer
4)Send a drone to charge
5)Release a drone from charging");
        }

        /// <summary>
        /// switch about update option
        /// </summary>
        static void SwitchUpdateOption()
        {
            PrintUpdateOption();
            int option = InputInt();
            Console.Clear();
            switch (option)
            {
                case ((int)Update.ConnectParceltoDrone):
                    ConnectParcelToDrone();
                    break;
                case ((int)Update.PickUpParcel):
                    PickUpAParcel();
                    break;
                case ((int)Update.DeliverParcel):
                    DeliverParcel();
                    break;
                case ((int)Update.SendDroneToCharge):
                    SendDroneToCharge();
                    break;
                case ((int)Update.ReleaseDrone):
                    ReleaseDrone();
                    break;
            }
        }


        /// <summary>
        /// print display option
        /// </summary>
        static void PrintDisplayOption()
        {
            Console.WriteLine(@"choose an option between 1 to 4:
1)Display station
2)Display drone
3)Display customer
4)Display parcel
5)Distance from station
6)Distance from customer"
);
        }

        /// <summary>
        /// switch about display option
        /// </summary>
        static void SwitchDisplayOption()
        {
            PrintDisplayOption();
            int option = InputInt();
            Console.Clear();
            switch (option)
            {
                case ((int)Display.DisplayStation):
                    DisplayStation();
                    break;
                case ((int)Display.DisplayDrone):
                    DisplayDrone();
                    break;
                case ((int)Display.DisplayCustomer):
                    DisplayCustomer();
                    break;
                case ((int)Display.DisplayParcel):
                    DisplayParcel();
                    break;
                case ((int)Display.DistanceFromStation):
                    Console.WriteLine("enter longitude, latitude and station id");
                    Console.WriteLine("The distance is: ",
                        DalObject.GetDistanceFromStation(
                            new GeoCoordinate()
                            {
                                Longitude = InputDouble(),
                                Latitude = InputDouble()
                            },
                            InputInt()));//station id
                    break;
                case ((int)Display.DistanceFromCustomer):
                    Console.WriteLine("enter longitude, latitude and customer id");
                    Console.WriteLine("The distance is: ",
                        DalObject.GetDistanceFromCustomer(
                            new GeoCoordinate()
                            {
                                Longitude = InputDouble(),
                                Latitude = InputDouble()
                            },
                            InputInt()));//customer id
                    break;

            }
        }


        /// <summary>
        /// print display list option
        /// </summary>
        static void PrintDisplayListOption()
        {
            Console.WriteLine(@"choose an option between 1 to 6:
1)Display stations list
2)Display Drones list
3)Display customers list
4)Display parcels list
5)Display parcels list that aren't associated with a drone
6)Display stations list with avaliable charging spots");
        }


        /// <summary>
        /// switch display list option
        /// </summary>
        static void SwitchDisplayListOption()
        {
            PrintDisplayListOption();
            int option = InputInt();
            
            Console.Clear();
            switch (option)
            {
                case ((int)DisplayList.DisplayStations):
                    DisplayStationList();
                    break;
                case ((int)DisplayList.DisplayDrones):
                    DisplayDroneList();
                    break;
                case ((int)DisplayList.DisplayCustomers):
                    DisplayCustomerList();
                    break;
                case ((int)DisplayList.DisplayParcels):
                    DisplayParcelList();
                    break;
                case ((int)DisplayList.DisplayUnassignParcles):
                    DisplayUnassignedParcels();
                    break;
                case ((int)DisplayList.DisplayAvailableStations):
                    DisplayAvailableStations();
                    break;
            }
        }

        /// <summary>
        /// The function receives data from the user,
        /// and transmits them to function in DalObject
        /// </summary>
        static void ConnectParcelToDrone()
        {
            Console.WriteLine("Enter Parcel ID and Drone ID");
            DalObject.AssignParcel(InputInt(),///Parcel Id
                InputInt());///Drone Id
        }
        /// <summary>
        /// The function receives data from the user,
        /// and transmits them to function in DalObject
        /// </summary>
        static void PickUpAParcel()
        {
            Console.WriteLine("Enter Parcel ID");
            DalObject.PickUpParcel(InputInt());///Parcel Id
        }
        /// <summary>
        /// The function receives data from the user,
        /// and transmits them to function in DalObject
        /// </summary>
        static void DeliverParcel()
        {
            Console.WriteLine("Enter Parcel ID");
            DalObject.DeliverParcel(InputInt());//parcel Id
        }
        /// <summary>
        /// The function receives data from the user,
        /// and transmits them to function in DalObject
        /// </summary>
        static void SendDroneToCharge()
        {
            Console.WriteLine("Enter Drone ID");
            int DroneId = InputInt();
            Console.WriteLine("Enter an ID of an available station:");
            DisplayAvailableStations();
            int StationId = InputInt();
            DalObject.ChargeDrone(DroneId, StationId);
        }
        /// <summary>
        /// The function receives data from the user,
        /// and transmits them to function in DalObject
        /// </summary>
        static void ReleaseDrone()
        {
            Console.WriteLine("Enter Drone ID");
            DalObject.ReleaseDrone(InputInt());///Drone Id
        }
        /// <summary>
        /// The function receives data from the user,
        /// and transmits them to function in DalObject
        /// </summary>
        static void DisplayStation()
        {
            Console.WriteLine("Enter Station ID");
            DalObject.DisplayStation(InputInt());///Station Id
        }
        /// <summary>
        /// The function receives data from the user,
        /// and transmits them to function in DalObject
        /// </summary>
        static void DisplayDrone()
        {
            Console.WriteLine("Enter Drone ID");
            DalObject.DisplayDrone(InputInt());///Drone Id
        }
        /// <summary>
        /// The function receives data from the user,
        /// and transmits them to function in DalObject
        /// </summary>
        static void DisplayCustomer()
        {
            Console.WriteLine("Enter Customer ID");
            DalObject.DisplayCustomer(InputInt());///Customer Id
        }
        /// <summary>
        /// The function receives data from the user,
        /// and transmits them to function in DalObject
        /// </summary>
        static void DisplayParcel()
        {
            Console.WriteLine("Enter Parcel ID");
            DalObject.DisplayParcel(InputInt());///Parcel Id
        }
        /// <summary>
        /// The function prints all stations that it get from the function in DalObject
        /// </summary>
        static void DisplayStationList()
        {
            foreach (Station s in DalObject.StationsList())
                Console.WriteLine(s);
        }
        /// <summary>
        /// The function prints all Drones that it get from the function in DalObject
        /// </summary>
        static void DisplayDroneList()
        {
            foreach (Drone d in DalObject.DronesList())
                Console.WriteLine(d);
        }
        /// <summary>
        /// The function prints all customers that it get from the function in DalObject
        /// </summary>
        static void DisplayCustomerList()
        {
            foreach (Customer c in DalObject.CustomersList())
                Console.WriteLine(c);
        }
        /// <summary>
        /// The function prints all parcels that it get from the function in DalObject
        /// </summary>
        static void DisplayParcelList()
        {
            foreach (Parcel s in DalObject.ParcelList())
                Console.WriteLine(s);
        }
        /// <summary>
        /// The function prints all parcels that it get from the function in DalObject
        /// </summary>
        static void DisplayUnassignedParcels()
        {
            foreach (Parcel p in DalObject.UnassignedParcels())
                Console.WriteLine(p);
        }

        /// <summary>
        /// The function prints all stations that it get from the function in DalObject
        /// </summary>
        static void DisplayAvailableStations()
        {
            foreach (Station s in DalObject.GetAvailableStations())
                Console.WriteLine(s);
        }
    }
}

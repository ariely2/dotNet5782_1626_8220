using System;
using System.Collections.Generic;
using IDAL.DalObject;
using IDAL.DO;
namespace ConsoleUI
{
    class Program
    {
        DalObject obj = new DalObject();
        static void PrintGeneralOption()
        {
            Console.WriteLine(@"choose an option between 1 to 5:
1)Add
2)Update
3)Display
4)Display list
5)exit");
        }
        static void PrintAddOption()
        {
            Console.WriteLine(@"choose an option between 1 to 4:
1)Add station
2)Add Drone
3)Add customer
4)Add parcel");
        }
        static void PrintUpdateOption()
        {
            Console.WriteLine(@"choose an option between 1 to 5:
1)Assign a parcel to a drone
2)Pick up a parcel by a drone
3)deliver a parcel to a customer
4)Send a drone to charge
5)Release a drone from charging");
        }
        static void PrintDisplayOption()
        {
            Console.WriteLine(@"choose an option between 1 to 4:
1)Display station
2)Display drone
3)Display customer
4)Display parcel");
        }
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
        static void SwitchAddOption()
        {
            PrintAddOption();
            int option = InputInt();
            Console.Clear();
            switch (option)
            {
                ///add a station
                case 1:

                    Console.WriteLine("Enter Id, StationName, NumberOfAvaliableChargeSlots, Longitude and Latitude");
                    DalObject.AddStation(new Station()
                    {
                        Id = InputInt(),
                        Name = Console.ReadLine(),
                        ChargeSlots = InputInt(),
                        Longitude = InputDouble(),
                        Latitude = InputDouble()
                    });
                    break;
                ///add a drone
                case 2:
                    Console.WriteLine("Enter Id, Drone's model, max weight, battery and drone's status");
                    DalObject.AddDrone(new Drone()
                    {
                        Id = InputInt(),
                        Model = Console.ReadLine(),
                        MaxWeight = InputEnum<WeightCategories>(),
                        Battery = InputInt(),
                        Status = InputEnum<DroneStatuses>()
                    });
                    break;
                ///add a customer
                case 3:
                    Console.WriteLine("Enter Id, name, phone number, longitude and latitude ");
                    DalObject.AddCustomer(new Customer()
                    {
                        Id = InputInt(),
                        Name = Console.ReadLine(),
                        Phone = Console.ReadLine(),
                        Longitude = InputDouble(),
                        Latitude = InputDouble()
                    });
                    break;
                ///add a parcel
                case 4:
                    Console.WriteLine("Enter sender id, reciever id, weight, priority, drone id(if not then 0)");
                    DalObject.AddParcel(new Parcel()
                    {
                        SenderId = InputInt(),
                        TargetId = InputInt(),
                        Weight = InputEnum<WeightCategories>(),
                        Priority = InputEnum<Priorities>(),
                        DroneId = InputInt(),
                        Requested = DateTime.Now,
                        Delivered = DateTime.MinValue,
                        PickedUp = DateTime.MinValue
                    });
                    break;
            }
        }
        static void SwitchUpdateOption()
        {
            PrintUpdateOption();
            int option = InputInt();
            Console.Clear();
            switch (option)
            {
                case 1:
                    ConnectParcelToDrone();
                    break;
                case 2:
                    PickUpAParcel();
                    break;
                case 3:
                    DeliverParcel();
                    break;
                case 4:
                    SendDroneToCharge();
                    break;
                case 5:
                    ReleaseDrone();
                    break;
            }
        }
        static void SwitchDisplayOption()
        {
            PrintDisplayOption();
            int option = InputInt();
            Console.Clear();
            switch (option)
            {
                case 1:
                    DisplayStation();
                    break;
                case 2:
                    DisplayDrone();
                    break;
                case 3:
                    DisplayCustomer();
                    break;
                case 4:
                    DisplayParcel();
                    break;

            }
        }
        static void SwitchDisplayListOption()
        {
            PrintDisplayListOption();
            int option = InputInt();
            
            Console.Clear();
            switch (option)
            {
                case 1:
                    DisplayStationList();
                    break;
                case 2:
                    DisplayDroneList();
                    break;
                case 3:
                    DisplayCustomerList();
                    break;
                case 4:
                    DisplayParcelList();
                    break;
                case 5:
                    DisplayUnassignedParcels();
                    break;
                case 6:
                    DisplayAvailableStations();
                    break;
            }
        }
        //need to fix this function
        static T InputEnum<T>() where T : struct, Enum
        {
            T result;
            bool valid = false;
            do
            {
                valid = Enum.TryParse<T>(Console.ReadLine(), out result);
                if (valid && !Enum.IsDefined(typeof(T), result))
                    valid = false;
                if (!valid)
                    Console.WriteLine("Invalid Enum, try again");
            } while (!valid);
            return result;
        }
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
        static void ConnectParcelToDrone()
        {
            Console.WriteLine("Enter Parcel ID and Drone ID");
            DalObject.AssignParcel(InputInt(),///Parcel Id
                InputInt());///Drone Id
        }
        static void PickUpAParcel()
        {
            Console.WriteLine("Enter Parcel ID");
            DalObject.PickUpParcel(InputInt());///Parcel Id
        }
        static void DeliverParcel()
        {
            Console.WriteLine("Enter Parcel ID");
            DalObject.DeliverParcel(InputInt());///parcel Id
        }
        static void SendDroneToCharge()
        {
            Console.WriteLine("Enter Drone ID");
            int DroneId = InputInt();
            Console.WriteLine("Choose an available station:");
            DisplayAvailableStations();
            string name = Console.ReadLine();
            DalObject.ChargeDrone(DroneId, name);
        }
        static void ReleaseDrone()
        {
            Console.WriteLine("Enter Drone ID");
            DalObject.ReleaseDrone(InputInt());///Drone Id
        }
        static void DisplayStation()
        {
            Console.WriteLine("Enter Station ID");
            DalObject.DisplayStation(InputInt());///Station Id
        }
        static void DisplayDrone()
        {
            Console.WriteLine("Enter Drone ID");
            DalObject.DisplayDrone(InputInt());///Drone Id
        }
        static void DisplayCustomer()
        {
            Console.WriteLine("Enter Customer ID");
            DalObject.DisplayCustomer(InputInt());///Customer Id
        }
        static void DisplayParcel()
        {
            Console.WriteLine("Enter Parcel ID");
            DalObject.DisplayParcel(InputInt());///Parcel Id
        }
        static void DisplayStationList()
        {
            foreach (Station s in DalObject.StationsList())
            {
                Console.WriteLine(s);
            }
        }
        static void DisplayDroneList()
        {
            foreach (Drone d in DalObject.DronesList())
            {
                Console.WriteLine(d);
            }
        }
        static void DisplayCustomerList()
        {
            foreach (Customer c in DalObject.CustomersList())
            {
                Console.WriteLine(c);
            }
        }
        static void DisplayParcelList()
        {
            foreach (Parcel s in DalObject.ParcelList())
            {
                Console.WriteLine(s);
            }
        }
        static void DisplayUnassignedParcels()
        {
            foreach (Parcel p in DalObject.UnassignedParcels())
            {
                Console.WriteLine(p);
            }
        }
        static void DisplayAvailableStations()
        {
            foreach (Station s in DalObject.GetAvailableStations())
            {
                Console.WriteLine(s);
            }
        }
        static void Main(string[] args)
        {
            DalObject a = new DalObject();
            int option;
            do
            {
                PrintGeneralOption();
                option = InputInt();
                Console.Clear();
                switch (option)
                {
                    case 1:
                        SwitchAddOption();
                        break;
                    case 2:
                        SwitchUpdateOption();
                        break;
                    case 3:
                        SwitchDisplayOption();
                        break;
                    case 4:
                        SwitchDisplayListOption();
                        break;
                    case 5:
                        break;
                }
            } while (option != 5);
        }
    }
}

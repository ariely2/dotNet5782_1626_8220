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
1)Add staion
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
4)send a drone to charge
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
            bool valid;
            PrintAddOption();
            int option;
            Int32.TryParse(Console.ReadLine(), out option);
            Console.Clear();
            switch (option)
            {
                //
                case 1:

                    Console.WriteLine("Enter Id, StationName, NumberOfAvaliableChargeSlots, Longitude and Latitude");
                    DalObject.AddStation(new Station()
                    {
                        Id = InputInt(),
                        Name = Console.ReadLine(),
                        ChargeSlots = InputInt(),
                        Longitude = InputInt(),
                        Latitude = InputInt()
                    }) ;
                    break;
                //
                case 2:
                    Console.WriteLine("Enter Id, Drone's model, max weight, battery and drone's status");
                    Enum.TryParse(Console.ReadLine(), out WeightCategories weight);
                    Int32.TryParse(Console.ReadLine(), out int battery); //why not get with InputInt()?
                    Enum.TryParse(Console.ReadLine(), out DroneStatuses status);
                    DalObject.AddDrone(new Drone() 
                    { 
                        Id = InputInt(),
                        Model = Console.ReadLine(),

                    }
                        ); 
                    break;
                case 3:
                    Console.WriteLine("Enter Id, name, phone number, longitude and latitude ");
                    Int32.TryParse(Console.ReadLine(), out int customerId);
                    string name = Console.ReadLine();
                    string phone = Console.ReadLine();
                    Int32.TryParse(Console.ReadLine(), out double longitude);//cant get double
                    Int32.TryParse(Console.ReadLine(), out double latitude);
                    DalObject.AddCustomer(customerId, name, phone, longitude, latitude);
                    break;
                case 4:
                    Console.WriteLine("Enter sender id, reciever id, weight, priority, drone id(if not then 0)");
                    Int32.TryParse(Console.ReadLine(), out int senderId);
                    Int32.TryParse(Console.ReadLine(), out int recieverId);
                    Enum.TryParse(Console.ReadLine(), out weight);
                    Enum.TryParse(Console.ReadLine(), out Priorities priority);
                    Int32.TryParse(Console.ReadLine(), out int droneId);
                    DalObject.AddParcel(senderId, recieverId, weight, priority, droneId);
                    break;
            }
        }
        static void SwitchUpdateOption()
        {
            PrintUpdateOption();
            int option;
            Int32.TryParse(Console.ReadLine(), out option);
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
            int option;
            Int32.TryParse(Console.ReadLine(), out option);
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
            int option;
            Int32.TryParse(Console.ReadLine(), out option);
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
        static void ConnectParcelToDrone()
        {
            Console.WriteLine("Enter Parcel ID and Drone ID");
            Int32.TryParse(Console.ReadLine(), out int ParcelID);
            Int32.TryParse(Console.ReadLine(), out int DroneID);
            DalObject.AssignParcel(ParcelID, DroneID);
        }
        static void PickUpAParcel()
        {
            Console.WriteLine("Enter Parcel ID");
            Int32.TryParse(Console.ReadLine(), out int ParcelID);
            DalObject.PickUpParcel(ParcelID);
        }
        static void DeliverParcel()
        {
            Console.WriteLine("Enter Parcel ID");
            Int32.TryParse(Console.ReadLine(), out int ParcelID);
            DalObject.DeliverParcel(ParcelID);
        }
        static void SendDroneToCharge()
        {
            Console.WriteLine("Enter Drone ID");
            Int32.TryParse(Console.ReadLine(), out int DroneID);
            Console.WriteLine("Choose an available station:");
            DisplayAvailableStations();
            string name = Console.ReadLine();
            DalObject.ChargeDrone(DroneID, name);
        }
        static void ReleaseDrone()
        {
            Console.WriteLine("Enter Drone ID");
            Int32.TryParse(Console.ReadLine(), out int id);
            DalObject.ReleaseDrone(id);
        }
        static void DisplayStation()
        {
            Console.WriteLine("Enter Station ID");
            int id = InputInt();
            DalObject.DisplayStation(id);
        }
        static void DisplayDrone()
        {
            Console.WriteLine("Enter Drone ID");
            int id = InputInt();
            DalObject.DisplayDrone(id);
        }
        static void DisplayCustomer()
        {
            Console.WriteLine("Enter Customer ID");
            int id = InputInt();
            DalObject.DisplayCustomer(id);
        }
        static void DisplayParcel()
        {
            Console.WriteLine("Enter Parcel ID");
            int id = InputInt();
            DalObject.DisplayParcel(id);
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
            int option;
            do
            {
                PrintGeneralOption();
                Int32.TryParse(Console.ReadLine(), out option);
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

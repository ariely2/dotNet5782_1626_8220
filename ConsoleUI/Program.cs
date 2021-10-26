using System;
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
            PrintAddOption();
            int option;
            Int32.TryParse(Console.ReadLine(), out option);
            Console.Clear();
            switch (option)
            {
                case 1:
                    Console.WriteLine("Enter Id, StationName, NumberOfAvaliableChargeSlots, Longitude and Latitude");
                    Int32.TryParse(Console.ReadLine(), out int stationId);
                    string name = Console.ReadLine();
                    Int32.TryParse(Console.ReadLine(), out int chargeSlots);
                    Int32.TryParse(Console.ReadLine(), out int longitude);
                    Int32.TryParse(Console.ReadLine(), out int latitude);           
                    DalObject.AddStation(stationId, name,chargeSlots,longitude,latitude);
                    break;
                case 2:
                    Console.WriteLine("Enter Id, Drone's model, max weight, battery and drone's status");
                    Int32.TryParse(Console.ReadLine(), out int droneId);
                    name = Console.ReadLine();
                    Enum.TryParse(Console.ReadLine(), out WeightCategories weight);
                    Int32.TryParse(Console.ReadLine(), out int battery);
                    Enum.TryParse(Console.ReadLine(), out DroneStatuses status);
                    DalObject.AddDrone(droneId, name, weight, battery, status); ;
                    break;
                case 3:
                    Console.WriteLine("Enter Id, name, phone number, longitude and latitude ");
                    Int32.TryParse(Console.ReadLine(), out int customerId);
                    name = Console.ReadLine();
                    string phone = Console.ReadLine();
                    Int32.TryParse(Console.ReadLine(), out longitude);
                    Int32.TryParse(Console.ReadLine(), out latitude);
                    DalObject.AddCustomer(customerId, name, phone, longitude, latitude);
                    break;
                case 4:
                    Console.WriteLine("Enter sender id, reciever id, weight, priority, drone id(if not then 0)");
                    Int32.TryParse(Console.ReadLine(),  out int senderId);
                    Int32.TryParse(Console.ReadLine(), out int recieverId);
                    Enum.TryParse(Console.ReadLine(), out weight);
                    Enum.TryParse(Console.ReadLine(), out Priorities priority);
                    Int32.TryParse(Console.ReadLine(), out droneId);
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
                    SendToDroneCharge();
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
                    DisplayStationsList();
                    break;
                case 2:
                    DisplayDronesList();
                    break;
                case 3:
                    DisplayCustomersList();
                    break;
                case 4:
                    DisplayParcelList();
                    break;
                case 5:
                    DisplayParcelListWithoutDrone();
                    break;
                case 6:
                    DisplayStationAvaliablecharge();
                    break;
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

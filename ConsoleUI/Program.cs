using System;
using System.Collections.Generic;
using Dal;
using DO;
namespace ConsoleUI
{
    class Program
    {

        #region enum
        public enum AddOption { Station = 1, Drone, Customer, Parcel }
        public enum UpdateOption { ConnectParceltoDrone = 1, PickUpParcel, DeliverParcel, SendDroneToCharge, ReleaseDrone }
        public enum RequestOption { Station = 1, Drone, Customer, Parcel, DistanceFromStation, DistanceFromCustomer }
        public enum RequestListOption { Stations = 1, Drones, Customers, Parcels, UnassignParcles, AvailableStations }
        public enum Option { Add = 1, Update, Request, RequestList, Exit }
        #endregion enum

        //the data base isntance
        private static DalApi.IDal dal;

        #region InputMethods

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

        #endregion InputMethods
        #region PrintOptions
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


        #endregion PrintOption
        #region Switches
        /// <summary>
        /// main function
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            dal = DalApi.DalFactory.GetDal();
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
                    case ((int)Option.Request):
                        SwitchDisplayOption();
                        break;
                    case ((int)Option.RequestList):
                        SwitchDisplayListOption();
                        break;
                    case ((int)Option.Exit):
                        break;
                }
                Console.Clear();
            } while (option != 5);
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

                case ((int)AddOption.Station):

                    AddStation();
                    break;

                case ((int)AddOption.Drone):
                    AddDrone();
                    break;

                case ((int)AddOption.Customer):
                    AddCustomer();
                    break;

                case ((int)AddOption.Parcel):
                    AddParcel();
                    break;
                default:
                    Console.WriteLine("Invalid input, try again");
                    break;

            }
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
                case ((int)UpdateOption.ConnectParceltoDrone):
                    ConnectParcelToDrone();
                    break;
                case ((int)UpdateOption.PickUpParcel):
                    PickUpAParcel();
                    break;
                case ((int)UpdateOption.DeliverParcel):
                    DeliverParcel();
                    break;
                case ((int)UpdateOption.SendDroneToCharge):
                    SendDroneToCharge();
                    break;
                case ((int)UpdateOption.ReleaseDrone):
                    ReleaseDrone();
                    break;
            }
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
                case ((int)RequestOption.Station):
                    Request<Station>();
                    break;
                case ((int)RequestOption.Drone):
                    Request<Drone>();
                    break;
                case ((int)RequestOption.Customer):
                    Request<Customer>();
                    break;
                case ((int)RequestOption.Parcel):
                    Request<Parcel>();
                    break;
                case ((int)RequestOption.DistanceFromStation):
                    GetDistanceFrom<Station>();
                    break;
                case ((int)RequestOption.DistanceFromCustomer):
                    GetDistanceFrom<Customer>();
                    break;
            }
            Console.Write("Press <Enter> to continue");
            while (Console.ReadKey().Key != ConsoleKey.Enter) ;
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
                case ((int)RequestListOption.Stations):
                    RequestList<Station>();
                    break;
                case ((int)RequestListOption.Drones):
                    RequestList<Drone>();
                    break;
                case ((int)RequestListOption.Customers):
                    RequestList<Customer>();
                    break;
                case ((int)RequestListOption.Parcels):
                    RequestList<Parcel>();
                    break;
                case ((int)RequestListOption.UnassignParcles):
                    DisplayUnassignedParcels();
                    break;
                case ((int)RequestListOption.AvailableStations):
                    DisplayAvailableStations();
                    break;
            }
            Console.Write("Press <Enter> to continue");
            while (Console.ReadKey().Key != ConsoleKey.Enter) ;
        }
        #endregion Switches
        #region AddMethod
        static void AddStation()
        {
            Console.WriteLine("Enter Id, StationName, NumberOfAvaliableChargeSlots, Longitude and Latitude");
            dal.Create<Station>(new Station()
            {
                Id = InputInt(),
                Name = Console.ReadLine(),
                ChargeSlots = InputInt(),
                Location = new Location()
                {
                    
                    Longitude = InputDouble(),
                    Latitude = InputDouble()
                }
            });
        }

        static void AddDrone()
        {
            Console.WriteLine("Enter Id, Drone's model, max weight, battery and drone's status");
            dal.Create<Drone>(new Drone()
            {
                Id = InputInt(),
                Model = Console.ReadLine(),
                MaxWeight = EnumExtension.InputEnum<WeightCategories>(),
            });
        }

        static void AddCustomer()
        {
            Console.WriteLine("Enter Id, name, phone number, longitude and latitude ");
            dal.Create<Customer>(new Customer()
            {
                Id = InputInt(),
                Name = Console.ReadLine(),
                Phone = Console.ReadLine(),
                Location = new Location()
                {
                    Latitude = InputDouble(),
                    Longitude = InputDouble()
                }
            });
        }

        static void AddParcel()
        {
            Console.WriteLine("Enter sender id, reciever id, weight, priority, drone id(if not then 0)");
            dal.Create<Parcel>(new Parcel()
            {
                SenderId = InputInt(),
                ReceiverId = InputInt(),
                Weight = EnumExtension.InputEnum<WeightCategories>(),
                Priority = EnumExtension.InputEnum<Priorities>(),
                DroneId = InputInt(),
                Requested = DateTime.Now,
                Scheduled = null,
                Delivered = null,
                PickedUp = null
            });
        }




        #endregion AddMethod
        #region UpdateMethods
        /// <summary>
        /// The function receives data from the user,
        /// and transmits them to function in DalObject
        /// </summary>
        static void ConnectParcelToDrone()
        {
            Console.WriteLine("Enter Parcel ID, Drone Id");
            dal.AssignParcel(InputInt(),InputInt()); //Parcel Id
        }
        /// <summary>
        /// The function receives data from the user,
        /// and transmits them to function in DalObject
        /// </summary>
        static void PickUpAParcel()
        {
            Console.WriteLine("Enter Parcel ID");
            dal.PickUpParcel(InputInt());//Parcel Id
        }
        /// <summary>
        /// The function receives data from the user,
        /// and transmits them to function in DalObject
        /// </summary>
        static void DeliverParcel()
        {
            Console.WriteLine("Enter Parcel ID");
            dal.DeliverParcel(InputInt());//parcel Id
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
            dal.ChargeDrone(DroneId, StationId);
        }
        /// <summary>
        /// The function receives data from the user,
        /// and transmits them to function in DalObject
        /// </summary>
        static void ReleaseDrone()
        {
            Console.WriteLine("Enter Drone ID");
            dal.ReleaseDrone(InputInt());//getting Drone Id and sending it to the releaseDrone function
        }
        #endregion UpdateMethods
        #region RequestMethods
        static void Request<T>() where T : struct
        {
            Console.WriteLine($"Enter {typeof(T).Name} ID");
            Console.WriteLine(dal.Request<T>(InputInt()));
        }
        static void GetDistanceFrom<T>() where T : struct
        {
            Console.WriteLine("enter longitude, latitude and " + typeof(T).Name + "id");
            Console.WriteLine("The distance is: {0} km",
                dal.GetDistanceFrom<T>(
                    new Location()
                    {
                        Longitude = InputDouble(),
                        Latitude = InputDouble()
                    },
                    InputInt()));
        }
        #endregion RequestMethods
        #region RequestListMethods
        static void RequestList<T>() where T : struct
        {
            foreach (T t in dal.RequestList<T>())
                Console.WriteLine(t);
        }

        /// <summary>
        /// The function prints all parcels that it get from the function in DalObject
        /// </summary>
        static void DisplayUnassignedParcels()
        {
            foreach (Parcel p in dal.RequestList<Parcel>(x=>x.DroneId==null))
                Console.WriteLine(p);
        }

        /// <summary>
        /// The function prints all stations that it get from the function in DalObject
        /// </summary>
        static void DisplayAvailableStations()
        {
            foreach (Station s in dal.RequestList<Station>(x => x.ChargeSlots != 0))
                Console.WriteLine(s);
        }
    }
}

    #endregion RequestListMethods



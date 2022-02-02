using System;
using BO;
namespace ConsoleUI_BL
{

    class Program
    {
        //enum option for the switches
        public enum AddOption { Station = 1, Drone, Customer, Parcel }
        public enum UpdateOption { ConnectParceltoDrone = 1, PickUpParcel, DeliverParcel, SendDroneToCharge, ReleaseDrone,DroneData,StationData,CustomerData }
        public enum RequestOption { Station = 1, Drone, Customer, Parcel}
        public enum RequestListOption { Stations = 1, Drones, Customers, Parcels, UnassignParcles, AvailableStations }
        public enum Option { Add = 1, Update, Request, RequestList, Exit }

        private static BlApi.IBL bl;
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
3)Request
4)Request list
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
            Console.WriteLine(@"choose an option between 1 to 8:
1)Assign a parcel to a drone
2)Pick up a parcel by a drone
3)deliver a parcel to a customer
4)Send a drone to charge
5)Release a drone from charging
6)Update Drone data
7)Update Station data
8)Update Customer data");
        }
        /// <summary>
        /// print display option
        /// </summary>
        static void PrintRequestOption()
        {
            Console.WriteLine(@"choose an option between 1 to 4:
1)Request station
2)Request drone
3)Request customer
4)Request parcel
5)Distance from station
6)Distance from customer"
);
        }

        /// <summary>
        /// print display list option
        /// </summary>
        static void PrintRequestListOption()
        {
            Console.WriteLine(@"choose an option between 1 to 6:
1)Request stations list
2)Request Drones list
3)Request customers list
4)Request parcels list
5)Request parcels list that aren't associated with a drone
6)Request stations list with avaliable charging spots");
        }


        #endregion PrintOption
        #region Main
        /// <summary>
        /// main function
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            bl = BlApi.BlFactory.GetBl();
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
                        SwitchRequestOption();
                        break;
                    case ((int)Option.RequestList):
                        SwitchRequestListOption();
                        break;
                    case ((int)Option.Exit):
                        break;
                    default:
                        Console.WriteLine("Invalid Input, try again\n");
                        break;
                }
            } while (option != 5);
        }
        #endregion Main
        #region Add
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
                    Console.WriteLine("Invalid Input, try again");
                    break;
            }
        }
        static void AddStation()
        {
            Console.WriteLine("Enter Id, Station Name, Number Of Charge Slots, Longitude and Latitude");
            try
            {
                bl.Create<Station>(new Station()
                {
                    Id = InputInt(),
                    Name = Console.ReadLine(),
                    AvailableSlots = InputInt(),
                    Location = new Location()
                    {
                        Latitude = InputDouble(),
                        Longitude = InputDouble()
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "Try again\n");
            } 
        }

        static void AddDrone()
        {
            Console.WriteLine("Enter Id, Model, Max Weight, Initial Station");
            try
            {
                bl.Create<Drone>(new Drone()
                {
                    Id = InputInt(),
                    Model = Console.ReadLine(), //check if a string is entered? also in customer and station name and phone
                    MaxWeight = EnumExtension.InputEnum<WeightCategories>(),
                    Location = bl.Request<Station>(InputInt()).Location
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "Try again\n");
            }
        }

        static void AddCustomer()
        {
            Console.WriteLine("Enter Id, Name, Phone, Longitude and Latitude");
            try
            {
                bl.Create<Customer>(new Customer()
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "Try again\n");
            }
        }

        static void AddParcel()
        {
            Console.WriteLine("Enter Sender Id, Receiver Id, Weight, Priority");
            try
            {
                bl.Create<Parcel>(new Parcel()
                {
                    Sender = new CustomerParcel() { Id = InputInt() },
                    Receiver = new CustomerParcel() { Id = InputInt() },
                    Weight = EnumExtension.InputEnum<WeightCategories>(),
                    Priority = EnumExtension.InputEnum<Priorities>(),
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "Try again\n");
            }
        }
        #endregion Add
        #region Update
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
                    DroneAssign();
                    break;
                case ((int)UpdateOption.PickUpParcel):
                    PickUpParcel();
                    break;
                case ((int)UpdateOption.DeliverParcel):
                    DeliverParcel();
                    break;
                case ((int)UpdateOption.SendDroneToCharge):
                    ChargeDrone();
                    break;
                case ((int)UpdateOption.ReleaseDrone):
                    DroneRelease();
                    break;
                case ((int)UpdateOption.DroneData):
                    DroneUpdate();
                    break;
                case ((int)UpdateOption.StationData):
                    StationUpdate();
                    break;
                case ((int)UpdateOption.CustomerData):
                    CustomerUpdate();
                    break;
            }
        }

        static void DroneAssign()
        {
            Console.WriteLine("Enter drone Id");
            try
            {
                int id = InputInt();
                bl.AssignDrone(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "Try again\n");
            }
        }
        static void PickUpParcel()
        {
            Console.WriteLine("Enter drone Id");
            try
            {
                int id = InputInt();
                bl.PickUp(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "Try again\n");
            }
        }
        static void DeliverParcel()
        {
            Console.WriteLine("Enter drone Id");
            try
            {
                int id = InputInt();
                bl.Deliver(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "Try again\n");
            }
        }
        static void ChargeDrone()
        {
            Console.WriteLine("Enter Id");
            try
            {
                int id = InputInt();
                bl.SendDroneToCharge(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "Try again\n");
            }
        }
        static void DroneRelease()
        {
            Console.WriteLine("Enter Id");
            try
            {
                int id = InputInt();
                bl.ReleaseDrone(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "Try again\n");
            }
        }

        static void DroneUpdate()
        {
            Console.WriteLine("Enter Id, new Model");
            try
            {
                int id = InputInt();
                string model = Console.ReadLine();
                bl.UpdateDrone(id, model); 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "Try again\n");
            }
        }

        static void StationUpdate()
        {
            Console.WriteLine("Enter Id,  then enter new model and/or new number of charge slots ");
            try
            {
                int id = InputInt();
                int? slots = null;
                string model = Console.ReadLine();
                string s = Console.ReadLine();
                if (!String.IsNullOrWhiteSpace(s)) //if slots number was entered
                    slots = int.Parse(s);
                if (String.IsNullOrWhiteSpace(model)) //if model was not entered
                    model = null;
                bl.UpdateStation(id, model, slots);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "Try again\n");
            }
        }
        static void CustomerUpdate()
        {
            Console.WriteLine("Enter Id,  then enter new name and/or new phone number");
            try
            {
                int id = InputInt();
                string name = Console.ReadLine();
                string phone = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(name)) //if name was not entered
                    name = null;
                if (String.IsNullOrWhiteSpace(phone)) //if phone was not entered
                    phone = null;
                bl.UpdateCustomer(id, name, phone);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "Try again\n");
            }
        }

        #endregion Update
        #region Request
        /// <summary>
        /// switch about display option
        /// </summary>
        static void SwitchRequestOption()
        {
            PrintRequestOption();
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
            }
        }

        static void Request<T>() where T : class
        {
            Console.WriteLine("enter id");
            try {
                Console.WriteLine(bl.Request<T>(InputInt()));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message + "try again");
            }
            }
        #endregion Request
        #region RequestList
        /// <summary>
        /// switch display list option
        /// </summary>
        static void SwitchRequestListOption()
        {
            PrintRequestListOption();
            int option = InputInt();

            Console.Clear();
            switch (option)
            {
                case ((int)RequestListOption.Stations):
                    RequestList<StationToList>();
                    break;
                case ((int)RequestListOption.Drones):
                    RequestList<DroneToList>();
                    break;
                case ((int)RequestListOption.Customers):
                    RequestList<CustomerToList>();
                    break;
                case ((int)RequestListOption.Parcels):
                    RequestList<ParcelToList>();
                    break;
                case ((int)RequestListOption.UnassignParcles):
                    RequestUnassignedParcels();
                    break;
                case ((int)RequestListOption.AvailableStations):
                    RequestAvailableStations();
                    break;
            }
        }

        static void RequestList<T>() where T : class
        {
            var arr = bl.RequestList<T>();
            if (arr == null)
                return;
            foreach (T t in arr)
            {
                Console.WriteLine(t);
            }
        }
        static void RequestUnassignedParcels()
        {
            foreach (ParcelToList p in bl.RequestList<ParcelToList>())
                if (bl.Request<Parcel>(p.Id).Drone == null)
                    Console.WriteLine(p);
        }
        static void RequestAvailableStations()
        {
            foreach (StationToList s in bl.RequestList<StationToList>())
                if (s.Available != 0)
                    Console.WriteLine(s);
        }
        #endregion RequestList
    }
}
using System;
using IBL;
using IBL.BO;
namespace ConsoleUI_BL
{

    class Program
    {
        private static IBL.IBL bl = new BL();
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
            int option;
            do
            {
                PrintGeneralOption();
                option = InputInt();
                Console.Clear();
                switch (option)
                {
                    case ((int)EnumOption.Add):
                        SwitchAddOption();
                        break;
                    case ((int)EnumOption.Update):
                        SwitchUpdateOption();
                        break;
                    case ((int)EnumOption.Request):
                        SwitchRequestOption();
                        break;
                    case ((int)EnumOption.RequestList):
                        SwitchRequestListOption();
                        break;
                    case ((int)EnumOption.Exit):
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

                case ((int)EnumAdd.Station):
                    AddStation();
                    break;
                case ((int)EnumAdd.Drone):
                    AddDrone();
                    break;
                case ((int)EnumAdd.Customer):
                    AddCustomer();
                    break;
                case ((int)EnumAdd.Parcel):
                    AddParcel();
                    break;
                default:
                    Console.WriteLine("Invalid Input, try again");
                    break;

            }
        }
        static void AddStation()
        {
            Console.WriteLine("Enter Id, StationName, NumberOfChargeSlots, Longitude and Latitude");
            try
            {
                bl.Create<Station>(new Station()
                {
                    Id = InputInt(),
                    Name = Console.ReadLine(),
                    AvailableSlots = InputInt(),
                    location = new Location()
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
                    MaxWeight = EnumExtension.InputEnum<WeightCategories>(),
                    Model = Console.ReadLine(),
                    Location = bl.Request<Station>(InputInt()).location
                });
            }
            catch (Exception ex)
            {
                throw new IdExistException("Drone id exists", ex);
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
                    location = new Location()
                    {
                        Latitude = InputDouble(),
                        Longitude = InputDouble()
                    }
                });
            }
            catch (Exception ex)
            {
                throw new IdExistException("Customer id exists", ex);
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
            //wouldn't be exception,the user doesn't choose the id
            catch (Exception ex)
            {
                throw new IdExistException("????????????", ex);
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
                case ((int)EnumUpdate.ConnectParceltoDrone):

                    break;
                case ((int)EnumUpdate.PickUpParcel):

                    break;
                case ((int)EnumUpdate.DeliverParcel):

                    break;
                case ((int)EnumUpdate.SendDroneToCharge):

                    break;
                case ((int)EnumUpdate.ReleaseDrone):
                    break;
                case ((int)EnumUpdate.DroneData):
                    DroneUpdate();
                    break;
                case ((int)EnumUpdate.StationData):
                    break;
                case ((int)EnumUpdate.CustomerData):
                    break;
            }
        }

        static void DroneUpdate()
        {
            Console.WriteLine("Enter Id, new Model");
            try
            {
                int id = InputInt();
                string model = Console.ReadLine();
                // bl.UpdateDrone 
            }
            catch (Exception ex)
            {
                throw new IdExistException("Drone doesn't exist", ex);
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
                case ((int)EnumRequest.Station):
                    Request<Station>();
                    break;
                case ((int)EnumRequest.Drone):
                    Request<Drone>();
                    break;
                case ((int)EnumRequest.Customer):
                    Request<Customer>();
                    break;
                case ((int)EnumRequest.Parcel):
                    Request<Parcel>();
                    break;
            }
        }

        static void Request<T>() where T : class
        {
            Console.WriteLine("enter id");
            Console.WriteLine(bl.Request<T>(InputInt()));
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
                case ((int)EnumRequestList.Stations):
                    RequestList<StationToList>();
                    break;
                case ((int)EnumRequestList.Drones):
                    RequestList<DroneToList>();
                    break;
                case ((int)EnumRequestList.Customers):
                    RequestList<CustomerToList>();
                    break;
                case ((int)EnumRequestList.Parcels):
                    RequestList<ParcelToList>();
                    break;
                case ((int)EnumRequestList.UnassignParcles):
                    RequestUnassignedParcels();
                    break;
                case ((int)EnumRequestList.AvailableStations):
                    RequestAvailableStations();
                    break;
            }
        }

        static void RequestList<T>() where T : class
        {
            foreach (T t in bl.RequestList<T>())
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
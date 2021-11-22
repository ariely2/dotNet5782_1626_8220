using System;
using IBL;
using IBL.BO;
namespace ConsoleUI_BL
{

    class Program
    {
        private static IBL.IBL BL = new BL();
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
        #region Switches
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
                }
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

                case ((int)Add.Station):

                    Console.WriteLine("Enter Id, StationName, NumberOfChargeSlots, Longitude and Latitude");
                    try
                    {
                        BL.Create<Station>(new Station()
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
                        throw new IdExistException("Station id exists", ex);
                    }
                    break;

                case ((int)Add.Drone):
                    Console.WriteLine("Enter Id, Model, Max Weight, Initial Station");
                    try {
                        BL.Create<Drone>(new Drone()
                        {
                            Id = InputInt(),
                            MaxWeight = EnumExtension.InputEnum<WeightCategories>(),
                            Model = Console.ReadLine(),
                            Location = Request<Station>(InputInt)
                        }) ;
                            }
                    catch (Exception ex)
                    {
                        throw new IdExistException("Drone id exists", ex);
                    }
                    break;

                case ((int)Add.Customer):
                    Console.WriteLine("Enter Id, Name, Phone, Longitude and Latitude");
                    try
                    {
                        BL.Create<Customer>(new Customer()
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

                    break;

                case ((int)Add.Parcel):
                    Console.WriteLine("Enter Sender Id, Receiver Id, Weight, Priority");
                    try
                    {
                        BL.Create<Parcel>(new Parcel()
                        {
                            
                        });
                    }
                    //wouldn't be exception,the user doesn't choose the id
                    catch (Exception ex)
                    {
                        throw new IdExistException("????????????", ex); 
                    }
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
                case ((int)Update.ConnectParceltoDrone):
                   
                    break;
                case ((int)Update.PickUpParcel):
                    
                    break;
                case ((int)Update.DeliverParcel):
                    
                    break;
                case ((int)Update.SendDroneToCharge):
                   
                    break;
                case ((int)Update.ReleaseDrone):
                    break;
                case ((int)Update.DroneData):
                    break;
                case ((int) Update.StationData):
                    break;
                case ((int)Update.CustomerData):
                    break;
            }
        }




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
                case ((int)Request.Station):

                    break;
                case ((int)Request.Drone):

                    break;
                case ((int)Request.Customer):

                    break;
                case ((int)Request.Parcel):

                    break;
            }
        }

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
                    RequestUnassignParcel();
                    break;
                case ((int)EnumRequestList.AvailableStations):
                    RequestAvailbleStation();
                    break;

            }
        }
        #endregion Switches
        #region RequestMethod
        static void RequestList<T>()where T:class
        {
            foreach( T t in BL.RequestList<T>())
            {
                Console.WriteLine(t);
            }
        }

        #endregion RequestMethod
    }
}
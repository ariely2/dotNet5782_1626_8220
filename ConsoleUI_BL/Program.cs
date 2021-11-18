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
                        throw new ExistObjectIdException("exist station id", ex);
                    }
                    break;

                case ((int)Add.AddDrone):
                    Console.WriteLine("Enter Id, Model, ");
                    BL.Create<Drone>(new Drone()
                    {
                        Id = InputInt(),
                        MaxWeight = EnumExtension.InputEnum<WeightCategories>(),
                        s
                    })

                    break;

                case ((int)Add.AddCustomer):
                    
                    break;

                case ((int)Add.AddParcel):
                    
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
                case ((int)Display.DisplayStation):
                   
                    break;
                case ((int)Display.DisplayDrone):
                    
                    break;
                case ((int)Display.DisplayCustomer):
                    
                    break;
                case ((int)Display.DisplayParcel):
                    
                    break;
                case ((int)Display.DistanceFromStation):
                    
                    break;
                case ((int)Display.DistanceFromCustomer):
                    
                    break;

            }
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
                    
                    break;
                case ((int)DisplayList.DisplayDrones):
                    
                    break;
                case ((int)DisplayList.DisplayCustomers):
                   
                    break;
                case ((int)DisplayList.DisplayParcels):
                    
                    break;
                case ((int)DisplayList.DisplayUnassignParcles):
                    
                    break;
                case ((int)DisplayList.DisplayAvailableStations):
                   
                    break;
            }
        }
        #endregion Switches
    }
}
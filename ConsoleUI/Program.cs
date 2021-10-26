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

                    break;
                case 2:

                    break;
                case 3:

                    break;
                case 4:

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

                    break;
                case 2:

                    break;
                case 3:

                    break;
                case 4:

                    break;
                case 5:
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

                    break;
                case 2:

                    break;
                case 3:

                    break;
                case 4:

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

                    break;
                case 2:

                    break;
                case 3:

                    break;
                case 4:

                    break;
                case 5:
                    break;
                case 6:
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

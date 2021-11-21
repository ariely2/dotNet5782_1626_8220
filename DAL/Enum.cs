using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    namespace DO
    {
        /// <summary>
        /// class for enum function
        /// </summary>
        public static class EnumExtension
        {
            private static Random random = new Random();

            /// <summary>
            /// create a random enum which type is T
            /// was taken from: https://stackoverflow.com/questions/3132126
            /// </summary>
            /// <typeparam name="T">represent the enum type, for example WeightCategories</typeparam>
            /// <returns>return a rnadom value of enum T</returns>
            public static T RandomEnumValue<T>()
            {
                var v = Enum.GetValues(typeof(T));
                return (T)v.GetValue(random.Next(v.Length));
            }


            /// <summary>
            /// get an enum type from user,
            /// and check if the input is valid and exist in T
            /// </summary>
            /// <typeparam name="T">T represent enum type. for example: WeightCategories</type>
            /// <returns>the function return the input from the user</returns>
            public static T InputEnum<T>() where T : struct, Enum
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
        }
        /// <summary>
        /// All the enums required for the exercise plus enum for the switches.
        /// </summary>
        public enum WeightCategories
        { 
            Light,
            Medium,
            Heavy
        }
        public enum Priorities
        {
            Normal,
            Fast,
            Emergency
        }
        public enum Add 
        { 
            Station = 1,
            Drone,
            Customer,
            Parcel
        }
        public enum Update 
        { 
            ConnectParceltoDrone = 1,
            PickUpParcel,
            DeliverParcel,
            SendDroneToCharge,
            ReleaseDrone
        }
        public enum Request 
        { 
            Station = 1,
            Drone,
            Customer,
            Parcel,
            DistanceFromStation,
            DistanceFromCustomer
        }
        public enum RequestList
        {
            Stations = 1,
            Drones,
            Customers,
            Parcels,
            UnassignParcles,
            AvailableStations
        }
        public enum Option
        {
            Add = 1,
            Update,
            Request,
            RequestList,
            Exit
        }
    }
}

using System;

namespace IBL
{
    namespace BO
    {
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

            /// <summary>
            /// the function return the status of the parcel
            /// </summary>
            /// <param name="d">delivered time</param>
            /// <param name="p">picked up time</param>
            /// <param name="s">scheduled time</param>
            /// <param name="r">requested time</param>
            /// <returns>return the status</returns>
            public static ParcelStatuses GetStatus(DateTime? d, DateTime? p, DateTime? s, DateTime? r)
            {
                if (d != null)
                    return ParcelStatuses.Delivered;
                else if (p != null)
                    return ParcelStatuses.PickedUp;
                else if (s != null)
                    return ParcelStatuses.Assigned;
                return ParcelStatuses.Created;
            }
        }
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
        public enum DroneStatuses
        {
            Available,
            Maintenance,
            Delivery
        }
        public enum ParcelStatuses
        {
            Created,
            Assigned,
            PickedUp,
            Delivered
        }
        public enum ShippingStatus
        {
            Waiting,
            EnRoute
        }
        public enum EnumParcelDeliver
        {
            PickUp,
            Delivery
        }
        public enum EnumAdd
        {
            Station = 1,
            Drone,
            Customer,
            Parcel
        }
        public enum EnumUpdate
        {
            ConnectParceltoDrone = 1,
            PickUpParcel,
            DeliverParcel,
            SendDroneToCharge,
            ReleaseDrone,
            DroneData,
            StationData,
            CustomerData
        }
        public enum EnumRequest
        {
            Station = 1,
            Drone,
            Customer,
            Parcel,
            DistanceFromStation,
            DistanceFromCustomer
        }
        public enum EnumRequestList
        {
            Stations = 1,
            Drones,
            Customers,
            Parcels,
            UnassignParcles,
            AvailableStations
        }
        public enum EnumOption
        {
            Add = 1,
            Update,
            Request,
            RequestList,
            Exit
        }
    }
}

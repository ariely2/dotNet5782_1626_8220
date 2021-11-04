using System;

namespace IBL
{
    namespace BO
    {
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
            Assigned,
            Delivery
        }
        public enum Add
        {
            AddStation = 1,
            AddDrone,
            AddCustomer,
            AddParcel
        }
        public enum Update
        {
            ConnectParceltoDrone = 1,
            PickUpParcel,
            DeliverParcel,
            SendDroneToCharge,
            ReleaseDrone
        }
        public enum Display
        {
            DisplayStation = 1,
            DisplayDrone,
            DisplayCustomer,
            DisplayParcel,
            DistanceFromStation,
            DistanceFromCustomer
        }
        public enum DisplayList
        {
            DisplayStations = 1,
            DisplayDrones,
            DisplayCustomers,
            DisplayParcels,
            DisplayUnassignParcles,
            DisplayAvailableStations
        }
        public enum Option
        {
            Add = 1,
            Update,
            Display,
            DisplayList,
            Exit
        }
    }
}

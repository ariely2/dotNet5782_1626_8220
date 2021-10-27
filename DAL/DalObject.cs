﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL.DO;
namespace IDAL
{
    namespace DalObject
    {
        public class DalObject
        {
            public DalObject()
            {
                DataSource.Config.Initialize();
            }
            public static void AddStation(Station station)
            {
                DataSource.Stations.Add(station);
            }
            public static void AddDrone(int id,string name, WeightCategories weight, int battery, DroneStatuses status)
            {
                DataSource.Drones.Add(new Drone()
                {
                    Id = id,
                    Model = name,
                    MaxWeight = weight,
                    Battery = battery,
                    Status = status
                }) ;
            }
            public static void AddCustomer(int id, string name, string phone, int longtitude, int latitude)
            {
                DataSource.Customers.Add(new Customer()
                {
                    Id = id,
                    Name = name,
                    Phone = phone,
                    Longitude = longtitude,
                    Latitude = latitude
                });
            }
            public static void AddParcel(int senderId, int targetId, WeightCategories weight, Priorities priority, int droneId)
            {
                DataSource.Parcels.Add(new Parcel() { SenderId = senderId, TargetId = targetId, Weight = weight, Priority = priority });
            }
            public static void AssignParcel(int ParcelId, int DroneId)
            {
                Parcel P = DataSource.Parcels.Find(x => x.Id == ParcelId);
                Drone D = DataSource.Drones.Find(x => x.Id == DroneId);
                //if (P.Id == ParcelId && D.Id == DroneId)   do we need this?
                P.DroneId = DroneId;
                P.Scheduled = DateTime.Now;
                Console.WriteLine("Parcel assigned to drone.");
            }
            public static void PickUpParcel(int ParcelId)
            {
                Parcel P = DataSource.Parcels.Find(x => x.Id == ParcelId);
                Drone D = DataSource.Drones.Find(x => x.Id == P.DroneId);
                D.Status = DroneStatuses.Delivery;
                P.PickedUp = DateTime.Now;
            }
            public static void DeliverParcel(int ParcelId)
            {
                Parcel P = DataSource.Parcels.Find(x => x.Id == ParcelId);
                Drone D = DataSource.Drones.Find(x => x.Id == P.DroneId);
                double B = D.Battery;
                //D.Status = DroneStatuses.;
                P.Delivered = DateTime.Now;
            }
            public static List<Station> StationsList()
            {
                return DataSource.Stations.ToList();
            }
        }
    }
}


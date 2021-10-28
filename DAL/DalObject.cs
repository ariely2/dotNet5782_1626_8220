using System;
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
            public static void AddDrone(Drone drone)
            {
                DataSource.Drones.Add(drone);
            }
            public static void AddCustomer(Customer customer)
            {
                DataSource.Customers.Add(customer);
            }
            public static void AddParcel(Parcel parcel)
            {
                parcel.Id = DataSource.Config.IdOfParcel++;
                parcel.Scheduled = parcel.DroneId == 0 ? DateTime.MinValue : parcel.Requested;
                DataSource.Parcels.Add(parcel);
            }
            public static void AssignParcel(int ParcelId, int DroneId)
            {
                Parcel P = DataSource.Parcels.Find(x => x.Id == ParcelId);
                Drone D = DataSource.Drones.Find(x => x.Id == DroneId);
                //if (P.Id == ParcelId && D.Id == DroneId)   do we need this?
                P.DroneId = DroneId;
                P.Scheduled = DateTime.Now;
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
                D.Status = DroneStatuses.Available;
                P.Delivered = DateTime.Now;
            }
            public static void ChargeDrone(int DroneID, string name)
            {
                Drone D = DataSource.Drones.Find(x => x.Id == DroneID);
                Station s = DataSource.Stations.Find(x => x.Name == name);
                s.ChargeSlots--;
                D.Status = DroneStatuses.Maintaince;
                DataSource.DroneCharge.Add(new DroneCharge() { DroneId = DroneID, StationId = s.Id });
            }
            public static List<Station> GetAvailableStations()
            {
                List<Station> Available = new List<Station>(StationsList());
                foreach(Station s in StationsList())
                {
                    if (s.ChargeSlots == 0)
                        Available.Remove(s);
                }
                return Available;
            }
            public static List<Parcel> UnassignedParcels()
            {
                List<Parcel> Unassigned = new List<Parcel>(ParcelList());
                //List<Parcel> Unassigned = ParcelList();
                foreach (Parcel p in ParcelList())
                {
                    if (p.DroneId !=0)
                        Unassigned.Remove(p);
                }
                return Unassigned;
            }

            public static List<Station> StationsList()
            {
                return DataSource.Stations.ToList();
            }
            public static void ReleaseDrone(int DroneID)
            {
                Drone D = DataSource.Drones.Find(x => x.Id == DroneID);
                D.Status = DroneStatuses.Available;
                DroneCharge C = DataSource.DroneCharge.Find(x => x.DroneId == DroneID);
                Station S = DataSource.Stations.Find(x => x.Id == C.StationId);
                S.ChargeSlots++;
            }
            public static List<Drone> DronesList()
            {
                return DataSource.Drones.ToList();
            }
            public static List<Customer> CustomersList()
            {
                return DataSource.Customers.ToList();
            }
            public static List<Parcel> ParcelList()
            {
                return DataSource.Parcels.ToList();
            }
            public static void DisplayStation(int id)
            {
                bool check = DataSource.Stations.Exists(x => x.Id == id);
                if (check)
                {
                    Station S = DataSource.Stations.Find(x => x.Id == id);
                    Console.WriteLine(S);
                }
                else
                    Console.WriteLine("There isn't a Station with the entered ID.");
            }
            public static void DisplayDrone(int id)
            {
                bool check = DataSource.Drones.Exists(x => x.Id == id);
                if (check)
                {
                    Drone D = DataSource.Drones.Find(x => x.Id == id);
                    Console.WriteLine(D);
                }
                   else
                    Console.WriteLine("There isn't a drone with the entered ID.");

            }
            public static void DisplayCustomer(int id)
            {
                bool check = DataSource.Customers.Exists(x => x.Id == id);
                if (check)
                {
                    Customer C = DataSource.Customers.Find(x => x.Id == id);
                    Console.WriteLine(C);
                }
                else
                    Console.WriteLine("There isn't a customer with the entered ID.");
            }
            public static void DisplayParcel(int id)
            {
                bool check = DataSource.Parcels.Exists(x => x.Id == id);
                if (check)
                {
                    Parcel P = DataSource.Parcels.Find(x => x.Id == id);
                    Console.WriteLine(P);
                }
                else
                    Console.WriteLine("There isn't a parcel with the entered ID.");
            }
        }
    }
}


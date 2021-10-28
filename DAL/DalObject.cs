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
            public static void Replace<T>(T Item, int index, List<T> list)
            {
                list.RemoveAt(index);
                list.Insert(index, Item);
            }
            public static void AssignParcel(int ParcelId, int DroneId)
            {
                Parcel P = DataSource.Parcels.Find(x => x.Id == ParcelId);
                Drone D = DataSource.Drones.Find(x => x.Id == DroneId);
                D.Status = DroneStatuses.Assigned;
                P.DroneId = DroneId;
                P.Scheduled = DateTime.Now;
                Replace(P, DataSource.Parcels.FindIndex(x => x.Id == ParcelId), DataSource.Parcels);
                Replace(D, DataSource.Drones.FindIndex(x => x.Id == DroneId), DataSource.Drones);
            }
            public static void PickUpParcel(int ParcelId)
            {
                Parcel P = DataSource.Parcels.Find(x => x.Id == ParcelId);
                Drone D = DataSource.Drones.Find(x => x.Id == P.DroneId);
                D.Status = DroneStatuses.Delivery; 
                P.PickedUp = DateTime.Now;
                Replace(P, DataSource.Parcels.FindIndex(x => x.Id == ParcelId), DataSource.Parcels);
                Replace(D, DataSource.Drones.FindIndex(x => x.Id == D.Id), DataSource.Drones);
            }
            public static void DeliverParcel(int ParcelId)
            {
                Parcel P = DataSource.Parcels.Find(x => x.Id == ParcelId);
                Drone D = DataSource.Drones.Find(x => x.Id == P.DroneId);
                D.Status = DroneStatuses.Available;
                P.Delivered = DateTime.Now; //change droneid to zero? or change function of parcels not assigned
                Replace(P, DataSource.Parcels.FindIndex(x => x.Id == ParcelId), DataSource.Parcels);
                Replace(D, DataSource.Drones.FindIndex(x => x.Id == D.Id), DataSource.Drones);
            }
            public static void ChargeDrone(int DroneId, int StationId)
            {
                Drone D = DataSource.Drones.Find(x => x.Id == DroneId);
                Station S = DataSource.Stations.Find(x => x.Id == StationId);
                S.ChargeSlots--;
                D.Status = DroneStatuses.Maintaince;
                DataSource.DroneCharge.Add(new DroneCharge() { DroneId = DroneId, StationId = S.Id });
                Replace(D, DataSource.Drones.FindIndex(x => x.Id == DroneId), DataSource.Drones);
                Replace(S, DataSource.Parcels.FindIndex(x => x.Id == StationId), DataSource.Stations);
            }
            public static List<Station> GetAvailableStations()
            {
                List<Station> Available = new List<Station>(StationsList());
                foreach (Station s in StationsList())
                {
                    if (s.ChargeSlots == 0)
                        Available.Remove(s);
                }
                return Available;
            }
            public static List<Parcel> UnassignedParcels()
            {
                List<Parcel> Unassigned = new List<Parcel>(ParcelList());
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
            public static void ReleaseDrone(int DroneId)
            {
                Drone D = DataSource.Drones.Find(x => x.Id == DroneId);
                D.Status = DroneStatuses.Available;
                DroneCharge C = DataSource.DroneCharge.Find(x => x.DroneId == D.Id);
                Station S = DataSource.Stations.Find(x => x.Id == C.StationId);
                S.ChargeSlots++;
                Replace(D, DataSource.Drones.FindIndex(x => x.Id == DroneId), DataSource.Drones);
                Replace(S, DataSource.Stations.FindIndex(x => x.Id == S.Id), DataSource.Stations);
                Replace(C, DataSource.DroneCharge.FindIndex(x => x.DroneId == D.Id), DataSource.DroneCharge);
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
                Station S = DataSource.Stations.Find(x => x.Id == id);
                Console.WriteLine(S);
            }
            public static void DisplayDrone(int id)
            {
                Drone D = DataSource.Drones.Find(x => x.Id == id);
                Console.WriteLine(D);
            }
            public static void DisplayCustomer(int id)
            {
                Customer C = DataSource.Customers.Find(x => x.Id == id);
                Console.WriteLine(C);
            }
            public static void DisplayParcel(int id)
            {
                Parcel P = DataSource.Parcels.Find(x => x.Id == id);
                Console.WriteLine(P);
            }
        }
    }
}


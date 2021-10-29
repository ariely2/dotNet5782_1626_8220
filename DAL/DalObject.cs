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
			/// <summary>
			/// constructor
			/// called the function initialze to initial the database
			/// </summary>
			public DalObject()
			{
				DataSource.Config.Initialize();
			}

			/// <summary>
			/// add station to the list
			/// </summary>
			/// <param name="station">station to add</param>
			public static void AddStation(Station station)
			{
				DataSource.Stations.Add(station);
			}

			/// <summary>
			/// add drone to the list
			/// </summary>
			/// <param name="drone">drone to add</param>
			public static void AddDrone(Drone drone)
			{
				DataSource.Drones.Add(drone);
			}

			/// <summary>
			/// add customer to the list
			/// </summary>
			/// <param name="customer">customer to add</param>
			public static void AddCustomer(Customer customer)
			{
				DataSource.Customers.Add(customer);
			}

			/// <summary>
			/// add parcel to the list
			/// </summary>
			/// <param name="parcel">parcel to add</param>
			public static void AddParcel(Parcel parcel)
			{
				parcel.Id = DataSource.Config.IdOfParcel++;
				parcel.Scheduled = parcel.DroneId == 0 ? DateTime.MinValue : parcel.Requested;
				DataSource.Parcels.Add(parcel);
			}
			
			/// <summary>
			/// replace the value at index index with the item T
			/// </summary>
			/// <typeparam name="T">represent an instance of struct T</typeparam>
			/// <param name="Item">Item to add to the list</param>
			/// <param name="index">index to remove </param>
			/// <param name="list">list of T</param>
			public static void Replace<T>(T Item, int index, List<T> list)
			{
			
				list.RemoveAt(index);
				list.Insert(index, Item);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="ParcelId"></param>
			/// <param name="DroneId"></param>
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

			/// <summary>
			/// 
			/// </summary>
			/// <param name="ParcelId"></param>
			public static void PickUpParcel(int ParcelId)
			{
				Parcel P = DataSource.Parcels.Find(x => x.Id == ParcelId);
				Drone D = DataSource.Drones.Find(x => x.Id == P.DroneId);
				D.Status = DroneStatuses.Delivery; 
				P.PickedUp = DateTime.Now;
				Replace(P, DataSource.Parcels.FindIndex(x => x.Id == ParcelId), DataSource.Parcels);
				Replace(D, DataSource.Drones.FindIndex(x => x.Id == D.Id), DataSource.Drones);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="ParcelId"></param>
			public static void DeliverParcel(int ParcelId)
			{
				Parcel P = DataSource.Parcels.Find(x => x.Id == ParcelId);
				Drone D = DataSource.Drones.Find(x => x.Id == P.DroneId);
				D.Status = DroneStatuses.Available;
				P.Delivered = DateTime.Now; //change droneid to zero? or change function of parcels not assigned
				Replace(P, DataSource.Parcels.FindIndex(x => x.Id == ParcelId), DataSource.Parcels);
				Replace(D, DataSource.Drones.FindIndex(x => x.Id == D.Id), DataSource.Drones);
			}
			
			/// <summary>
			/// 
			/// </summary>
			/// <param name="DroneId"></param>
			/// <param name="StationId"></param>
			public static void ChargeDrone(int DroneId, int StationId)
			{
				Drone D = DataSource.Drones.Find(x => x.Id == DroneId);
				Station S = DataSource.Stations.Find(x => x.Id == StationId);
				S.ChargeSlots--;
				D.Status = DroneStatuses.Maintenance;
				DataSource.DroneCharges.Add(new DroneCharge() { DroneId = DroneId, StationId = S.Id });
				Replace(D, DataSource.Drones.FindIndex(x => x.Id == DroneId), DataSource.Drones);
				Replace(S, DataSource.Parcels.FindIndex(x => x.Id == StationId), DataSource.Stations);
			}

			/// <summary>
			/// find station with available charge slots
			/// </summary>
			/// <returns>list of station with chargeSlots >0</returns>
			public static List<Station> GetAvailableStations()
			{
				List<Station> Available = new List<Station>(StationsList());
				foreach (Station s in StationsList())
				{
					if (s.ChargeSlots == 0)
						Available.Remove(s);
					Available.RemoveAll(x => x.ChargeSlots == 0);
				}
	
				return Available;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
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

			//need to fix
			/// <summary>
			/// The function releases the drone from the station where it is charged
			/// </summary>
			/// <param name="DroneId">id of drone to release</param>
			public static void ReleaseDrone(int DroneId)
			{
				Drone D = DataSource.Drones.Find(x => x.Id == DroneId);
				D.Status = DroneStatuses.Available;
				DroneCharge C = DataSource.DroneCharges.Find(x => x.DroneId == D.Id);
				Station S = DataSource.Stations.Find(x => x.Id == C.StationId);
				S.ChargeSlots++;
				Replace(D, DataSource.Drones.FindIndex(x => x.Id == DroneId), DataSource.Drones);
				Replace(S, DataSource.Stations.FindIndex(x => x.Id == S.Id), DataSource.Stations);
				DataSource.DroneCharges.Remove(C);
			}

			/// <summary>
			/// the function return list of Station
			/// </summary>
			/// <returns>list of Station</returns>
			public static List<Station> StationsList()
			{
				return DataSource.Stations.ToList();
			}

			/// <summary>
			/// the function return list of Drone
			/// </summary>
			/// <returns>list of Drone</returns>
			public static List<Drone> DronesList()
			{
				return DataSource.Drones.ToList();
			}

			/// <summary>
			/// the function return list of Customer
			/// </summary>
			/// <returns>list of Customer</returns>
			public static List<Customer> CustomersList()
			{
				return DataSource.Customers.ToList();
			}

			/// <summary>
			/// the function return list of Parcel
			/// </summary>
			/// <returns>list of Parcel</returns>
			public static List<Parcel> ParcelList()
			{
				return DataSource.Parcels.ToList();
			}

			/// <summary>
			/// return Station with the same id as the parameter
			/// </summary>
			/// <param name="id">id of station</param>
			public static Station DisplayStation(int id)
			{
				Station S = DataSource.Stations.Find(x => x.Id == id);
				return S;
			}

			/// <summary>
			/// return Drone with the same id as the parameter id
			/// </summary>
			/// <param name="id">id of drone</param>
			public static Drone DisplayDrone(int id)
			{
				Drone D = DataSource.Drones.Find(x => x.Id == id);
				return D;
			}

			/// <summary>
			/// return Customer with the same id as the parameter id
			/// </summary>
			/// <param name="id">id of customer</param>
			public static Customer DisplayCustomer(int id)
			{
				Customer C = DataSource.Customers.Find(x => x.Id == id);
				return C;
			}

			/// <summary>
			/// return parcel with the same id as the parameter id
			/// </summary>
			/// <param name="id">id of parcel</param>
			public static Parcel DisplayParcel(int id)
			{
				Parcel P = DataSource.Parcels.Find(x => x.Id == id);
				return P;
			}
		}
	}
}


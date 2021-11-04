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

        public class DalObject : IDal
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
            public void AddStation(Station station)
            {
                DataSource.Stations.Add(station);
            }

            /// <summary>
            /// add drone to the list
            /// </summary>
            /// <param name="drone">drone to add</param>
            public void AddDrone(Drone drone)
            {
                DataSource.Drones.Add(drone);
            }

            /// <summary>
            /// add customer to the list
            /// </summary>
            /// <param name="customer">customer to add</param>
            public void AddCustomer(Customer customer)
            {
                DataSource.Customers.Add(customer);
            }

            /// <summary>
            /// add parcel to the list
            /// </summary>
            /// <param name="parcel">parcel to add</param>
            public void AddParcel(Parcel parcel)
            {
                parcel.Id = DataSource.Config.IdOfParcel;
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
            public void Replace<T>(T Item, int index, List<T> list)
            {

                list.RemoveAt(index);
                list.Insert(index, Item);
            }

            /// <summary>
            /// the function reponsible for assign a drone to a parcel
            /// </summary>
            /// <param name="ParcelId">id of the parcel</param>
            /// <param name="DroneId">id of the drone</param>
            public void AssignParcel(int ParcelId)
            {
                Parcel p = DataSource.Parcels.Find(x => x.Id == ParcelId);
                Drone d = DataSource.Drones.Find(x => x.MaxWeight >= p.Weight /*&& x.Status==DroneStatuses.Available*/);
                //d.Status = DroneStatuses.Assigned;
                p.DroneId = d.Id;
                p.Scheduled = DateTime.Now;
                int a = DataSource.Parcels.FindIndex(x => x.Id == ParcelId);
                int b = DataSource.Drones.FindIndex(x => x.Id == d.Id);
                if (a < 0 || b < 0)
                    return;
                Replace(p, a, DataSource.Parcels);
                Replace(d, b, DataSource.Drones);
            }

            /// <summary>
            /// the function responsible for pick up a parcel by a drone
            /// </summary>
            /// <param name="ParcelId">id of the parcel</param>
            public void PickUpParcel(int ParcelId)
            {
                Parcel p = DataSource.Parcels.Find(x => x.Id == ParcelId);
                Drone d = DataSource.Drones.Find(x => x.Id == p.DroneId);
                //d.Status = DroneStatuses.Delivery;
                p.PickedUp = DateTime.Now;
                int a = DataSource.Parcels.FindIndex(x => x.Id == ParcelId);
                int b = DataSource.Drones.FindIndex(x => x.Id == d.Id);
                if (a < 0 || b < 0)
                    return;
                Replace(p, a, DataSource.Parcels);
                Replace(d, b, DataSource.Drones);
            }

            /// <summary>
            /// the function responsible for deliver a parcel to a customer
            /// </summary>
            /// <param name="ParcelId">id of the parcel</param>
            public void DeliverParcel(int ParcelId)
            {
                Parcel p = DataSource.Parcels.Find(x => x.Id == ParcelId);
                Drone d = DataSource.Drones.Find(x => x.Id == p.DroneId);
                int a = DataSource.Parcels.FindIndex(x => x.Id == ParcelId);
                int b = DataSource.Drones.FindIndex(x => x.Id == d.Id);
                if (a < 0 || b < 0)
                    return;
                //d.Status = DroneStatuses.Available;
                p.Delivered = DateTime.Now;
                Replace(p, a, DataSource.Parcels);
                Replace(d, b, DataSource.Drones);
            }

            /// <summary>
            /// the function charge a drone in a station
            /// </summary>
            /// <param name="DroneId">id of the drone</param>
            /// <param name="StationId">id of the station</param>
            public void ChargeDrone(int DroneId, int StationId)
            {
                Drone d = DataSource.Drones.Find(x => x.Id == DroneId);
                Station s = DataSource.Stations.Find(x => x.Id == StationId);
                s.ChargeSlots--;
                //d.Status = DroneStatuses.Maintenance;
                int a = DataSource.Drones.FindIndex(x => x.Id == d.Id);
                int b = DataSource.Stations.FindIndex(x => x.Id == s.Id);
                if (a < 0 || b < 0)
                    return;
                DataSource.DroneCharges.Add(new DroneCharge() { DroneId = DroneId, StationId = s.Id });
                Replace(d, a, DataSource.Drones);
                Replace(s, b, DataSource.Stations);
            }

			/// <summary>
			/// find station with available charge slots
			/// </summary>
			/// <returns>list of station with chargeSlots >0</returns>
			public IEnumerable<Station> GetAvailableStations()
			{
				return (IEnumerable<Station>)DataSource.Stations.FindAll(s => s.ChargeSlots != 0);
			}

			/// <summary>
			/// The function returns parcels without a drone assigned with them
			/// </summary>
			/// <returns>list of parcels</returns>
			public IEnumerable<List<Parcel>> UnassignedParcels()
			{
				return (IEnumerable<List<Parcel>>)DataSource.Parcels.FindAll(x => x.DroneId == 0);
			}

			/// <summary>
			/// The function releases the drone from the station where it is charged
			/// </summary>
			/// <param name="DroneId">id of drone to release</param>
			public void ReleaseDrone(int DroneId)
			{
				Drone d = DataSource.Drones.Find(x => x.Id == DroneId);
				//d.Status = DroneStatuses.Available;
				DroneCharge c = DataSource.DroneCharges.Find(x => x.DroneId == d.Id);
				Station s = DataSource.Stations.Find(x => x.Id == c.StationId);
				s.ChargeSlots++;
				int a = DataSource.Drones.FindIndex(x => x.Id == d.Id);
				int b = DataSource.Stations.FindIndex(x => x.Id == s.Id);
				if (a < 0 || b < 0 || !DataSource.DroneCharges.Exists(x => x.DroneId == d.Id))
					return;
				Replace(d, a, DataSource.Drones);
				Replace(s, b, DataSource.Stations);
				DataSource.DroneCharges.Remove(c);
			}

			/// <summary>
			/// the function return list of Station
			/// </summary>
			/// <returns>list of Station</returns>
			public IEnumerable<List<Station>> StationsList()
			{
				return (IEnumerable<List<Station>>)DataSource.Stations.ToList();
			}

			/// <summary>
			/// the function return list of Drone
			/// </summary>
			/// <returns>list of Drone</returns>
			public IEnumerable<List<Drone>> DronesList()
            {
                return (IEnumerable<Drone>)DataSource.Drones;
            }

			/// <summary>
			/// the function return list of Customer
			/// </summary>
			/// <returns>list of Customer</returns>
			public IEnumerable<List<Customer>> CustomersList()
            {
                return (IEnumerable<Customer>)DataSource.Customers;
            }

			/// <summary>
			/// the function return list of Parcel
			/// </summary>
			/// <returns>list of Parcel</returns>
			public IEnumerable<List<Parcel>> ParcelList()
            {
                return (IEnumerable<Parcel>)DataSource.Parcels;
            }

            /// <summary>
            /// return Station with the same id as the parameter
            /// </summary>
            /// <param name="id">id of station</param>
            /// <returns>return an instance of the station</returns>
            public Station DisplayStation(int id)
            {
                return DataSource.Stations.Find(x => x.Id == id);
            }

            /// <summary>
            /// return Drone with the same id as the parameter id
            /// </summary>
            /// <param name="id">id of drone</param>
            /// <returns>return an instance of the drone</returns>
            public Drone DisplayDrone(int id)
            {
                return DataSource.Drones.Find(x => x.Id == id);
            }

            /// <summary>
            /// return Customer with the same id as the parameter id
            /// </summary>
            /// <param name="id">id of customer</param>
            /// <returns>return an instance of the cutomer</returns>
            public Customer DisplayCustomer(int id)
            {
                return DataSource.Customers.Find(x => x.Id == id);
            }

            /// <summary>
            /// return parcel with the same id as the parameter id
            /// </summary>
            /// <param name="id">id of parcel</param>
            /// <returns>return an instance of teh parcel</returns>
            public Parcel DisplayParcel(int id)
            {
                return DataSource.Parcels.Find(x => x.Id == id);
            }

            /// <summary>
            /// calculate the distance between a coordiante and a station
            /// </summary>
            /// <param name="geo">represent a coordinate</param>
            /// <param name="id">id of a station</param>
            /// <returns>reutrn the distance between the coordinate and the station</returns>
            public double GetDistanceFromStation(GeoCoordinate geo, int id)
            {
                Station s = DataSource.Stations.Find(x => x.Id == id);
                return GeoCoordinate.distance(geo, s.Coordinate);
            }

            /// <summary>
            /// calculate the distance between a coordinate and a customer
            /// </summary>
            /// <param name="geo">represent a coordiante</param>
            /// <param name="id">id of the station</param>
            /// <returns>the distance between the coordinate and the customer</returns>
            public double GetDistanceFromCustomer(GeoCoordinate geo, int id)
            {
                Customer c = DataSource.Customers.Find(x => x.Id == id);
                return GeoCoordinate.distance(geo, c.Coordinate);
            }
        }
    }
}


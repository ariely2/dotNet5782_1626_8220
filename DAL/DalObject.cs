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
            public void Add<T>(T t) where T : struct
            {
                switch (t)
                {
                    case Station s:
                        DataSource.Stations.Add(s);
                        break;
                    case Drone d:
                        DataSource.Drones.Add(d);
                        break;
                    case Customer c:
                        DataSource.Customers.Add(c);
                        break;
                    case Parcel p:
                        p.Id = DataSource.Config.IdOfParcel;
                        p.Scheduled = p.DroneId == 0 ? DateTime.MinValue : p.Requested;
                        DataSource.Parcels.Add(p);
                        break;
                    default:
                        break;
                }
            }
            public IEnumerable<T> RequestList<T>() where T : struct
            {
                switch (typeof(T).Name)
                {
                    case nameof(Station):
                        return (IEnumerable<T>)DataSource.Stations;
                    case nameof(Customer):
                        return (IEnumerable<T>)DataSource.Customers;
                    case nameof(Drone):
                        return (IEnumerable<T>)DataSource.Drones;
                    case nameof(Parcel):
                        return (IEnumerable<T>)DataSource.Parcels;
                    default:
                        break;
                }
                return null;

            }

            public T Request<T>(int id)where T:struct
            {
                switch (typeof(T).Name)
                {
                    case nameof(Station):
                        return (T)Convert.ChangeType(DataSource.Stations.Find(s=>s.Id==id), typeof(T));
                    case nameof(Customer):
                        return (T)Convert.ChangeType(DataSource.Customers.Find(c => c.Id == id), typeof(T));
                    case nameof(Drone):
                        return (T)Convert.ChangeType(DataSource.Drones.Find(d=> d.Id == id), typeof(T));
                    case nameof(Parcel):
                        return (T)Convert.ChangeType(DataSource.Parcels.Find(p => p.Id == id), typeof(T));
                    default:
                        break;
                }
                //throw;
                return default(T);
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
            public IEnumerable<Parcel> UnassignedParcels()
            {
                return (IEnumerable<Parcel>)DataSource.Parcels.FindAll(x => x.DroneId == 0);
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
            public double[] GetBatteryUsageInfo()
            {
                double[] info =
                    {DataSource.Config.AvailableUse,
                    DataSource.Config.LightUse,
                    DataSource.Config.MediumUse,
                    DataSource.Config.HeavyUse,
                    DataSource.Config.ChargeRate };
                return info;
            }
        }
    }
}



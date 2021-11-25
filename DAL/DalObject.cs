using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using IDAL.DO;
namespace IDAL
{
    namespace DalObject
    {
        public class DalObject : IDal
        {
            #region Constructor
            /// <summary>
            /// constructor
            /// called the function initialze to initial the database
            /// </summary>
            public DalObject()
            {
                DataSource.Config.Initialize();
            }
            #endregion Constructor
            #region Create
            public void Create<T>(T t) where T : struct
            {

                switch (t)
                {

                    case Station s:
                        if (!DataSource.Stations.Find(x => x.Id == s.Id).Equals(default(Station)))
                            throw new ExistId("Station id already exist\n");
                        DataSource.Stations.Add(s);
                        break;
                    case Drone d:
                        if (!DataSource.Drones.Find(x => x.Id == d.Id).Equals(default(Drone)))
                            throw new ExistId("Drone id already exist\n");
                        DataSource.Drones.Add(d);
                        break;
                    case Customer c:
                        if (!DataSource.Customers.Find(x => x.Id == c.Id).Equals(default(Customer)))
                            throw new ExistId("Customer id already exist\n");
                        DataSource.Customers.Add(c);
                        break;
                    case Parcel p://isn't possible to have exception in parcel, the id isn't chosen by the user.
                        p.Id = DataSource.Config.IdOfParcel;
                        DataSource.Parcels.Add(p);
                        break;
                    default: //unknown struct
                        throw new NotExistStruct("struct isn't exist\n");
                }
            }
            #endregion Create
            #region Request

            public T Request<T>(int id) where T : struct
            {
                T ans;
                switch (typeof(T).Name)
                {
                    case nameof(Station):
                        ans = (T)Convert.ChangeType(DataSource.Stations.Find(s => s.Id == id), typeof(T));
                        break;
                    case nameof(Customer):
                        ans = (T)Convert.ChangeType(DataSource.Customers.Find(c => c.Id == id), typeof(T));
                        break;
                    case nameof(Drone):
                        ans = (T)Convert.ChangeType(DataSource.Drones.Find(d => d.Id == id), typeof(T));
                        break;
                    case nameof(Parcel):
                        ans = (T)Convert.ChangeType(DataSource.Parcels.Find(p => p.Id == id), typeof(T));
                        break;
                    default:
                        throw new NotExistStruct("struct doesn't exist\n");
                }
                if (ans.Equals(default(T)))
                    throw new NotExistId("id doesn't exist\n");
                return ans;
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

            public double GetDistanceFrom<T>(Location location, int id)where T:struct
            {
                Location ans;
                switch (typeof(T).Name)
                {
                    case nameof(Station):
                        ans = DataSource.Stations.Find(s => s.Id == id).location;
                        break;
                    case nameof(Customer):
                        ans = DataSource.Customers.Find(c => c.Id == id).location;
                        break;
                    default:
                        throw new NotExistStruct("Not support this requests\n");
                }
                if (ans.Equals(default(T)))
                    throw new NotExistId("id isn't exist\n");
                return Location.distance(location, ans);
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

            #endregion Request
            #region Update
            /// <summary>
            /// the function reponsible for assign a drone to a parcel
            /// </summary>
            /// <param name="ParcelId">id of the parcel</param>
            public void AssignParcel(int ParcelId)
            {
                Parcel p = Request<Parcel>(ParcelId);
                Drone d = DataSource.Drones.Find(x => x.MaxWeight >= p.Weight);
                p.DroneId = d.Id;
                p.Scheduled = DateTime.Now;
                int a = DataSource.Parcels.FindIndex(x => x.Id == ParcelId);
                Replace(p, a, DataSource.Parcels);
            }

            /// <summary>
            /// the function responsible for pick up a parcel by a drone
            /// </summary>
            /// <param name="ParcelId">id of the parcel</param>
            public void PickUpParcel(int ParcelId)
            {
                Parcel p = DataSource.Parcels.Find(x => x.Id == ParcelId);
                p.PickedUp = DateTime.Now;
                int a = DataSource.Parcels.FindIndex(x => x.Id == ParcelId);
                Replace(p, a, DataSource.Parcels);
            }

            /// <summary>
            /// the function responsible for deliver a parcel to a customer
            /// </summary>
            /// <param name="ParcelId">id of the parcel</param>
            public void DeliverParcel(int ParcelId)
            {
                Parcel p = DataSource.Parcels.Find(x => x.Id == ParcelId);
                int a = DataSource.Parcels.FindIndex(x => x.Id == ParcelId);
                p.Delivered = DateTime.Now;
                Replace(p, a, DataSource.Parcels);
            }

            /// <summary>
            /// the function charge a drone in a station
            /// </summary>
            /// <param name="DroneId">id of the drone</param>
            /// <param name="StationId">id of the station</param>
            public void ChargeDrone(int DroneId, int StationId)
            {
                Station s = DataSource.Stations.Find(x => x.Id == StationId);
                s.ChargeSlots--;
                int b = DataSource.Stations.FindIndex(x => x.Id == s.Id);
                DataSource.DroneCharges.Add(new DroneCharge() { DroneId = DroneId, StationId = s.Id });
                Replace(s, b, DataSource.Stations);
            }


            /// <summary>
            /// The function releases the drone from the station where it is charged
            /// </summary>
            /// <param name="DroneId">id of drone to release</param>
            public void ReleaseDrone(int DroneId)
            {
                Drone d = DataSource.Drones.Find(x => x.Id ==DroneId);
                DroneCharge c = DataSource.DroneCharges.Find(x => x.DroneId == d.Id);
                Station s = DataSource.Stations.Find(x => x.Id == c.StationId);
                s.ChargeSlots++;
                int b = DataSource.Stations.FindIndex(x => x.Id == s.Id);
                Replace(s, b, DataSource.Stations);
                DataSource.DroneCharges.Remove(c);
            }
            #endregion Update
            #region InternalMethods
            /// <summary>
            /// replace the value at index index with the item T
            /// </summary>
            /// <typeparam name="T">represent an instance of struct T</typeparam>
            /// <param name="Item">Item to add to the list</param>
            /// <param name="index">index to remove </param>
            /// <param name="list">list of T</param>
            internal void Replace<T>(T Item, int index, List<T> list)
            {

                list.RemoveAt(index);
                list.Insert(index, Item);
            }
            #endregion InternalMethods
            public int[] Receivers()
            {
                List<Parcel> d = (List<Parcel>)DataSource.Parcels.FindAll(x => x.Delivered != DateTime.MinValue); //all delivered parcels
                //d.RemoveAll(x => DataSource.Customers.Exists(c => c.Id == x.TargetId)); //remove receivers who aren't customers
                int [] t = d.Select(x => x.TargetId).ToArray(); //getting receiver ids
                return t.Distinct().ToArray(); //return array without duplicates
            }
        }
    }
}
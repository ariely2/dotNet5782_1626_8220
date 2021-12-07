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

            /// <summary>
            /// add an object to the lists (customer, station, drone, parcel)
            /// </summary>
            /// <typeparam name="T">type of object</typeparam>
            /// <param name="t">the object</param>
            public void Create<T>(T t) where T : struct
            {

                switch (t)
                {
                    case Station s:
                        //id out of bounds
                        if (!s.Check())
                            throw new OutOfBoundsException("Station's id out of bounds\n");
                        //id already exist
                        if (DataSource.Stations.Exists(x => x.Id == s.Id))
                            throw new AlreadyExistException("Station's id already exist\n");

                        //station with negative charge slots
                        if (s.ChargeSlots < 0)
                            throw new NotPossibleException("Can't be station with negative available slots\n");
                        
                        //add station
                        DataSource.Stations.Add(s);
                        break;
                    case Drone d:
                        //id out of bounds
                        if (!d.Check())
                            throw new OutOfBoundsException("Drone's id out of bounds\n");
                        //id already exist
                        if (DataSource.Drones.Exists(x => x.Id == d.Id))
                            throw new AlreadyExistException("Drone's id already exist\n");

                        //add drone
                        DataSource.Drones.Add(d);
                        break;
                    case Customer c:

                        //id out of bounds
                        if (!c.Check())
                            throw new OutOfBoundsException("Customer's id out of bounds\n");

                        //id already exist
                        if (DataSource.Customers.Exists(x => x.Id == c.Id))
                            throw new AlreadyExistException("Customer's id already exist\n");

                        //add customer
                        DataSource.Customers.Add(c);
                        break;

                    case Parcel p:
                        //isn't possible to have id exception in parcel, the id isn't chosen by the user.

                        //sender isn't exist
                        if (!DataSource.Customers.Exists(c => c.Id == p.SenderId))
                            throw new NotExistException("Sender's id isn't exist\n");

                        //target isn't exist
                        if (!DataSource.Customers.Exists(c => c.Id == p.ReceiverId))
                            throw new NotExistException("Target's id ins't exist\n");

                        //drone isn't exist, and its id isn't 0
                        if (p.DroneId != null && !DataSource.Drones.Exists(d => d.Id == p.DroneId))
                            throw new NotExistException("Drone's id isn't exist\n");

                        //add parcel
                        p.Id = DataSource.Config.IdOfParcel++;
                        DataSource.Parcels.Add(p);
                        break;
                    default: //unknown struct
                        throw new NotSupportException("Not support " + typeof(T).Name + "\n");
                }
            }
            #endregion Create
            #region Request

            /// <summary>
            /// request an object (customer, parcel, station, drone)
            /// if the object isn't exist, the function throw an exception
            /// </summary>
            /// <typeparam name="T">type of the requested object is T</typeparam>
            /// <param name="id">id of the object</param>
            /// <returns>return the object</returns>
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
                        throw new NotSupportException("Not support this struct\n");
                }
                //if it isn't exist
                if (ans.Equals(default(T)))
                    throw new NotExistException(typeof(T).Name + " with id: " + id +  " isn't exist\n");
                return ans;
            }

            /// <summary>
            /// return a list of type T
            /// the lists are: Stations, drones, customers, parcels
            /// </summary>
            /// <typeparam name="T">type of the requested list</typeparam>
            /// <returns></returns>
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
                        throw new NotSupportException("Not support this struct");
                }
            }

            /// <summary>
            /// find station with avaiable chargeslots
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
                return (IEnumerable<Parcel>)DataSource.Parcels.FindAll(x => x.DroneId == null);
            }

            /// <summary>
            /// the function return the distance from specific location to an object (customer, station)
            /// throw an exception, if the object isn't exist
            /// </summary>
            /// <typeparam name="T">type of the object</typeparam>
            /// <param name="location">the specific location</param>
            /// <param name="id">id of the object</param>
            /// <returns>reutrn the distance between the object and the location</returns>
            public double GetDistanceFrom<T>(Location location, int id) where T : struct
            {
                //in case of exception, request function would send it
                Location ans;
                switch (typeof(T).Name)
                {
                    case nameof(Station):
                        ans = Request<Station>(id).Location;
                        break;
                    case nameof(Customer):
                        ans = Request<Customer>(id).Location;
                        break;
                    default:
                        throw new NotSupportException("Not support this struct\n");
                }
                return Location.distance(location, ans);
            }

            /// <summary>
            /// the function return a list of battery usage info
            /// </summary>
            /// <returns>array of double about battery usage info</returns>
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

            public int[] Receivers()
            {
                List<Parcel> d = (List<Parcel>)DataSource.Parcels.FindAll(x => x.Delivered != DateTime.MinValue); //all delivered parcels
                //d.RemoveAll(x => DataSource.Customers.Exists(c => c.Id == x.TargetId)); //remove receivers who aren't customers
                int[] t = d.Select(x => x.ReceiverId).ToArray(); //getting receiver ids
                return t.Distinct().ToArray(); //return array without duplicates
            }

            #endregion Request
            #region Update
            /// <summary>
            /// the function connect parcel to a drone
            /// throw exceptions if the drone or the parcel aren't exist
            /// </summary>
            /// <param name="parcelId">parcel's id</param>
            /// <param name="droneId">drone's id</param>
            public void AssignParcel(int parcelId, int droneId)
            {
                //if the drone or parcel isn't exist, request function would send an exception
                Parcel p= Request<Parcel>(parcelId);
                Drone d = Request<Drone>(droneId);
                //update properties of the parcel
                p.Scheduled = DateTime.Now;
                p.DroneId = d.Id;
                DataSource.Parcels[DataSource.Parcels.FindIndex(p => p.Id == parcelId)] = p;
            }

            /// <summary>
            /// the function responsible for pick up a parcel by a drone
            /// throw exception if the parcel isn't exist or if we didn't assign a drone to the parcel
            /// </summary>
            /// <param name="parcelId">id of the parcel</param>
            public void PickUpParcel(int parcelId)
            {
                //if parcel isn't exist, request function would send an exception
                Parcel p=  Request<Parcel>(parcelId);
                p.PickedUp = DateTime.Now;
                DataSource.Parcels[DataSource.Parcels.FindIndex(x => x.Id == parcelId)] = p;
            }

            /// <summary>
            /// the function responsible for deliver a parcel to a customer
            /// if the parcel isn't exists or if the drone didn't pick up the parcel, or assign to the parcel then throw an exception, 
            /// </summary>
            /// <param name="parcelId">id of the parcel</param>
            public void DeliverParcel(int parcelId)
            {
                //if parcel isn't exist, request function would send an exception
                Parcel p = Request<Parcel>(parcelId);
                p.Delivered = DateTime.Now;
                DataSource.Parcels[DataSource.Parcels.FindIndex(p => p.Id == parcelId)] = p;
            }

            /// <summary>
            /// the function charge a drone in a specific station
            /// throw an exception if the station is full, or there station or the drone ins't exist
            /// </summary>
            /// <param name="droneId">id of the drone</param>
            /// <param name="stationId">id of the station</param>
            public void ChargeDrone(int droneId, int stationId)
            {
                //if station or drone isn't exist, request function would send an exception
                Station s = Request<Station>(stationId);
                Drone d = Request<Drone>(droneId);
                s.ChargeSlots--;
                DataSource.DroneCharges.Add(new DroneCharge() { DroneId = d.Id, StationId = s.Id });
                DataSource.Stations[DataSource.Stations.FindIndex(x => x.Id == s.Id)] = s;
            }


            /// <summary>
            /// The function releases the drone from the station where it is currently charged
            /// </summary>
            /// <param name="droneId">id of drone to release</param>
            public void ReleaseDrone(int droneId)
            {
                //if the drone isn't exist, request function would send an exception  
                Drone d = Request<Drone>(droneId);
                //assume that the drone is currently charging, bl responsible for checking that
                DroneCharge c = DataSource.DroneCharges.Find(x => x.DroneId == droneId);

                Station s = Request<Station>(c.StationId);
                s.ChargeSlots++;

                DataSource.Stations[DataSource.Stations.FindIndex(x => x.Id == s.Id)] = s;
               
                DataSource.DroneCharges.Remove(c);
            }
            #endregion Update
            #region Delete
            /// <summary>
            /// remove an object from its list, given it's id
            /// throw an exception if the object isn't exist
            /// </summary>
            /// <typeparam name="T">type of the object (station, customer, drone, parcel)</typeparam>
            /// <param name="id">id of the object</param>
            public void Delete<T>(int id) where T : struct
            {
                //if there is an exception of id isn't exist, request function would send it
                switch (typeof(T).Name)
                {
                    case nameof(Station):
                        Station s = Request<Station>(id);
                        DataSource.Stations.Remove(s);
                        break;
                    case nameof(Customer):
                        Customer c = Request<Customer>(id);
                        DataSource.Customers.Remove(c);
                        break;
                    case nameof(Drone):
                        Drone d = Request<Drone>(id);
                        DataSource.Drones.Remove(d);
                        break;
                    case nameof(Parcel):
                        Parcel p = Request<Parcel>(id);
                        DataSource.Parcels.Remove(p);
                        break;
                    default:
                        throw new NotSupportException("Not support" + typeof(T).Name + "struct\n");

                }
            }
            #endregion Delete
        }
    }
}
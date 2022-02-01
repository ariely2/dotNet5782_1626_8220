using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using DalApi;
using DO;
using System.Reflection;
namespace Dal
{
    internal sealed class DalXml : IDal
    {
        private readonly Dictionary<string, string> pathes;
        static readonly IDal instance = new DalXml();
        public static IDal Instance { get => instance; }
        DalXml()
        {
            pathes = new Dictionary<string, string>();
            pathes.Add("Drone", @"Drones.xml");
            pathes.Add("Station", @"Stations.xml");
            pathes.Add("Config", @"Config.xml");
            pathes.Add("DroneCharge", @"DroneCharges.xml");
            pathes.Add("Parcel", @"Parcels.xml");
            pathes.Add("Customer", @"Customers.xml");
            XMLTools.LoadListFromXMLSerializer<Drone>(pathes["Drone"]);
            XMLTools.LoadListFromXMLSerializer<Station>(pathes["Station"]);
            XMLTools.LoadListFromXMLSerializer<DroneCharge>(pathes["DroneCharge"]);
            XMLTools.LoadListFromXMLSerializer<Parcel>(pathes["Parcel"]);
            XMLTools.LoadListFromXMLSerializer<Customer>(pathes["Customer"]);
        }
        #region Create

        /// <summary>
        /// add an object to the lists (customer, station, drone, parcel)
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="t">the object</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Create<T>(T t) where T : struct
        {

            switch (t)
            {
                case Station s:
                    var Stations = XMLTools.LoadListFromXMLSerializer<Station>(pathes["Station"]);
                    //id out of bounds
                    if (!s.Check())
                        throw new OutOfBoundsException("Station's id out of bounds\n");
                    //id already exist
                    if (Stations.Exists(x => x.Id == s.Id))
                        throw new AlreadyExistException("A station with the same Id already exist\n");
                    //station with negative charge slots
                    if (s.ChargeSlots < 0)
                        throw new NotPossibleException("A Station can't have a negative number of available slots\n");
                    //check location
                    if (s.Location.Longitude > 80 || s.Location.Longitude < -180 || s.Location.Latitude < -90 || s.Location.Latitude > 90)
                        throw new OutOfBoundsException("Station location out of bounds\n");
                    //add station
                    Stations.Add(s);

                    XMLTools.SaveListToXMLSerializer<Station>(Stations, pathes["Station"]);
                    break;
                case Drone d:

                    var Drones = XMLTools.LoadListFromXMLSerializer<Drone>(pathes["Drone"]);
                    //id out of bounds
                    if (!d.Check())
                        throw new OutOfBoundsException("Drone's id out of bounds\n");
                    //id already exist
                    if (Drones.Exists(x => x.Id == d.Id))
                        throw new AlreadyExistException("A Drone with the same Id already exist\n");

                    //add drone
                    Drones.Add(d);
                    XMLTools.SaveListToXMLSerializer<Drone>(Drones, pathes["Drone"]);
                    break;
                case Customer c:

                    var Customers = XMLTools.LoadListFromXMLSerializer<Customer>(pathes["Customer"]);
                    //id out of bounds
                    if (!c.Check())
                        throw new OutOfBoundsException("Customer's id out of bounds\n");

                    //id already exist
                    if (Customers.Exists(x => x.Id == c.Id))
                        throw new AlreadyExistException("Customer's id already exists\n");

                    //check location
                    if (c.Location.Longitude > 80 || c.Location.Longitude < -180 || c.Location.Latitude < -90 || c.Location.Latitude > 90)
                        throw new OutOfBoundsException("Station location out of bounds\n");
                    //add customer
                    Customers.Add(c);
                    XMLTools.SaveListToXMLSerializer<Customer>(Customers, pathes["Customer"]);
                    break;

                case Parcel p:
                    //isn't possible to have id exception in parcel, the id isn't chosen by the user.

                    Customers = XMLTools.LoadListFromXMLSerializer<Customer>(pathes["Customer"]);
                    //sender isn't exist
                    if (!Customers.Exists(c => c.Id == p.SenderId))
                        throw new NotExistException("Sender's id isn't exist\n");

                    //target isn't exist
                    if (!Customers.Exists(c => c.Id == p.ReceiverId))
                        throw new NotExistException("Target's id ins't exist\n");


                    //drone isn't exist, and its id isn't 0
                    if (p.DroneId != null)
                        Request<Drone>((int)p.DroneId);

                    var Parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(pathes["Parcel"]);
                    XElement conf = XMLTools.LoadListFromXElement(pathes["Config"]);

                    //add parcel
                    p.Id = Convert.ToInt32(conf.Element("ParcelId").Value);
                    conf.Element("ParcelId").SetValue(p.Id + 1);
                    XMLTools.SaveListToXElement(conf, pathes["Config"]);

                    Parcels.Add(p);
                    XMLTools.SaveListToXMLSerializer<Parcel>(Parcels, pathes["Parcel"]);
                    break;
                case DroneCharge dc:
                    var DroneCharges = XMLTools.LoadListFromXMLSerializer<DroneCharge>(pathes["DroneCharge"]);
                    Request<Drone>(dc.DroneId);
                    var station = Request<Station>(dc.StationId);station.ChargeSlots--;
                    DroneCharges.Add(dc);
                    Update<Station>(station.Id, station);
                    XMLTools.SaveListToXMLSerializer<DroneCharge>(DroneCharges, pathes["DroneCharge"]);
                    break;
                default: //unknown struct
                    throw new NotSupportException("Not support " + typeof(T).Name + "\n");
            }
        }
        #endregion Create
        #region Request

        /// <summary>
        /// request an object (customer, parcel, station, drone,droneCharge)
        /// if the object isn't exist, the function throw an exception
        /// </summary>
        /// <typeparam name="T">type of the requested object is T</typeparam>
        /// <param name="id">id of the object</param>
        /// <returns>return the object</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public T Request<T>(int id) where T : struct
        {
            T ans;

            switch (typeof(T).Name)
            {
                case nameof(Station):

                    ans = (T)Convert.ChangeType(RequestList<Station>(s => s.Id == id).FirstOrDefault(), typeof(T));
                    break;
                case nameof(Customer):

                    ans = (T)Convert.ChangeType(RequestList<Customer>(c => c.Id == id).FirstOrDefault(), typeof(T));
                    break;
                case nameof(Drone):

                    ans = (T)Convert.ChangeType(RequestList<Drone>(d => d.Id == id).FirstOrDefault(), typeof(T));
                    break;
                case nameof(Parcel):

                    ans = (T)Convert.ChangeType(RequestList<Parcel>(p => p.Id == id).FirstOrDefault(), typeof(T));
                    break;
                case nameof(DroneCharge):
                    ans = (T)Convert.ChangeType(RequestList<DroneCharge>(dc => dc.DroneId == id).FirstOrDefault(), typeof(T));
                    break;
                default:
                    throw new NotSupportException("Not support this struct\n");
            }
            //if it isn't exist
            if (ans.Equals(default(T)))
                throw new NotExistException(typeof(T).Name + " with id: " + id + " isn't exist\n");
            return ans;
        }

        /// <summary>
        /// return a list of type T
        /// the lists are: Stations, drones, customers, parcels, droneCharge
        /// </summary>
        /// <typeparam name="T">type of the requested list</typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<T> RequestList<T>(Expression<Func<T, bool>> ex = null) where T : struct
        {
            switch (typeof(T).Name)
            {
                case nameof(Station):
                    return (IEnumerable<T>)XMLTools.LoadListFromXMLSerializer<Station>(pathes["Station"]).FindAll(ex == null ? x => true : Expression.Lambda<Func<Station, bool>>(Expression.Convert(ex.Body, typeof(bool)), ex.Parameters).Compile().Invoke).AsEnumerable();
                case nameof(Customer):
                    return (IEnumerable<T>)XMLTools.LoadListFromXMLSerializer<Customer>(pathes["Customer"]).FindAll(ex == null ? x => true : Expression.Lambda<Func<Customer, bool>>(Expression.Convert(ex.Body, typeof(bool)), ex.Parameters).Compile().Invoke).AsEnumerable();
                case nameof(Drone):
                    return (IEnumerable<T>)XMLTools.LoadListFromXMLSerializer<Drone>(pathes["Drone"]).FindAll(ex == null ? x => true : Expression.Lambda<Func<Drone, bool>>(Expression.Convert(ex.Body, typeof(bool)), ex.Parameters).Compile().Invoke).AsEnumerable();
                case nameof(Parcel):
                    return (IEnumerable<T>)XMLTools.LoadListFromXMLSerializer<Parcel>(pathes["Parcel"]).FindAll(ex == null ? x => true : Expression.Lambda<Func<Parcel, bool>>(Expression.Convert(ex.Body, typeof(bool)), ex.Parameters).Compile().Invoke).AsEnumerable();
                case nameof(DroneCharge):
                    return (IEnumerable<T>)XMLTools.LoadListFromXMLSerializer<DroneCharge>(pathes["DroneCharge"]).FindAll(ex == null ? x => true : Expression.Lambda<Func<DroneCharge, bool>>(Expression.Convert(ex.Body, typeof(bool)), ex.Parameters).Compile().Invoke).AsEnumerable();
                default:
                    throw new NotSupportException("Not support this struct");
            }
        }

        /// <summary>
        /// the function return the distance from specific location to an object (customer, station)
        /// throw an exception, if the object isn't exist
        /// </summary>
        /// <typeparam name="T">type of the object</typeparam>
        /// <param name="location">the specific location</param>
        /// <param name="id">id of the object</param>
        /// <returns>reutrn the distance between the object and the location</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
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
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double[] GetBatteryUsageInfo()
        {
            XElement conf = XMLTools.LoadListFromXElement(pathes["Config"]);
            XElement battery = conf.Element("Battery");
            double[] info = {Convert.ToInt32(battery.Element("AvailableUse").Value),
                    Convert.ToInt32(battery.Element("LightUse").Value),
                    Convert.ToInt32(battery.Element("MediumUse").Value),
                    Convert.ToInt32(battery.Element("HeavyUse").Value),
                    Convert.ToInt32(battery.Element("ChargeRate").Value) };
            return info;
        }
        //need to move it to bl
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int[] Receivers()
        {
            IEnumerable<Parcel> d = RequestList<Parcel>(x => x.Delivered != null); //all delivered parcels
            //d.RemoveAll(x => Customers.Exists(c => c.Id == x.TargetId)); //remove receivers who aren't customers
            int[] t = d.Select(x => x.ReceiverId).ToArray(); //getting receiver ids
            return t.Distinct().ToArray(); //return array without duplicates
        }

        #endregion Request
        #region Update
        /// <summary>
        /// the function connect parcel to a drone
        /// throw exceptions if the drone or the parcel aren't exist
        /// </summary>
        /// <param name="ParcelId">parcel's id</param>
        /// <param name="DroneId">drone's id</param>
        public void AssignParcel(int ParcelId, int DroneId)
        {
            //if the drone or parcel isn't exist, request function would send an exception
            Parcel p = Request<Parcel>(ParcelId);
            Drone d = Request<Drone>(DroneId);
            //update properties of the parcel
            p.Scheduled = DateTime.Now;
            p.DroneId = d.Id;
            Update<Parcel>(ParcelId, p);
        }

        /// <summary>
        /// the function responsible for pick up a parcel by a drone
        /// throw exception if the parcel isn't exist or if we didn't assign a drone to the parcel
        /// </summary>
        /// <param name="parcelId">id of the parcel</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PickUpParcel(int parcelId)
        {
            //if parcel isn't exist, request function would send an exception
            Parcel p = Request<Parcel>(parcelId);
            p.PickedUp = DateTime.Now;
            Update<Parcel>(parcelId, p);
        }

        /// <summary>
        /// the function responsible for deliver a parcel to a customer
        /// if the parcel isn't exists or if the drone didn't pick up the parcel, or assign to the parcel then throw an exception, 
        /// </summary>
        /// <param name="parcelId">id of the parcel</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeliverParcel(int parcelId)
        {
            //if parcel isn't exist, request function would send an exception
            Parcel p = Request<Parcel>(parcelId);
            p.Delivered = DateTime.Now;
            Update<Parcel>(parcelId, p);
        }

        /// <summary>
        /// the function charge a drone in a specific station
        /// throw an exception if the station is full, or there station or the drone ins't exist
        /// </summary>
        /// <param name="droneId">id of the drone</param>
        /// <param name="stationId">id of the station</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ChargeDrone(int droneId, int stationId)
        {
            //if station or drone isn't exist, request function would send an exception
            Station s = Request<Station>(stationId);
            Drone d = Request<Drone>(droneId);

            Create<DroneCharge>(new DroneCharge() { DroneId = d.Id, StationId = s.Id, Start = DateTime.Now });
        }


        /// <summary>
        /// The function releases the drone from the station where it is currently charged
        /// </summary>
        /// <param name="droneId">id of drone to release</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ReleaseDrone(int droneId)
        {
            //assume that the drone is currently charging, bl responsible for checking that
            DroneCharge c = Request<DroneCharge>(droneId);
            Delete<DroneCharge>(droneId);
        }
        /// <summary>
        /// update an object in xml files
        /// </summary>
        /// <typeparam name="T">type of the object. can be: Customer, Drone, DroneCharge, Parcel, Station</typeparam>
        /// <param name="id">id of the object</param>
        /// <param name="t">the new object</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Update<T>(int id, T t) where T : struct
        {
            Delete<T>(id);

            var list = XMLTools.LoadListFromXMLSerializer<T>(pathes[typeof(T).Name]);
            list.Add(t);
            XMLTools.SaveListToXMLSerializer<T>(list, pathes[typeof(T).Name]);
        }
        #endregion Update
        #region Delete
        /// <summary>
        /// remove an object from its list, given it's id
        /// throw an exception if the object isn't exist
        /// </summary>
        /// <typeparam name="T">type of the object (station, customer, drone, parcel)</typeparam>
        /// <param name="id">id of the object</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Delete<T>(int id) where T : struct
        {
            //if there is an exception of id isn't exist, request function would send it
            switch (typeof(T).Name)
            {
                case nameof(Station):
                    Station s = Request<Station>(id);
                    var Stations = XMLTools.LoadListFromXMLSerializer<Station>(pathes["Station"]);
                    Stations.Remove(s);
                    XMLTools.SaveListToXMLSerializer<Station>(Stations,pathes["Station"]);
                    break;
                case nameof(Customer):
                    Customer c = Request<Customer>(id);
                    var Customers = XMLTools.LoadListFromXMLSerializer<Customer>(pathes["Customer"]);
                    Customers.Remove(c);
                    XMLTools.SaveListToXMLSerializer<Customer>(Customers, pathes["Customer"]);
                    break;
                case nameof(Drone):
                    Drone d = Request<Drone>(id);
                    var Drones = XMLTools.LoadListFromXMLSerializer<Drone>(pathes["Drone"]);
                    Drones.Remove(d);
                    XMLTools.SaveListToXMLSerializer<Drone>(Drones, pathes["Drone"]);
                    break;
                case nameof(Parcel):
                    Parcel p = Request<Parcel>(id);
                    var Parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(pathes["Parcel"]);
                    Parcels.Remove(p);
                    XMLTools.SaveListToXMLSerializer<Parcel>(Parcels, pathes["Parcel"]);
                    break;
                case nameof(DroneCharge):
                    //find the charge according to drone id
                    DroneCharge dc = Request<DroneCharge>(id);
                    //there is extra place for another drone
                    var station = Request<Station>(dc.StationId);station.ChargeSlots++;
                    Update<Station>(station.Id, station);

                    var DroneCharges = XMLTools.LoadListFromXMLSerializer<DroneCharge>(pathes["DroneCharge"]);
                    DroneCharges.Remove(dc);
                    XMLTools.SaveListToXMLSerializer<DroneCharge>(DroneCharges, pathes["DroneCharge"]);
                    break;
                default:
                    throw new NotSupportException("Not support" + typeof(T).Name + "struct\n");
            }
        }
        #endregion Delete
    }
}

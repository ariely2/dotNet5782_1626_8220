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
            XMLTools.LoadListFromXMLSerializer<Parcel>( pathes["Parcel"]);
            XMLTools.LoadListFromXMLSerializer<Customer>(pathes["Customer"]);
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AssignParcel(int ParcelId, int DroneId)
        {
            Parcel p = Request<Parcel>(ParcelId);
            Drone d = Request<Drone>(DroneId);
            //update properties of the parcel
            p.Scheduled = DateTime.Now;
            p.DroneId = d.Id;
            Update<Parcel>(ParcelId, p);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ChargeDrone(int DroneId, int StationId)
        {
            Station s = Request<Station>(StationId);
            Drone d = Request<Drone>(DroneId);
            s.ChargeSlots--;
            Create<DroneCharge>(new DroneCharge(){DroneId = d.Id, StationId = s.Id });
            Update<Station>(StationId, s);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Create<T>(T t) where T : struct
        {
            XElement xml = XMLTools.LoadListFromXElement(pathes[typeof(T).Name]);
            XElement s = new XElement(typeof(T).Name);
            foreach (PropertyInfo finfo in t.GetType().GetProperties())
            {
                //update id of parcel in case
                if (finfo.Name == "Id" && typeof(T).Name == "Parcel")
                {
                    var conf = XMLTools.LoadListFromXElement(pathes["config"]);
                    int id = Convert.ToInt32(conf.Element("configData").Element("parcelId").Value) + 1;
                    s.Add(new XElement(finfo.Name, id));
                    conf.Element("configData").Element("parcelId").SetValue(id);
                    XMLTools.SaveListToXElement(conf, pathes["config"]);
                }
                //typeof(T).GetMethods();
                else
                {
                    if (finfo.Name == "Id")
                        if (RequestList<T>().Any(s=> Convert.ToInt32(typeof(T).GetProperty("Id").GetValue(s))== (int)finfo.GetValue(t)))
                            throw new AlreadyExistException(typeof(T).Name + " with id of: " + finfo.GetValue(t) + " already exist\n");
                    s.Add(new XElement(finfo.Name, finfo.GetValue(t)));
                }
            }
            xml.Add(s);
            XMLTools.SaveListToXElement(xml, pathes[typeof(T).Name]);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Delete<T>(int id) where T : struct
        {
            var xml = XMLTools.LoadListFromXMLSerializer<T>(pathes[typeof(T).Name]);
            xml.Remove(Request<T>(id));
            XMLTools.SaveListToXMLSerializer<T>(xml,pathes[typeof(T).Name]);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeliverParcel(int ParcelId)
        {
            Parcel p = Request<Parcel>(ParcelId);
            p.Delivered = DateTime.Now;
            Update<Parcel>(ParcelId, p);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public double[] GetBatteryUsageInfo()
        {
            var xml = XMLTools.LoadListFromXElement("config.xml");
            double[] info = { Convert.ToInt32(xml.Element("configData").Element("Battery").Element("availableUse").Value),
            Convert.ToInt32(xml.Element("configData").Element("Battery").Element("lightUse").Value),
            Convert.ToInt32(xml.Element("configData").Element("Battery").Element("mediumUse").Value),
            Convert.ToInt32(xml.Element("configData").Element("Battery").Element("heavyUse").Value),
            Convert.ToInt32(xml.Element("configData").Element("Battery").Element("chargeRate").Value)};
            return info;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public double GetDistanceFrom<T>(Location location, int id) where T : struct
        {
            T t = Request<T>(id);
            return Location.distance((Location)t.GetType().GetProperty("Location").GetValue(t),location);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PickUpParcel(int ParcelId)
        {
            Parcel p = Request<Parcel>(ParcelId);
            p.PickedUp = DateTime.Now;
            Update<Parcel>(ParcelId, p);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int[] Receivers()
        {
            IEnumerable<Parcel> d = RequestList<Parcel>(x => x.Delivered != null); //all delivered parcels
            //d.RemoveAll(x => DataSource.Customers.Exists(c => c.Id == x.TargetId)); //remove receivers who aren't customers
            int[] t = d.Select(x => x.ReceiverId).ToArray(); //getting receiver ids
            return t.Distinct().ToArray(); //return array without duplicates
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ReleaseDrone(int DroneId)
        {
            //if the drone isn't exist, request function would send an exception  
            Drone d = Request<Drone>(DroneId);
            //assume that the drone is currently charging, bl responsible for checking that
            DroneCharge c = Request<DroneCharge>(DroneId);
            Station s = Request<Station>(c.StationId);
            s.ChargeSlots++;
            Update<Station>(s.Id, s);
            Delete<DroneCharge>(DroneId);
        }
        public void Update<T>(int id, T t)where T:struct
        {
            Delete<T>(id);
            var xml = XMLTools.LoadListFromXElement(pathes[typeof(T).Name]);
            XElement s = new XElement(typeof(T).Name);
            foreach (PropertyInfo finfo in typeof(T).GetProperties())
            {
                s.Add(new XElement(finfo.Name, finfo.GetValue(t)));
            }
            xml.Add(s);
            XMLTools.SaveListToXElement(xml, pathes[typeof(T).Name]);

        [MethodImpl(MethodImplOptions.Synchronized)]
        public T Request<T>(int id) where T : struct
        {
            List<T> xml = XMLTools.LoadListFromXMLSerializer<T>(pathes[typeof(T).Name]);
            PropertyInfo idProp;
            if (typeof(T).Name == "DroneCharge")
                idProp = typeof(T).GetProperty("DroneId");
            else
                idProp = typeof(T).GetProperty("Id");
            if (idProp != null)
            {
                var result = xml.Find(x => idProp.GetValue(x).Equals(id));
                if(result.Equals(default(T)))
                    throw new NotExistException(typeof(T).Name + " with id: " + id + " isn't exist\n");
                return result;
            }
            throw new NotSupportException("Not support this struct\n");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<T> RequestList<T>(Expression<Func<T, bool>> ex = null) where T : struct
        {
            return XMLTools.LoadListFromXMLSerializer<T>(pathes[typeof(T).Name]).FindAll(ex == null ? x => true : ex.Compile().Invoke).AsEnumerable<T>();
        }
    }
}

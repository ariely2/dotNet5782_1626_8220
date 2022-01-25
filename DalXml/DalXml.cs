using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
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
            pathes.Add("Drone", "Drones.xml");
            pathes.Add("Station", "Stations.xml");
            pathes.Add("Config", "Config.xml");
            pathes.Add("DroneCharge", "DroneCharges.xml");
            pathes.Add("Parcel", "Parcels.xml");
            pathes.Add("Customer", "Customers.xml");
        }

        public void AssignParcel(int ParcelId, int DroneId)
        {
            throw new NotImplementedException();
        }

        public void ChargeDrone(int DroneId, int StationId)
        {
            throw new NotImplementedException();
        }

        public void Create<T>(T t) where T : struct
        {
            XElement xml = XMLTools.LoadListFromXElement(pathes[typeof(T).Name]);
            XElement s = new XElement(typeof(T).Name);
            foreach(PropertyInfo finfo in typeof(T).GetProperties())
            {
                typeof(T).GetMethods();
                s.Add(new XElement(finfo.Name, finfo.GetValue(t)));
            }
            xml.Add(s);
            XMLTools.SaveListToXElement(xml, pathes[typeof(T).Name]);
        }

        public void Delete<T>(int id) where T : struct
        {
            var xml = XMLTools.LoadListFromXMLSerializer<T>(pathes[typeof(T).Name]);
            xml.Remove(Request<T>(id));
            XMLTools.SaveListToXMLSerializer<T>(xml,pathes[typeof(T).Name]);
        }

        public void DeliverParcel(int ParcelId)
        {
            throw new NotImplementedException();
        }

        public double[] GetBatteryUsageInfo()
        {
            throw new NotImplementedException();
        }

        public double GetDistanceFrom<T>(Location location, int id) where T : struct
        {
            throw new NotImplementedException();
        }

        public void PickUpParcel(int ParcelId)
        {
            throw new NotImplementedException();
        }

        public int[] Receivers()
        {
            throw new NotImplementedException();
        }

        public void ReleaseDrone(int DroneId)
        {
            throw new NotImplementedException();
        }

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




    

        public IEnumerable<T> RequestList<T>(Expression<Func<T, bool>> ex = null) where T : struct
        {
            return XMLTools.LoadListFromXMLSerializer<T>(pathes[typeof(T).Name]).FindAll(ex == null ? x => true : ex.Compile().Invoke).AsEnumerable<T>();
        }
    }
}

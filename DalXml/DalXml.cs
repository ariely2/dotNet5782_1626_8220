using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using DalApi;
using DO;
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
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AssignParcel(int ParcelId, int DroneId)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ChargeDrone(int DroneId, int StationId)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Create<T>(T t) where T : struct
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Delete<T>(int id) where T : struct
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeliverParcel(int ParcelId)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public double[] GetBatteryUsageInfo()
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public double GetDistanceFrom<T>(Location location, int id) where T : struct
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PickUpParcel(int ParcelId)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int[] Receivers()
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ReleaseDrone(int DroneId)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public T Request<T>(int id) where T : struct
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<T> RequestList<T>(Expression<Func<T, bool>> ex = null) where T : struct
        {
            throw new NotImplementedException();
        }
    }
}

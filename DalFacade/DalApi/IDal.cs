using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DO;
namespace DalApi
{
    /// <summary>
    /// interface with all the function requried to the database
    /// </summary>
    public interface IDal
    {
        void Update<T>(int id, T t) where T : struct;
        void Create<T>(T t) where T : struct;
        public T Request<T>(int id) where T : struct;
        double[] GetBatteryUsageInfo();
        public IEnumerable<T> RequestList<T>(Expression<Func<T,bool>> ex = null) where T : struct;
        double GetDistanceFrom<T>(Location location, int id) where T : struct;
        void AssignParcel(int ParcelId, int DroneId);
        void ChargeDrone(int DroneId, int StationId);
        void DeliverParcel(int ParcelId);
        void PickUpParcel(int ParcelId);
        void ReleaseDrone(int DroneId);
        IEnumerable<int> Receivers();
        public void Delete<T>(int id) where T : struct;

    }
}
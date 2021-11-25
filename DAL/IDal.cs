using IDAL.DO;
using System.Collections.Generic;

namespace IDAL 
{
    public interface IDal
    {
        void Create<T>(T t) where T : struct;
        public T Request<T>(int id) where T : struct;
        double[] GetBatteryUsageInfo();
        public IEnumerable<T> RequestList<T>() where T : struct;
        IEnumerable<Station> GetAvailableStations();
        IEnumerable<Parcel> UnassignedParcels();
        double GetDistanceFrom<T>(Location location, int id) where T : struct;
        void AssignParcel(int ParcelId, int DroneId);
        void ChargeDrone(int DroneId, int StationId);
        void DeliverParcel(int ParcelId);
        void PickUpParcel(int ParcelId);
        void ReleaseDrone(int DroneId);
        int[] Receivers();
        public void Delete<T>(int id) where T : struct;

    }
}
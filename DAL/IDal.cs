using IDAL.DO;
using System.Collections.Generic;

namespace IDAL 
{
    public interface IDal
    {
        void Create<T>(T t) where T : struct;
        public IEnumerable<T> RequestList<T>() where T : struct;
        public T Request<T>(int id) where T : struct;
        void AssignParcel(int ParcelId);
        void ChargeDrone(int DroneId, int StationId);
        void DeliverParcel(int ParcelId);
        double GetDistanceFromCustomer(GeoCoordinate geo, int id);
        double GetDistanceFromStation(GeoCoordinate geo, int id);
        void PickUpParcel(int ParcelId);
        void ReleaseDrone(int DroneId);
        void Replace<T>(T Item, int index, List<T> list);
        IEnumerable<Station> GetAvailableStations();
        IEnumerable<Parcel> UnassignedParcels();
        double[] GetBatteryUsageInfo();
    }
}
﻿using IDAL.DO;
using System.Collections.Generic;

namespace IDAL.DalObject
{
    public interface IDal
    {
        void AddCustomer(Customer customer);
        void AddDrone(Drone drone);
        void AddParcel(Parcel parcel);
        void AddStation(Station station);
        void AssignParcel(int ParcelId);
        void ChargeDrone(int DroneId, int StationId);
        IEnumerable<List<Customer>> CustomersList();
        void DeliverParcel(int ParcelId);
        Customer DisplayCustomer(int id);
        Drone DisplayDrone(int id);
        Parcel DisplayParcel(int id);
        Station DisplayStation(int id);
        IEnumerable<List<Drone>> DronesList();
        IEnumerable<List<Station>> GetAvailableStations();
        double GetDistanceFromCustomer(GeoCoordinate geo, int id);
        double GetDistanceFromStation(GeoCoordinate geo, int id);
        IEnumerable<List<Parcel>> ParcelList();
        void PickUpParcel(int ParcelId);
        void ReleaseDrone(int DroneId);
        void Replace<T>(T Item, int index, List<T> list);
        IEnumerable<List<Station>> StationsList();
        IEnumerable<List<Parcel>> UnassignedParcels();
    }
}
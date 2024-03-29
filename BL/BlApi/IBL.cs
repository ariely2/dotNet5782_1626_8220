﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BO;
namespace BlApi
{
    /// <summary>
    /// interface with all the methods needed
    /// </summary>
    public interface IBL
    {
        public void Create<T>(T t) where T : class;
        public T Request<T>(int id) where T : class;
        public IEnumerable<T> RequestList<T>(Expression<Func<T,bool>> ex = null) where T : class;
        public void SendDroneToCharge(int d); //dronetolist or drone?
        public void ReleaseDrone(int d);
        public void AssignDrone(int id);
        public void PickUp(int id);
        public void Deliver(int id);
        public Location ClosestStation(Location d);

        public double MinBattery(double distance, int id);

        public void UpdateStation(int id, string name = null, int? slots = null);
        public void UpdateDrone(int id, string model = null);
        public void UpdateCustomer(int id, string name = null, string phone = null);
        public void DeleteParcel(int id);
        public void CustomerDeliver(int id);
        public void CustomerPickUp(int id);
        public void Simulator(int id, Action a, Func<bool> f);

    }
}

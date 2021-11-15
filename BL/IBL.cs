﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    interface IBL
    {
        public void Create<T>(T t) where T : struct;
        public T Request<T>(int id) where T : class;
        public IEnumerable<T> RequestList<T>() where T : class;

        public void UpdateDroneName(Drone d);
        public void UpdateStation(Station s);
        public void UpdateCustomer(Customer c);
        public void SendDroneToCharge(Drone d);
        public void ReleaseDrone(Drone d, double t);
        public void AssignDrone(Drone d);
        public void PickUp(Drone d);
        public void Deliver(Drone d);

    }
}

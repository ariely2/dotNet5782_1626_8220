using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBL.BO;
namespace IBL
{
    public interface IBL
    {
        public void Create<T>(T t) where T : class; //check if zacceses modifiers are correct 
        public T Request<T>(int id) where T : class;
        public IEnumerable<T> RequestList<T>() where T : class;
        public void Update<T>(int id, T t) where T : class;
        public void SendDroneToCharge(int d); //dronetolist or drone?
        public void ReleaseDrone(int d, double t);
        public void AssignDrone(int id);
        public void PickUp(int id);
        public void Deliver(int id);
        public bool isDroneAssigned(DroneToList d);
        public Location ClosestStation(DroneToList d);
        public double MinBattery(double distance, int id);
    }
}

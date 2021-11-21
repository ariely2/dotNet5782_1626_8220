using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
using IDAL.DalObject;

namespace IBL.BO
{
    public partial class BL:IBL
    {
        private IDal dal;
        public List<DroneToList> drones = new List<DroneToList>(); //public?
        //static??
        public double AvailableUse; 
        public double LightUse; 
        public double MediumUse; 
        public double HeavyUse;
        public double ChargeRate;
        public static Random r = new Random();
        public double[] info;
        public BL() 
        { 
            dal = new DalObject();
            info = dal.GetBatteryUsageInfo();
            AvailableUse = info[0];
            LightUse = info[1];
            MediumUse = info[2];
            HeavyUse = info[3];
            ChargeRate = info[4];
            
            foreach(var d in dal.RequestList<IDAL.DO.Drone>())
            {
                drones.Add(new DroneToList()
                {
                    Id = d.Id,
                    Model = d.Model,
                    MaxWeight = (BO.WeightCategories)d.MaxWeight
                });
                DroneToList Current = drones.Last();
                if (isDroneAssigned(Current))
                {
                    Current.Status = DroneStatuses.Delivery;
                    var p = dal.Request<IDAL.DO.Parcel>(Current.ParcelId); //the parcel assigned to the current drone
                    var c = dal.Request<IDAL.DO.Customer>(p.SenderId);
                    var t = dal.Request<IDAL.DO.Customer>(p.TargetId);
                    if (p.PickedUp == DateTime.MinValue)
                    {
                        Location t = new Location() { Latitude = c.location.Latitude, Longitude = c.location.Longitude };
                        Current.Location = ClosestStation(t);
                    }
                    else
                    {
                        Current.Location.Latitude = c.location.Latitude;
                        Current.Location.Longitude = c.location.Longitude;
                    }
                    Location target = new Location() { Latitude = t.location.Latitude, Longitude = t.location.Longitude };
                    double distance = Location.distance(Current.Location, target);
                    distance += Location.distance(target, ClosestStation(target));
                    double b = MinBattery(distance, Current.Id);
                    Current.Battery = r.NextDouble() * (100 - b) + b;
                }
                else
                {
                    Current.Status = (DroneStatuses)r.Next(0, 2);
                    if (Current.Status == DroneStatuses.Maintenance)
                    {
                        var Stations = dal.GetAvailableStations(); // we need an availble station to charge the drone
                        var l = Stations.ElementAt(r.Next(0, Stations.Count())); //getting random station 
                        Current.Location.Latitude = l.location.Latitude;
                        Current.Location.Longitude = l.location.Longitude;
                        Current.Battery = r.Next(0, 21);
                        SendDroneToCharge(Current.Id, l.Id);
                    }
                    else // if the drone's status is "Available"
                    {
                        var p = dal.Receivers();
                        int i = r.Next(0, p.Length);
                        var receiver = dal.Request<IDAL.DO.Customer>(p[i]);
                        Current.Location.Latitude = receiver.location.Latitude;
                        Current.Location.Longitude = receiver.location.Longitude;
                        double b = MinBattery(Location.distance(Current.Location, ClosestStation(Current.Location)), Current.Id);
                        Current.Battery = r.NextDouble() * (100 - b) + b;
                    }
                }
            }
        }
    }
}

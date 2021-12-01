using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
using IDAL.DalObject;

namespace IBL.BO
{
    /// <summary>
    /// Partial BL class that configures a BL instance
    /// </summary>
    public partial class BL:IBL
    {
        private IDal dal;
        public List<DroneToList> Drones = new List<DroneToList>(); //public?
        //static?
        public double AvailableUse; 
        public double LightUse; 
        public double MediumUse; 
        public double HeavyUse;
        public double ChargeRate;
        public static Random r = new Random();
        public double[] info;
        public BL() //BL constructor
        { 
            dal = new DalObject();
            info = dal.GetBatteryUsageInfo();
            AvailableUse = info[0];
            LightUse = info[1];
            MediumUse = info[2];
            HeavyUse = info[3];
            ChargeRate = info[4];
            
            foreach(var d in dal.RequestList<IDAL.DO.Drone>()) //adding drones from DAL drones list to  BL list
            {
                Drones.Add(new DroneToList()
                {
                    Id = d.Id,
                    Model = d.Model,
                    MaxWeight = (BO.WeightCategories)d.MaxWeight
                });
                DroneToList Current = Drones.Last(); // modifying the last drone entered to the list each time, to modify all of them
                if (isDroneAssigned(Current))
                {
                    Current.Status = DroneStatuses.Delivery;
                    var p = dal.Request<IDAL.DO.Parcel>(Current.ParcelId); //the parcel assigned to the current drone
                    var c = dal.Request<IDAL.DO.Customer>(p.SenderId); // the parcel's sender and receiver
                    var t = dal.Request<IDAL.DO.Customer>(p.TargetId);
                    Location source = GetCustomerLocation(c.Id);
                    Location target = GetCustomerLocation(t.Id);
                    double distance = Location.distance(source, target); //distance from sender to receiver
                    distance += Location.distance(target, ClosestStation(target)); // + distance from receiver to closest station
                    if (p.PickedUp == DateTime.MinValue) //if the parcel isn't picked up yet
                    {
                        Current.Location = ClosestStation(GetCustomerLocation(c.Id)); //change drone's location to sender's location
                        distance += Location.distance(Current.Location, source); // + distance from initial location to sender
                    }
                    else
                        Current.Location = GetCustomerLocation(c.Id);
                    double b = MinBattery(distance, Current.Id);
                    Current.Battery = r.NextDouble() * (100 - b) + b; //random battery between minimum battery needed to 100
                }
                else
                {
                    Current.Status = (DroneStatuses)r.Next(0, 2); 
                    if (Current.Status == DroneStatuses.Maintenance)
                    {
                        var Stations = dal.GetAvailableStations(); // we need an availble station to charge the drone
                        var s = Stations.ElementAt(r.Next(0, Stations.Count())); //getting random station
                        Current.Location = GetStationLocation(s.Id);
                        Current.Battery = r.Next(0, 21);
                        dal.ChargeDrone(d.Id, s.Id); //is this it?
                    }
                    else // if the drone's status is "Available"
                    {
                        var p = dal.Receivers();
                        int i = r.Next(0, p.Length);
                        var receiver = dal.Request<IDAL.DO.Customer>(p[i]); // getting a random receiver
                        Current.Location = GetStationLocation(receiver.Id);
                        double b = MinBattery(Location.distance(Current.Location, ClosestStation(Current.Location)), Current.Id);
                        Current.Battery = r.NextDouble() * (100 - b) + b; //random battery between minimum battery needed to 100
                    }
                }
            }
        }
    }
}

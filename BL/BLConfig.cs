using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;

namespace IBL.BO
{
    /// <summary>
    /// Partial BL class that configures a BL instance
    /// </summary>
    public partial class BL:IBL
    {
        private IDal dal;
        private List<DroneToList> Drones; //public?

        //static?
        public double AvailableUse; 
        public double LightUse; 
        public double MediumUse; 
        public double HeavyUse;
        public double ChargeRate;
        public static Random r = new Random();
        public double[] info;
        public BL() //BL constructor, initializes BL instance
        {
            dal = new IDAL.DalObject.DalObject();
            Drones = new List<DroneToList>();
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
                    Parcel p = Request<Parcel>((int)Current.ParcelId); //the parcel assigned to the current drone
                    Customer sender = Request<Customer>(p.Sender.Id); // the parcel's sender
                    Customer receiver = Request<Customer>(p.Receiver.Id);//thet parcel's receiver

                    double distance = Location.distance(sender.location, receiver.location); //distance from sender to receiver
                    distance += Location.distance(receiver.location, ClosestStation(receiver.location)); // + distance from receiver to closest station
                    if (p.PickedUp == null) //if the parcel isn't picked up yet
                    {
                        Current.Location = ClosestStation(GetCustomerLocation(sender.Id)); //change drone's location to sender's location
                        distance += Location.distance(Current.Location, sender.location); // + distance from initial location to sender
                    }
                    else
                        Current.Location = GetCustomerLocation(sender.Id);
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
                        var receiver = Request<Customer>(p[i]); // getting a random receiver
                        Current.Location = GetStationLocation(receiver.Id);
                        double b = MinBattery(Location.distance(Current.Location, ClosestStation(Current.Location)), Current.Id);
                        Current.Battery = r.NextDouble() * (100 - b) + b; //random battery between minimum battery needed to 100
                    }
                }
            }

            foreach (var d in Drones)
                Console.WriteLine(d);
        }
        /// <summary>
        /// function that checks if the drone is assigned to a parcel
        /// </summary>
        public bool isDroneAssigned(DroneToList d)
        {
            var p = dal.RequestList<IDAL.DO.Parcel>().ToList().Find(x => x.DroneId == d.Id);
            if (p.Delivered == null) //if the parcel isn't delivered yet
            {
                d.ParcelId = p.Id; //updating drone's parcel id, because it's 0 (we use this function when configuring the drone list)
                return true;
            }
            return false;
        }
    }
}

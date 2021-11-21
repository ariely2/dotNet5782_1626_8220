using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
namespace IBL.BO
{
    public partial class BL : IBL
    {
        public void AssignDrone(int id)
        {
           //Drone d = dal.Request<IDAL.DO.Drone>(id);
           //if(d.Status == DroneStatuses.Available)
        }

        #region Create
        public void Create<T>(T t) where T : class
        {
            try
            {
                switch (t)
                {
                    case Station s:
                        dal.Create<IDAL.DO.Station>(new IDAL.DO.Station()
                        {
                            Id = s.Id,
                            Name = s.Name,
                            ChargeSlots = s.AvailableSlots,
                            location = new IDAL.DO.Location()
                            {
                                Latitude = s.location.Latitude,
                                Longitude = s.location.Longitude
                            }
                        });
                        break;
                    case Customer c:
                        dal.Create<IDAL.DO.Customer>(new IDAL.DO.Customer()
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Phone = c.Phone,
                            location = new IDAL.DO.Location()
                            {
                                Latitude = c.location.Latitude,
                                Longitude = c.location.Longitude
                            }
                        });
                        break;
                    case Drone d:
                        dal.Create<IDAL.DO.Drone>(new IDAL.DO.Drone() //might need to also add drone to BL drone list
                        {
                            Id = d.Id,
                            Model = d.Model,
                            MaxWeight = (IDAL.DO.WeightCategories)d.MaxWeight, //is this ok?
                            
                        });
                        break;
                    case Parcel p:
                        dal.Create<IDAL.DO.Parcel>(new IDAL.DO.Parcel()
                        {
                            Id =p.Id,
                            SenderId = p.Sender.Id,
                            TargetId = p.Receiver.Id,
                            Weight = (IDAL.DO.WeightCategories)p.Weight,
                            Priority = (IDAL.DO.Priorities)p.Priority,
                            Requested = DateTime.Now,
                            Scheduled = DateTime.MinValue,
                            Delivered = DateTime.MinValue,
                            PickedUp = DateTime.MinValue
                        });
                        break;
                    default:
                        throw new NotExistClass("Class doesn't exist\n");
                }
            }
            catch
            {

            }
        }
        #endregion Create
        public void Deliver(int id)
        {
            DroneToList d = drones.Find(x => x.Id == id);
            if (d.ParcelId != 0)
            {
                var p = dal.Request<IDAL.DO.Parcel>(d.ParcelId);
                if (p.PickedUp != DateTime.MinValue && p.Delivered == DateTime.MinValue)
                {
                    //battery
                    p.Delivered = DateTime.Now;
                    var c = dal.Request<IDAL.DO.Customer>(p.TargetId);
                    d.Location.Latitude = c.location.Latitude;
                    d.Location.Longitude = c.location.Longitude;
                    d.Status = DroneStatuses.Available;
                }

            }
            //throw cant pick up?();
        }

        public void PickUp(int id)
        {
            DroneToList d = drones.Find(x => x.Id == id);
            if (d.ParcelId != 0)
            {
                var p = dal.Request<IDAL.DO.Parcel>(d.ParcelId);
                if (p.PickedUp == DateTime.MinValue)
                {
                    //battery
                    p.PickedUp = DateTime.Now;
                    var c = dal.Request<IDAL.DO.Customer>(p.SenderId);
                    d.Location.Latitude = c.location.Latitude;
                    d.Location.Longitude = c.location.Longitude;
                }

            }
            //throw cant pick up?();
        }

        public void ReleaseDrone(int id, double t)
        {
            throw new NotImplementedException();
        }


        #region Request
        public T Request<T>(int id) where T : class
        {
            T ans = default(T);
            try
            {
                switch (typeof(T).Name)
                {
                    case nameof(Station): //does this turn T to IDAL.DO.T type? if so, manually create BO.T and assign from dal.Request
                        ans = (T)Convert.ChangeType(dal.Request<IDAL.DO.Station>(id), typeof(T)); //add manually drone list?
                        break;
                    case nameof(Customer):
                        ans = (T)Convert.ChangeType(dal.Request<IDAL.DO.Customer>(id), typeof(T));
                        break;
                    case nameof(Drone):
                        ans = (T)Convert.ChangeType(dal.Request<IDAL.DO.Drone>(id), typeof(T)); //search in drone list instead
                        break;
                    case nameof(Parcel):
                        ans = (T)Convert.ChangeType(dal.Request<IDAL.DO.Parcel>(id), typeof(T));
                        break;
                    default:
                        throw new NotExistClass("struct doesn't exist\n");
                }
                if (ans.Equals(default(T)))
                    throw new NotExistId("id doesn't exist\n");
            }
            catch
            {

            }
            return ans;//is this ok?
            // throw new NotImplementedException();
        }

        public IEnumerable<T> RequestList<T>() where T : class
        {
            switch (typeof(T).Name)
            {
                case nameof(Station):
                    return (IEnumerable<T>)dal.RequestList<IDAL.DO.Station>();
                case nameof(Customer):
                    return (IEnumerable<T>)dal.RequestList<IDAL.DO.Customer>();
                case nameof(Drone):
                    return (IEnumerable<T>)dal.RequestList<IDAL.DO.Drone>();
                case nameof(Parcel):
                    return (IEnumerable<T>)dal.RequestList<IDAL.DO.Parcel>();
                default:
                    throw new NotExistClass("requested class doesn't exist\n");
            }
           // return null;
           // throw new NotImplementedException();
        }
        #endregion Request

        public void SendDroneToCharge(int id, int stationID = 0)
        {
            throw new NotImplementedException();
        }

        public void Update<T>(int id, T t) where T : class
        {
            throw new NotImplementedException();
        }
        public bool isDroneAssigned(DroneToList d)
        {
            var p = dal.RequestList<IDAL.DO.Parcel>();
            for(int i = 0; i < p.Count(); i++)
            {
                if(p.ElementAt(i).DroneId == d.Id) // if the drone is assigned to a parcel
                {
                    if (p.ElementAt(i).Delivered == DateTime.MinValue) // if the parcel isn't delivered yet
                    {
                        d.ParcelId = p.ElementAt(i).Id;
                        return true;
                    }
                }
            }
            return false;
        }

        public Location ClosestStation(Location d)
        {
            //var p = dal.Request<IDAL.DO.Parcel>(d.ParcelId); //the parcel assigned to the current drone
            var s = dal.RequestList<IDAL.DO.Station>();
            //var c = dal.Request<IDAL.DO.Customer>(p.SenderId);
            List<Station> stations = new List<Station>();
            foreach(var b in s)
            {
                stations.Add(new Station()
                {
                    location = new Location()
                    {
                        Latitude = b.location.Latitude,
                        Longitude = b.location.Longitude
                    }
                });
            }
            double distance = Location.distance(stations.First().location, d);
            int id = stations.First().Id;
            foreach (var b in stations)
            {
                if (Location.distance(b.location, d) < distance)
                {
                    distance = Location.distance(b.location, d);
                    id = b.Id;
                }
            }
            IDAL.DO.Location loc = dal.Request<IDAL.DO.Customer>(id).location;
            return new Location() { Latitude = loc.Latitude, Longitude = loc.Longitude };
        }

        public double MinBattery(double distance, int id)
        {
            DroneToList d = drones.Find(x => x.Id == id);
            if(d.Status == DroneStatuses.Delivery)
            {
                var p = dal.Request<IDAL.DO.Parcel>(d.ParcelId);
                return info[((int)p.Weight) + 1] * distance;
            }
            else
                return info[0] * distance;
        }

    }
}

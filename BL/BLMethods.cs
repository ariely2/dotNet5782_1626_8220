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
                        Random r = new Random();
                        drones.Add(new DroneToList()
                        {
                            Id = d.Id,
                            Battery = r.NextDouble()*20+20,//nubmer between 20 to 40, need to remove digit,
                            Location = d.Location,
                            MaxWeight = d.MaxWeight,
                            Model = d.Model,
                            Status = DroneStatuses.Maintenance,
                            ParcelId = 0
                        });
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
                    var s = dal.Request<IDAL.DO.Customer>(p.SenderId);
                    Location t = new Location() { Latitude = s.location.Latitude, Longitude = s.location.Longitude };
                    d.Battery = MinBattery(Location.distance(t, d.Location), d.Id);
                    var c = dal.Request<IDAL.DO.Customer>(p.TargetId);
                    p.Delivered = DateTime.Now;
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
                    Location t = new Location() { Latitude = c.location.Latitude, Longitude = c.location.Longitude };
                    d.Battery = MinBattery(Location.distance(t, d.Location), d.Id);
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
                        IDAL.DO.Station s = dal.Request<IDAL.DO.Station>(id);
                        ans = (T)Convert.ChangeType(new Station()
                        {
                            AvailableSlots = s.ChargeSlots,
                            Id = s.Id,
                            location = new Location() { Latitude = s.location.Latitude, Longitude = s.location.Longitude },
                            Name = s.Name,
                            Charging = drones.FindAll(d=>d.Location.Latitude == s.location.Latitude && d.Location.Longitude == s.location.Longitude).Select(d=>new DroneCharge() {Id = d.Id, Battery = d.Battery }).ToList()
                        }, typeof(T));
                        break;
                    case nameof(Customer):
                        IDAL.DO.Customer c = dal.Request<IDAL.DO.Customer>(id);
                        ans = (T)Convert.ChangeType(new Customer()
                        {
                            Id = c.Id,
                            location = new Location() { Latitude = c.location.Latitude, Longitude = c.location.Longitude },
                            Name = c.Name,
                            Phone = c.Phone,
                            To =   RequestList<Parcel>().ToList().FindAll(p => p.Receiver.Id == c.Id).Select(p => new ParcelAtCustomer() { Id = p.Id, Customer = new CustomerParcel() { Id = c.Id, Name = c.Name }, Priority = p.Priority, Status = p.Delivered == DateTime.MinValue ? (p.PickedUp == DateTime.MinValue ? (p.Scheduled == DateTime.MinValue ? ParcelStatuses.Created : ParcelStatuses.Assigned) : ParcelStatuses.PickedUp) : ParcelStatuses.Delivered, Weight = p.Weight }).ToList(),
                            From = RequestList<Parcel>().ToList().FindAll(p=> p.Sender.Id == c.Id).Select(p=> new ParcelAtCustomer() { Id = p.Id, Customer = new CustomerParcel() { Id = c.Id, Name = c.Name}, Priority = p.Priority, Status = p.Delivered == DateTime.MinValue ? (p.PickedUp == DateTime.MinValue ? (p.Scheduled == DateTime.MinValue ? ParcelStatuses.Created : ParcelStatuses.Assigned) : ParcelStatuses.PickedUp) : ParcelStatuses.Delivered, Weight = p.Weight }).ToList()
                        }, typeof(T));
                        break;
                    case nameof(Drone):
                        DroneToList d = drones.Find(b => b.Id == id);
                        ans = (T)Convert.ChangeType(new Drone()
                        {
                            Id = d.Id,
                            Battery = d.Battery,
                            Location = d.Location,
                            MaxWeight = d.MaxWeight,
                            Model = d.Model,
                            Parcel = null,//need to add parceldeliver
                            Status = d.Status
                        }, typeof(T));
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
                case nameof(StationToList):
                    return (IEnumerable<T>)dal.RequestList<IDAL.DO.Station>().Select(s => new StationToList()
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Available = 0,//?,
                        Occupied = 0//?
                    });

                case nameof(CustomerToList):
                    return (IEnumerable<T>)dal.RequestList<IDAL.DO.Customer>().Select(c => new CustomerToList()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Phone = c.Phone,
                        Delivered = 0,
                        NoDelivered = 0,
                        NoReceived = 0,
                        Received = 0
                    });

                case nameof(DroneToList):
                    return (IEnumerable<T>)drones;
                case nameof(ParcelToList):
                    return (IEnumerable<T>)dal.RequestList<IDAL.DO.Parcel>().Select(p => new ParcelToList
                    {
                        Id = p.Id,
                        Priority = (Priorities)Enum.Parse(typeof(Priorities),p.Priority.ToString()),
                        ReceiverName = Request<Customer>(p.TargetId).Name,
                        SenderName = Request<Customer>(p.SenderId).Name,
                        Status = p.Delivered==DateTime.MinValue?(p.PickedUp==DateTime.MinValue? (p.Scheduled==DateTime.MinValue?ParcelStatuses.Created:ParcelStatuses.Assigned):ParcelStatuses.PickedUp):ParcelStatuses.Delivered,
                        Weight = (WeightCategories)Enum.Parse(typeof(Priorities),p.Weight.ToString()),
                        
                    }) ;
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
            if(d.Status == DroneStatuses.Available)
                return info[0] * distance;
            else
            {
                var p = dal.Request<IDAL.DO.Parcel>(d.ParcelId);
                return info[((int)p.Weight) + 1] * distance;
            }
        }
    }
}

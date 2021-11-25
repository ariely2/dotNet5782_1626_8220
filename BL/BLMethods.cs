﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
namespace IBL.BO
{
    public partial class BL : IBL
    {
        public void AssignDrone(int id) //make this shorter!
        {
            //var d = dal.Request<IDAL.DO.Drone>(id); //update also in "drones"?
            DroneToList d = drones.Find(x => x.Id == id);
            if (d.Status == DroneStatuses.Available)
            {
                List<IDAL.DO.Parcel> AllParcels = dal.RequestList<IDAL.DO.Parcel>().ToList();
                AllParcels.RemoveAll(x => (int)x.Weight > (int)d.MaxWeight); //removing parcels that the drone can't take
                bool found = false;
                while (!found && AllParcels.Count()!=0)
                {
                    int max = AllParcels.Max(x => (int)x.Priority); //finding max priority that exists in parcel list   //might need to write var instead int?
                    var priority = AllParcels.Where(x => (int)x.Priority == max);
                    AllParcels.RemoveAll(x => (int)x.Priority == max); //removing parcels that don't have max priority
                    while(!found && priority.Count() !=0)
                    {
                        max = priority.Max(x => (int)x.Weight); //finding max weight that exists in parcel list
                        var weight = AllParcels.Where(x => (int)x.Weight == max).ToList();
                        AllParcels.RemoveAll(x => (int)x.Weight == max); //removing parcels that don't have max weight
                        weight.OrderByDescending(x => Location.distance(d.Location, GetCustomerLocation(x.SenderId)));
                        while (!found && weight.Count()!=0)
                        {
                            var best = weight.Last();
                            Location sender = GetCustomerLocation(best.SenderId);
                            Location receiver = GetCustomerLocation(best.TargetId);
                            double distance =
                                  Location.distance(d.Location, sender)
                                + Location.distance(sender, receiver)
                                + Location.distance(receiver, ClosestStation(receiver));
                            double min = info[((int)best.Weight) + 1] * distance;
                            if (min >= d.Battery)
                            {
                                found = true;
                                d.Status = DroneStatuses.Delivery;
                                var selected = dal.Request<IDAL.DO.Parcel>(best.Id);
                                d.ParcelId = selected.Id;
                                selected.DroneId = d.Id; //does it change this also in IDAL or only in the function?
                                selected.Scheduled = DateTime.Now;
                            }
                            else
                                weight.Remove(best);
                        }
                    }
                }
               // if(!found) // if there's no parcel the drone can take
                   // throw
            }
             //else
                   //  throw something?
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
                            Battery = r.NextDouble() * 20 + 20,//nubmer between 20 to 40, need to remove digit,
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
                            Id = p.Id,
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
                        throw new NotExistClassException("Class doesn't exist\n");
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
            DroneToList d = drones.Find(x => x.Id == id);
            if (d.Status == DroneStatuses.Maintenance)
            {

            }
            //else
                //throw new NotImplementedException();
        }


        #region Request
        //need to make the code look better
        public T Request<T>(int id) where T : class
        {
            T ans = default(T);
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
                        Charging = drones.FindAll(d => d.Location.Equals(s.location)).Select(d => new DroneCharge() { Id = d.Id, Battery = d.Battery }).ToList()
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
                        To = RequestList<Parcel>().ToList().FindAll(p => p.Receiver.Id == c.Id).Select(p => new ParcelAtCustomer() { Id = p.Id, Customer = new CustomerParcel() { Id = c.Id, Name = c.Name }, Priority = p.Priority, Status = p.Delivered == DateTime.MinValue ? (p.PickedUp == DateTime.MinValue ? (p.Scheduled == DateTime.MinValue ? ParcelStatuses.Created : ParcelStatuses.Assigned) : ParcelStatuses.PickedUp) : ParcelStatuses.Delivered, Weight = p.Weight }).ToList(),
                        From = RequestList<Parcel>().ToList().FindAll(p => p.Sender.Id == c.Id).Select(p => new ParcelAtCustomer() { Id = p.Id, Customer = new CustomerParcel() { Id = c.Id, Name = c.Name }, Priority = p.Priority, Status = p.Delivered == DateTime.MinValue ? (p.PickedUp == DateTime.MinValue ? (p.Scheduled == DateTime.MinValue ? ParcelStatuses.Created : ParcelStatuses.Assigned) : ParcelStatuses.PickedUp) : ParcelStatuses.Delivered, Weight = p.Weight }).ToList()
                    }, typeof(T));
                    break;
                case nameof(Drone):
                    DroneToList d = drones.Find(b => b.Id == id);
                    Parcel p = Request<Parcel>(d.ParcelId);
                    ans = (T)Convert.ChangeType(new Drone()
                    {
                        Id = d.Id,
                        Battery = d.Battery,
                        Location = d.Location,
                        MaxWeight = d.MaxWeight,
                        Model = d.Model,
                        //check if the drone status is delivery
                        Parcel = d.Status == DroneStatuses.Delivery ? new ParcelDeliver()
                        {
                            Id = d.ParcelId,
                            Priority = p.Priority,
                            Receiver = p.Receiver,
                            Sender = p.Sender,
                            Status = p.Delivered != DateTime.MinValue,
                            Destination = Request<Customer>(p.Receiver.Id).location,
                            Source = Request<Customer>(p.Sender.Id).location,
                            Weight = p.Weight,
                            Distance = Location.distance(Request<Customer>(p.Receiver.Id).location, Request<Customer>(p.Sender.Id).location)
                        }:null,
                        Status = d.Status
                    }, typeof(T));
                    break;
                case nameof(Parcel):
                    IDAL.DO.Parcel pi = dal.Request<IDAL.DO.Parcel>(id);
                    DroneToList a = drones.Find(d => d.Id == pi.DroneId);
                    ans = (T)Convert.ChangeType(new Parcel()
                    {
                        Id = pi.Id,
                        Drone = new DroneParcel() { Id = a.Id, Baterry = a.Battery, Location = a.Location },
                        Priority = (Priorities)pi.Priority,
                        Weight = (WeightCategories)pi.Weight,
                        Receiver = new CustomerParcel() { Id = pi.TargetId, Name = Request<Customer>(pi.TargetId).Name },
                        Sender = new CustomerParcel() { Id = pi.SenderId, Name = Request<Customer>(pi.SenderId).Name },
                        Delivered = pi.Delivered,
                        PickedUp = pi.PickedUp,
                        Requested = pi.Requested,
                        Scheduled = pi.Scheduled
                    }, typeof(T));
                    break;
                default:
                    throw new NotExistClassException("class doesn't exist\n");
            }
            if (ans.Equals(default(T)))
                throw new NotExistIdException("id doesn't exist\n");
            return ans;
        }
                
        //need to make the code look better
        public IEnumerable<T> RequestList<T>() where T : class
        {
            switch (typeof(T).Name)
            {
                case nameof(StationToList):
                    return (IEnumerable<T>)dal.RequestList<IDAL.DO.Station>().Select(s => new StationToList()
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Available = s.ChargeSlots,
                        Occupied = drones.FindAll(d=>d.Status==DroneStatuses.Maintenance&& d.Location.Equals(s.location)).Count()//?
                    }) ;

                case nameof(CustomerToList):
                    return (IEnumerable<T>)dal.RequestList<IDAL.DO.Customer>().Select(c => new CustomerToList()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Phone = c.Phone,
                        Delivered = RequestList<Parcel>().ToList().FindAll(p=> p.Sender.Id == c.Id && p.Delivered!=DateTime.MinValue).Count(),
                        NoDelivered = RequestList<Parcel>().ToList().FindAll(p=>p.Sender.Id == c.Id && p.Delivered == DateTime.MinValue).Count(),
                        NoReceived = RequestList<Parcel>().ToList().FindAll(p=>p.Receiver.Id == c.Id && p.Delivered != DateTime.MinValue).Count(),
                        Received = RequestList<Parcel>().ToList().FindAll(p=>p.Receiver.Id == c.Id && p.Delivered == DateTime.MinValue).Count()
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
                    throw new NotExistClassException("requested class doesn't exist\n");
            }
        }
        #endregion Request

        #region Update
        public void SendDroneToCharge(int id, int stationID = 0)
        {
            DroneToList d = drones.Find(x => x.Id == id);
            if(d.Status == DroneStatuses.Available)
            {
                var stations = dal.RequestList<IDAL.DO.Station>().ToList();
                stations.RemoveAll(x => x.ChargeSlots == 0); //removing stations with no available charge slots
                stations.OrderByDescending(x => Location.distance(d.Location, GetStationLocation(x.Id)));
                bool found = false;
                while(!found && stations.Count!=0)
                {
                    Location s = GetStationLocation(stations.Last().Id);
                    double distance = Location.distance(d.Location, s);
                    if (MinBattery(distance, d.Id) <= d.Battery)
                    {
                        found = true;
                        d.Battery = MinBattery(distance, d.Id);
                        d.Location = s;
                        d.Status = DroneStatuses.Maintenance;
                        dal.ChargeDrone(d.Id, stations.Last().Id); //is this it?
                    }
                    else
                        stations.RemoveAt(stations.Count - 1);
                }
                //if(!found) //if there's no station the drone can go to
                    //throw
            }
            //else
                //throw new NotImplementedException();
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


        #endregion Update

        public double MinBattery(double distance, int id)
        {
            DroneToList d = drones.Find(x => x.Id == id);
            if (d.Status == DroneStatuses.Available)
                return info[0] * distance;
            else
            {
                var p = dal.Request<IDAL.DO.Parcel>(d.ParcelId);
                return info[((int)p.Weight) + 1] * distance;
            }
        }
        public Location GetCustomerLocation(int id) //get customers location
        {
            var c = dal.Request<IDAL.DO.Customer>(id);
            return new Location() { Latitude = c.location.Latitude, Longitude = c.location.Longitude };
        }
        public Location GetStationLocation(int id) //get station's location
        {
            var s = dal.Request<IDAL.DO.Station>(id);
            return new Location() { Latitude = s.location.Latitude, Longitude = s.location.Longitude };
        }
    }

}

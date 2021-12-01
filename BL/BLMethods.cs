using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
namespace IBL.BO
{ //change IDAL request to request?
    /// <summary>
    /// Partial BL class that contains all methods necessary for BL
    /// </summary>
    public partial class BL : IBL
    {
        
        #region Create
        /// <summary>
        /// generic function that creates new objects
        /// </summary>
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
                        dal.Create<IDAL.DO.Drone>(new IDAL.DO.Drone()
                        {
                            Id = d.Id,
                            Model = d.Model,
                            MaxWeight = (IDAL.DO.WeightCategories)d.MaxWeight,
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
                        location = GetStationLocation(s.Id),
                        Name = s.Name,
                        //getting a list of all drones charging at s, and making a list of DroneCharge based on the drone list
                        Charging = 
                        drones.FindAll(d => d.Location.Equals(s.location)) 
                        .Select(d => new DroneCharge() { Id = d.Id, Battery = d.Battery }).ToList()
                    }, typeof(T));
                    break;
                case nameof(Customer):
                    IDAL.DO.Customer c = dal.Request<IDAL.DO.Customer>(id);
                    ans = (T)Convert.ChangeType(new Customer()
                    {
                        Id = c.Id,
                        location = GetCustomerLocation(c.Id),
                        Name = c.Name,
                        Phone = c.Phone,
                        To = RequestList<Parcel>().ToList() //list of all parcels sent to customer
                        .FindAll(p => p.Receiver.Id == c.Id)
                        .Select(p => new ParcelAtCustomer() {
                            Id = p.Id,
                            Customer = new CustomerParcel() { Id = c.Id, Name = c.Name },
                            Priority = p.Priority,
                            Status = p.Delivered == DateTime.MinValue ? (p.PickedUp == DateTime.MinValue ? (p.Scheduled == DateTime.MinValue ? ParcelStatuses.Created : ParcelStatuses.Assigned) : ParcelStatuses.PickedUp) : ParcelStatuses.Delivered,
                            Weight = p.Weight }).ToList(),

                        From = RequestList<Parcel>().ToList() // list of all parcels sent from customer
                        .FindAll(p => p.Sender.Id == c.Id)
                        .Select(p => new ParcelAtCustomer() {
                            Id = p.Id,
                            Customer = new CustomerParcel() { Id = c.Id, Name = c.Name },
                            Priority = p.Priority,
                            Status = p.Delivered == DateTime.MinValue ? (p.PickedUp == DateTime.MinValue ? (p.Scheduled == DateTime.MinValue ? ParcelStatuses.Created : ParcelStatuses.Assigned) : ParcelStatuses.PickedUp) : ParcelStatuses.Delivered,
                            Weight = p.Weight }).ToList()

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
                        Parcel = d.Status == DroneStatuses.Delivery ? new ParcelDeliver()
                        { //if drone is delivering a parcel, create a ParcelDeliver instance
                            Id = d.ParcelId,
                            Priority = p.Priority,
                            Receiver = p.Receiver,
                            Sender = p.Sender,
                            Status = (EnumParcelDeliver)(p.PickedUp == DateTime.MinValue?0:1),
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

        public void AssignDrone(int id) 
        {
            //var d = dal.Request<IDAL.DO.Drone>(id); //update also in "drones"?
            DroneToList d = drones.Find(x => x.Id == id);
            if (d.Status == DroneStatuses.Available)
            {
                List<IDAL.DO.Parcel> AllParcels = dal.RequestList<IDAL.DO.Parcel>().ToList(); //getting list of all parcels
                AllParcels.RemoveAll(x => (int)x.Weight > (int)d.MaxWeight); //removing parcels that the drone can't take
                //does this delete parcels from DAL? 
                bool found = false;
                while (!found && AllParcels.Count() != 0) //while there are potential parcels left
                {
                    int max = AllParcels.Max(x => (int)x.Priority); //finding max priority that exists in parcel list
                    var priority = AllParcels.Where(x => (int)x.Priority == max); //getting list of parcels with max priority
                    AllParcels.RemoveAll(x => (int)x.Priority == max); //removing parcels that don't have max priority (because we moved them to another list)
                    while (!found && priority.Count() != 0) //while there are potential parcels with max priority left
                    {
                        max = priority.Max(x => (int)x.Weight); //finding max weight that exists in parcels with max priority list
                        var weight = AllParcels.Where(x => (int)x.Weight == max).ToList(); // getting list of parcels with max weight and priority
                        AllParcels.RemoveAll(x => (int)x.Weight == max); //removing parcels that don't have max weight
                        weight.OrderByDescending(x => Location.distance(d.Location, GetCustomerLocation(x.SenderId))); //sorting list by distance from drone to parcel's sender
                        while (!found && weight.Count() != 0) //while there are potential parcels with max priority and weight left
                        {
                            var best = weight.Last(); //geting parcel with shortest distance from drone to parcel's sender from list
                            Location sender = GetCustomerLocation(best.SenderId);
                            Location receiver = GetCustomerLocation(best.TargetId);
                            double distance =
                                  Location.distance(d.Location, sender)
                                + Location.distance(sender, receiver)
                                + Location.distance(receiver, ClosestStation(receiver));
                            double min = info[((int)best.Weight) + 1] * distance; //getting minimum battery required
                            if (min <= d.Battery) //checking if drone has enough battery
                            {
                                found = true;
                                d.Status = DroneStatuses.Delivery;
                                var selected = dal.Request<IDAL.DO.Parcel>(best.Id);
                                d.ParcelId = selected.Id;
                                selected.DroneId = d.Id; //does it change this also in IDAL or only in the function?
                                selected.Scheduled = DateTime.Now;
                            }
                            else
                                weight.Remove(best); //if the drone doesn't have enough battery to deliver best parcel we can find, remove it from list 
                        }
                    }
                }
                // if(!found) // if there's no parcel the drone can take
                // throw
            }
            //else
            //  throw something?
        }
        public void Deliver(int id)
        {
            DroneToList d = drones.Find(x => x.Id == id);
            if (d.ParcelId != 0) //if there's a parcel assigned to the drone
            {
                var p = dal.Request<IDAL.DO.Parcel>(d.ParcelId);
                if (p.PickedUp != DateTime.MinValue && p.Delivered == DateTime.MinValue) //if the parcel is picked up but not delivered yet
                {
                    var s = dal.Request<IDAL.DO.Customer>(p.SenderId);
                    Location t = GetCustomerLocation(s.Id);
                    p.Delivered = DateTime.Now;
                    d.Battery -= MinBattery(Location.distance(t, d.Location), d.Id); //updating drone's battery
                    var c = dal.Request<IDAL.DO.Customer>(p.TargetId);
                    d.Location = GetCustomerLocation(c.Id); //updating drone's location
                    d.Status = DroneStatuses.Available;
                }
            }
            //throw cant pick up?();
        }

        public void PickUp(int id)
        {
            DroneToList d = drones.Find(x => x.Id == id);
            if (d.ParcelId != 0) //if there's a parcel assigned to the drone
            {
                var p = dal.Request<IDAL.DO.Parcel>(d.ParcelId);
                if (p.PickedUp == DateTime.MinValue) // if the parcel isn't picked up yet
                {
                    p.PickedUp = DateTime.Now;
                    var c = dal.Request<IDAL.DO.Customer>(p.SenderId);
                    Location t = GetCustomerLocation(c.Id);
                    d.Battery -= MinBattery(Location.distance(t, d.Location), d.Id); //updating drone's battery
                    d.Location = GetCustomerLocation(c.Id);
                }
            }
            //throw cant pick up?();
        }

        public void ReleaseDrone(int id, double t)
        {
            DroneToList d = drones.Find(x => x.Id == id);
            if (d.Status == DroneStatuses.Maintenance) //if drone is charging 
            {
                d.Status = DroneStatuses.Available;
                d.Battery += info[4] * t;//assuming t is time in hours //updating drone's battery
                dal.ReleaseDrone(d.Id);
            }
            //else
                //throw new NotImplementedException();
        }
        public void SendDroneToCharge(int id)
        {
            DroneToList d = drones.Find(x => x.Id == id);
            if(d.Status == DroneStatuses.Available)
            {
                var stations = dal.RequestList<IDAL.DO.Station>().ToList(); //getting list of all stations
                stations.RemoveAll(x => x.ChargeSlots == 0); //removing stations with no available charge slots
                stations.OrderByDescending(x => Location.distance(d.Location, GetStationLocation(x.Id))); //sorting station list by distance from drone to station
                bool found = false;
                while(!found && stations.Count!=0) //while there are stations left in list
                {
                    Location s = GetStationLocation(stations.Last().Id);
                    double distance = Location.distance(d.Location, s);
                    if (MinBattery(distance, d.Id) <= d.Battery) //if drone has enough battery to get to station
                    {
                        found = true;
                        d.Battery = MinBattery(distance, d.Id);
                        d.Location = s;
                        d.Status = DroneStatuses.Maintenance;
                        dal.ChargeDrone(d.Id, stations.Last().Id); //is this it?
                    }
                    else
                        stations.RemoveAt(stations.Count - 1); //if the drone doesn't have enough battery to get to station, remove it from list 
                }
                //if(!found) //if there's no station the drone can go to
                    //throw
            }
            //else
                //throw new NotImplementedException();
        }

        public void UpdateStation(int id, string name = null, int? slots = null)
        {
            IDAL.DO.Station s= dal.Request<IDAL.DO.Station>(id); //getting station
            dal.Delete<IDAL.DO.Station>(id); //deleting old station
            dal.Create<IDAL.DO.Station>(new IDAL.DO.Station()
            {
                Id = id,
                Name = name == null ? s.Name : name,
                location = s.location,
                ChargeSlots = (int)(slots == null ? s.ChargeSlots : (slots - Request<Station>(id).Charging.Count()))
            }); //creating updated station
        }
        public void UpdateDrone(int id, string model = null)
        {
            IDAL.DO.Drone d = dal.Request<IDAL.DO.Drone>(id); //getting drone
            dal.Delete<IDAL.DO.Drone>(id); //deleting old drone
            dal.Create<IDAL.DO.Drone>(new IDAL.DO.Drone()
            {
                Id = id,
                MaxWeight = d.MaxWeight,
                Model = model == null ? d.Model : model
            }); //creating updated drone
        }
        public void UpdateCustomer(int id, string name = null, string phone = null)
        {
            IDAL.DO.Customer c = dal.Request<IDAL.DO.Customer>(id); //getting customer
            dal.Delete<IDAL.DO.Customer>(id); //deleting old customer
            dal.Create<IDAL.DO.Customer>(new IDAL.DO.Customer()
            {
                Id = id,
                location = c.location,
                Name = name == null ? c.Name : name,
                Phone = phone == null? c.Phone:phone
            }); //creating updated customer
        }



        #endregion Update

        public bool isDroneAssigned(DroneToList d)
        {
            var p = dal.RequestList<IDAL.DO.Parcel>(); //getting list of all parcels
            for (int i = 0; i < p.Count(); i++) 
            {
                if (p.ElementAt(i).DroneId == d.Id) // if the drone is assigned to a parcel
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
            var s = dal.RequestList<IDAL.DO.Station>(); //getting list of all stations
            double distance = Location.distance(GetStationLocation(s.First().Id), d);
            int id = s.First().Id;
            foreach (var b in s)
            {
                if (Location.distance(GetStationLocation(b.Id), d) < distance) //if station is closer than current closest station
                {
                    distance = Location.distance(GetStationLocation(b.Id), d); //update shortest distance and id of closest station
                    id = b.Id;
                }
            }
            IDAL.DO.Location loc = dal.Request<IDAL.DO.Customer>(id).location; 
            return new Location() { Latitude = loc.Latitude, Longitude = loc.Longitude };
        }
        public double MinBattery(double distance, int id)
        {
            DroneToList d = drones.Find(x => x.Id == id);
            if (d.Status == DroneStatuses.Available) //if drone is available, return corresponding battery per entered distance
                return info[0] * distance; 
            else //if a parcel is assigned to drone
            {
                var p = dal.Request<IDAL.DO.Parcel>(d.ParcelId);
                if(p.Delivered == DateTime.MinValue) //if parcel wasn't delivered yet (distance is the distance to pick up parcel)
                    return info[0] * distance;
                else // distance is distance to deliver parcel
                    return info[((int)p.Weight) + 1] * distance; //return battery corresponding to parcel's weight and distance
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

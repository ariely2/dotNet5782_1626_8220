﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace IBL.BO
{ 
    /// <summary>
    /// Partial BL class that contains all methods necessary for BL
    /// </summary>
    public partial class BL : IBL
    {

        #region Create
        /// <summary>
        /// generic function that creates new objects( station, customer, drone, parcel)
        /// </summary>
        public void Create<T>(T t) where T : class
        {
            try
            {
                switch (t)
                {
                    case Station s:

                        //check if there is a station in the exact location
                        if (RequestList<Station>().ToList().Exists(x => x.location.Equals(s.location)))
                            throw new AlreadyExistLocationException("Station already exist in the exact location\n");

                        //don't know if we need to check this
                        //check if there is a custoner in the exact location
                        if (RequestList<Customer>().ToList().Exists(x => x.location.Equals(s.location)))
                            throw new AlreadyExistLocationException("Customer already exist in the exact location\n");


                        //create station in dal
                        dal.Create<IDAL.DO.Station>(new IDAL.DO.Station()
                        {
                            Id = s.Id,
                            Name = s.Name,
                            ChargeSlots = s.AvailableSlots,
                            Location = new IDAL.DO.Location()
                            {
                                Latitude = s.location.Latitude,
                                Longitude = s.location.Longitude
                            }
                        });
                        break;

                    case Customer c:
                        //check if there is a customer in the exact location
                        if (RequestList<Customer>().ToList().Exists(x => x.location.Equals(c.location)))
                            throw new AlreadyExistLocationException("Customer already exist in the exact locatoin\n");
                        //dont know if we need to check this
                        //check if there is a station in the exact location
                        if (RequestList<Station>().ToList().Exists(x => x.location.Equals(c.location)))
                            throw new AlreadyExistLocationException("Station already exist in the exact location\n");


                        //create customer in dal
                        dal.Create<IDAL.DO.Customer>(new IDAL.DO.Customer()
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Phone = c.Phone,
                            Location = new IDAL.DO.Location()
                            {
                                Latitude = c.location.Latitude,
                                Longitude = c.location.Longitude
                            }
                        });
                        break;

                    case Drone d:
                        Random r = new Random();


                        Station station = RequestList<Station>().ToList().Find(s => s.location.Equals(d.Location));
                        //check if the drone location is the same as exist station
                        if (station.Equals(default(Station)))
                            throw new NotExistException("There is no station in this location\n");
                        //check if the station have place for the drone
                        if (station.AvailableSlots == 0)
                            throw new NotPossibleStationException("This station have no enough place\n");

                        //create the drone in dal
                        dal.Create<IDAL.DO.Drone>(new IDAL.DO.Drone()
                        {
                            Id = d.Id,
                            Model = d.Model,
                            MaxWeight = (IDAL.DO.WeightCategories)d.MaxWeight,
                        });

                        //if the id already exist, dal.create would throw an exception
                        //create the drone in bl
                        Drones.Add(new DroneToList()
                        {
                            Id = d.Id,
                            Battery = r.NextDouble() * 20 + 20,//nubmer between 20 to 40
                            Location = d.Location,
                            MaxWeight = d.MaxWeight,
                            Model = d.Model,
                            Status = DroneStatuses.Maintenance,
                            ParcelId = 0
                        });

                        //update station info, reduce available slots by 1
                        UpdateStation(station.Id, null, station.Charging.Count() + station.AvailableSlots);
                        break;

                    case Parcel p:
                        dal.Create<IDAL.DO.Parcel>(new IDAL.DO.Parcel()
                        {
                            Id = p.Id,
                            SenderId = p.Sender.Id,
                            TargetId = p.Receiver.Id,
                            Weight = (IDAL.DO.WeightCategories)p.Weight,
                            Priority = (IDAL.DO.Priorities)p.Priority,
                            DroneId = null,
                            Requested = DateTime.Now,
                            Scheduled = null,
                            Delivered = null,
                            PickedUp = null
                        });
                        break;
                    default:
                        throw new NotSupportException("Not support " + typeof(T).Name + '\n');
                }
            }
            //id already exist, need to convert the exception from dal to bl
            catch (IDAL.DO.ExistIdException ex)
            {
                throw new ExistIdException(ex.Message, ex);
            }
            //id out of bounds, need to convert the exception from dal to bl
            catch (IDAL.DO.IdOutOfBoundsException ex)
            {
                throw new IdOutOfBoundsExceptoin(ex.Message, ex);
            }
            //not support struct, need to convert the exception from dal to bl
            //this catch is not supposed to happen but just in case if there is a change in dal
            catch (IDAL.DO.NotSupportException ex)
            {
                throw new NotSupportException(ex.Message, ex);
            }
            //phone number of customer already exist
            catch (IDAL.DO.ExistPhoneException ex)
            {
                throw new ExistPhoneException(ex.Message, ex);
            }
            //the station have no place for additional drone
            catch(IDAL.DO.NotPossibleStationException ex)
            {
                throw new NotPossibleStationException(ex.Message, ex);
            }
            
        }


        #endregion Create

        #region Request
        /// <summary>
        /// the function return object from type T with the same id
        /// </summary>
        /// <typeparam name="T">type of object to return </typeparam>
        /// <param name="id">id of the object</param>
        /// <returns>the requested object</returns>
        public T Request<T>(int id) where T : class
        {
            T ans = default(T);
            try {
                switch (typeof(T).Name)
                {
                    case nameof(Station):

                        //get a IDAL.DO.station with this id, convert it to a BL.BO.station
                        IDAL.DO.Station s = dal.Request<IDAL.DO.Station>(id);
                        ans = (T)Convert.ChangeType(new Station()
                        {
                            AvailableSlots = s.ChargeSlots,
                            Id = s.Id,
                            location = GetStationLocation(s.Id),
                            Name = s.Name,
                            //getting a list of all drones charging at s, and making a list of DroneCharge based on the drone list, can be null
                            Charging = Drones.FindAll(d =>d.Status==DroneStatuses.Maintenance && d.Location.Equals(s.Location))
                                             .Select(d => new DroneCharge() { Id = d.Id, Battery = d.Battery }).ToList()
                        }, typeof(T));
                        break;


                    case nameof(Customer):
                        //get a customer from dal and convert it to cutomer of bl
                        IDAL.DO.Customer c = dal.Request<IDAL.DO.Customer>(id);
                        ans = (T)Convert.ChangeType(new Customer()
                        {
                            Id = c.Id,
                            location = GetCustomerLocation(c.Id),
                            Name = c.Name,
                            Phone = c.Phone,
                            To = RequestList<Parcel>().ToList() //list of all parcels sent to customer, can be empty
                            .FindAll(p => p.Receiver.Id == c.Id)
                            .Select(p => new ParcelAtCustomer()
                            {
                                Id = p.Id,
                                Customer = new CustomerParcel() { Id = c.Id, Name = c.Name },
                                Priority = p.Priority,
                                Status = EnumExtension.GetStatus(p.Delivered, p.PickedUp, p.Scheduled, p.Requested),
                                Weight = p.Weight

                            }).ToList(),

                            From = RequestList<Parcel>().ToList() // list of all parcels sent from customer, can be empty
                            .FindAll(p => p.Sender.Id == c.Id)
                            .Select(p => new ParcelAtCustomer()
                            {
                                Id = p.Id,
                                Customer = new CustomerParcel() { Id = c.Id, Name = c.Name },
                                Priority = p.Priority,
                                Status = EnumExtension.GetStatus(p.Delivered, p.PickedUp, p.Scheduled, p.Requested),
                                Weight = p.Weight
                            }).ToList()

                        }, typeof(T));
                        break;
                    case nameof(Drone):
                        DroneToList d = Drones.Find(b => b.Id == id);
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
                                Status = (EnumParcelDeliver)(p.PickedUp == DateTime.MinValue ? 0 : 1),
                                Destination = Request<Customer>(p.Receiver.Id).location,
                                Source = Request<Customer>(p.Sender.Id).location,
                                Weight = p.Weight,
                                Distance = Location.distance(Request<Customer>(p.Receiver.Id).location, Request<Customer>(p.Sender.Id).location)
                            } : null,
                            Status = d.Status
                        }, typeof(T));
                        break;
                    case nameof(Parcel):
                        IDAL.DO.Parcel pi = dal.Request<IDAL.DO.Parcel>(id);
                        DroneToList a = Drones.Find(d => d.Id == pi.DroneId);
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
                        throw new NotSupportException("Not support " + typeof(T).Name + '\n');
                }
            }
            catch (NotSupportException ex)
            {
                throw new NotSupportException(ex.Message, ex);
            }
            catch (IDAL.DO.NotExistException ex)
            {
                throw new NotExistException(typeof(T).Name + " with id of: " + id + " isn't exist\n", ex);
            }
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
                        Occupied = Drones.FindAll(d => d.Status == DroneStatuses.Maintenance && d.Location.Equals(s.Location)).Count()//?
                    });

                case nameof(CustomerToList):
                    return (IEnumerable<T>)dal.RequestList<IDAL.DO.Customer>().Select(c => new CustomerToList()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Phone = c.Phone,
                        Delivered = RequestList<Parcel>().ToList().FindAll(p => p.Sender.Id == c.Id && p.Delivered != DateTime.MinValue).Count(),
                        NoDelivered = RequestList<Parcel>().ToList().FindAll(p => p.Sender.Id == c.Id && p.Delivered == DateTime.MinValue).Count(),
                        NoReceived = RequestList<Parcel>().ToList().FindAll(p => p.Receiver.Id == c.Id && p.Delivered != DateTime.MinValue).Count(),
                        Received = RequestList<Parcel>().ToList().FindAll(p => p.Receiver.Id == c.Id && p.Delivered == DateTime.MinValue).Count()
                    });

                case nameof(DroneToList):
                    return (IEnumerable<T>)Drones;
                case nameof(ParcelToList):
                    return (IEnumerable<T>)dal.RequestList<IDAL.DO.Parcel>().Select(p => new ParcelToList
                    {
                        Id = p.Id,
                        Priority = (Priorities)Enum.Parse(typeof(Priorities), p.Priority.ToString()),
                        ReceiverName = Request<Customer>(p.TargetId).Name,
                        SenderName = Request<Customer>(p.SenderId).Name,
                        Status = p.Delivered == DateTime.MinValue ? (p.PickedUp == DateTime.MinValue ? (p.Scheduled == DateTime.MinValue ? ParcelStatuses.Created : ParcelStatuses.Assigned) : ParcelStatuses.PickedUp) : ParcelStatuses.Delivered,
                        Weight = (WeightCategories)Enum.Parse(typeof(Priorities), p.Weight.ToString()),

                    });
                default:
                    throw new NotSupportException("Not support " + typeof(T).Name + '\n');
            }
        }
        #endregion Request

        #region Update

        /// <summary>
        /// A function that assigns a drone to deliver a parcel, and finds the best parcel for the drone
        /// according to the requirements
        /// </summary>
        public void AssignDrone(int id) 
        {
            //updates also in "drones"?
            DroneToList d = Drones.Find(x => x.Id == id);
            if (d.Status == DroneStatuses.Available)
            {
                List<Parcel> AllParcels = RequestList<Parcel>().ToList(); //getting list of all parcels
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
                        weight.OrderByDescending(x => Location.distance(d.Location, GetCustomerLocation(x.Sender.Id))); //sorting list by distance from drone to parcel's sender
                        while (!found && weight.Count() != 0) //while there are potential parcels with max priority and weight left
                        {
                            var best = weight.Last(); //geting parcel with shortest distance from drone to parcel's sender from list
                            Location sender = GetCustomerLocation(best.Sender.Id);
                            Location receiver = GetCustomerLocation(best.Receiver.Id);
                            double distance =
                                  Location.distance(d.Location, sender)
                                + Location.distance(sender, receiver)
                                + Location.distance(receiver, ClosestStation(receiver));
                            double min = info[((int)best.Weight) + 1] * distance; //getting minimum battery required
                            if (min <= d.Battery) //checking if drone has enough battery
                            {
                                found = true;
                                d.Status = DroneStatuses.Delivery;
                                Parcel selected = Request<Parcel>(best.Id);
                                d.ParcelId = selected.Id;
                                dal.AssignParcel(selected.Id, d.Id);
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

        /// <summary>
        /// function that makes the drone deliver the parcel to the receiver
        /// </summary>
        /// <param name="id"></param>
        public void Deliver(int id)
        {
            DroneToList d = Drones.Find(x => x.Id == id);
            if (d.ParcelId != 0) //if there's a parcel assigned to the drone
            {
                Parcel p = Request<Parcel>(d.ParcelId);
                if (p.PickedUp != DateTime.MinValue && p.Delivered == DateTime.MinValue) //if the parcel is picked up but not delivered yet
                {
                    Customer s = Request<Customer>(p.Sender.Id);
                    Location t = GetCustomerLocation(s.Id);
                    dal.DeliverParcel(p.Id);
                    d.Battery -= MinBattery(Location.distance(t, d.Location), d.Id); //updating drone's battery
                    Customer r = Request<Customer>(p.Receiver.Id);
                    d.Location = GetCustomerLocation(r.Id); //updating drone's location
                    d.Status = DroneStatuses.Available;
                }
            }
            //throw cant pick up?();
        }

        /// <summary>
        /// function that makes the drone pick up the parcel from the sender
        /// </summary>
        /// <param name="id"></param>
        public void PickUp(int id)
        {
            DroneToList d = Drones.Find(x => x.Id == id);
            if (d.ParcelId != 0) //if there's a parcel assigned to the drone
            {
                Parcel p = Request<Parcel>(d.ParcelId);
                if (p.PickedUp == DateTime.MinValue) // if the parcel isn't picked up yet
                {
                    dal.PickUpParcel(p.Id);
                    Customer s = Request<Customer>(p.Sender.Id);
                    Location t = GetCustomerLocation(s.Id);
                    d.Battery -= MinBattery(Location.distance(t, d.Location), d.Id); //updating drone's battery
                    d.Location = GetCustomerLocation(s.Id);
                }
            }
            //throw cant pick up?();
        }

        /// <summary>
        /// function that releases drone from charging
        /// </summary>
        public void ReleaseDrone(int id, double t)
        {
            DroneToList d = Drones.Find(x => x.Id == id);
            if (d.Status == DroneStatuses.Maintenance) //if drone is charging 
            {
                d.Status = DroneStatuses.Available;
                d.Battery += info[4] * t;//assuming t is time in hours //updating drone's battery
                dal.ReleaseDrone(d.Id);
            }
            //else
                //throw new NotImplementedException();
        }

        /// <summary>
        /// function that sends the drone to charge
        /// </summary>
        public void SendDroneToCharge(int id)
        {
            DroneToList d = Drones.Find(x => x.Id == id);
            if (d.Status == DroneStatuses.Available)
            {
                var stations = RequestList<Station>().ToList();
                stations.RemoveAll(x => x.AvailableSlots == 0); //removing stations with no available charge slots
                stations.OrderByDescending(x => Location.distance(d.Location, x.location)); //sorting station list by distance from drone to station
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

        /// <summary>
        /// function that updates station's details
        /// </summary>
        public void UpdateStation(int id, string name = null, int? slots = null)
        {
            IDAL.DO.Station s= dal.Request<IDAL.DO.Station>(id); //getting station
            dal.Delete<IDAL.DO.Station>(id); //deleting old station
            dal.Create<IDAL.DO.Station>(new IDAL.DO.Station()
            {
                Id = id,
                Name = name == null ? s.Name : name,
                Location = s.Location,
                ChargeSlots = (int)(slots == null ? s.ChargeSlots : (slots - Request<Station>(id).Charging.Count()))
            }); //creating updated station
        }

        /// <summary>
        /// function that updates drone's details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
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

        /// <summary>
        /// function that updates customer's details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="phone"></param>
        public void UpdateCustomer(int id, string name = null, string phone = null)
        {
            IDAL.DO.Customer c = dal.Request<IDAL.DO.Customer>(id); //getting customer
            dal.Delete<IDAL.DO.Customer>(id); //deleting old customer
            dal.Create<IDAL.DO.Customer>(new IDAL.DO.Customer()
            {
                Id = id,
                Location = c.Location,
                Name = name == null ? c.Name : name,
                Phone = phone == null? c.Phone:phone
            }); //creating updated customer
        }



        #endregion Update

        #region InternalMethod

        /// <summary>
        /// function that checks if the drone is assigned to a parcel
        /// </summary>
        public bool isDroneAssigned(DroneToList d)
        {
            var p = RequestList<Parcel>(); //getting list of all parcels
            foreach(var n in p)
            {
                if(n.Drone.Id == d.Id)
                {
                    if (n.Delivered == DateTime.MinValue) // if the parcel isn't delivered yet
                    {
                        d.ParcelId = n.Id; //updating drone's parcel id, because it's 0 (we use this function when configuring the drone list)
                        return true;
                    }
                }
            }
            return false;
        }
        
        /// <summary>
        /// function that returns the location of the closest station to given location
        /// </summary>
        public Location ClosestStation(Location d)
        {
            var stations = RequestList<Station>(); //getting list of all stations
            double distance = Location.distance(stations.First().location, d);
            int id = stations.First().Id;
            foreach (var b in stations)
            {
                if (Location.distance(b.location, d) < distance) //if station is closer than current closest station
                {
                    distance = Location.distance(b.location, d); //update shortest distance and id of closest station
                    id = b.Id;
                }
            }
            Location loc = Request<Customer>(id).location;
            return loc;
        }
        /// <summary>
        /// function the returns the minimum battery needed for a drone to fly a given distance
        /// based on it's status and parcel weight (if it's carrying a parcel)
        /// </summary>
        public double MinBattery(double distance, int id)
        {
            DroneToList d = Drones.Find(x => x.Id == id);
            if (d.Status == DroneStatuses.Available) //if drone is available, return corresponding battery per entered distance
                return info[0] * distance; 
            else //if a parcel is assigned to drone
            {
                Parcel p = Request<Parcel>(d.ParcelId);
                if(p.Delivered == DateTime.MinValue) //if parcel wasn't delivered yet (distance is the distance to pick up parcel)
                    return info[0] * distance;
                else // distance is distance to deliver parcel
                    return info[((int)p.Weight) + 1] * distance; //return battery corresponding to parcel's weight and distance
            }
        }
        /// <summary>
        /// function that returns customer's location based on their id
        /// </summary>
        public Location GetCustomerLocation(int id) //get customers location
        {
            var c = Request<Customer>(id);
            return c.location;
        }

        /// <summary>
        /// function that returns station's location based on it's id
        /// </summary>
        public Location GetStationLocation(int id) //get station's location
        {
            var s = Request<Station>(id);
            return s.location;
        }
        #endregion InternalMethod
    }
}

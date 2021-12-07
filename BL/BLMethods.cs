using System;
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
        /// generic function that creates new object(station, customer, drone, parcel) in dal
        /// </summary>
        public void Create<T>(T t) where T : class
        {
            try
            {
                switch (t)
                {
                    case Station s:

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
                        

                        StationToList station = RequestList<StationToList>().ToList().Find(s => Request<Station>(s.Id).location.Equals(d.Location));
                        //check if the drone location is the same as exist station

                        if (station.Equals(default(Station)))
                            throw new NotExistException("There is no station in this location\n");

                        //check if the station have place for the drone
                        if (station.Available == 0)
                            throw new NotPossibleException("This station have no enough place\n");

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
                            ParcelId = null//not assigned to a parcel
                        });

                        //update station info, reduce available slots by 1
                        UpdateStation(station.Id, null, station.Available + station.Occupied);
                        break;

                    case Parcel p:
                        dal.Create<IDAL.DO.Parcel>(new IDAL.DO.Parcel()
                        {
                            Id = p.Id,
                            SenderId = p.Sender.Id,
                            ReceiverId = p.Receiver.Id,
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
            //catch exception from dal. exception from bl would go through


            //id already exist, need to convert the exception from dal to bl
            catch (IDAL.DO.AlreadyExistException ex)
            {
                throw new AlreadyExistException(ex.Message, ex);
            }
            //id out of bounds, need to convert the exception from dal to bl
            catch (IDAL.DO.OutOfBoundsException ex)
            {
                throw new OutOfBoundsExceptoin(ex.Message, ex);
            }
            //not support struct, need to convert the exception from dal to bl
            //this catch is not supposed to happen but just in case if there is a change in dal
            catch (IDAL.DO.NotSupportException ex)
            {
                throw new NotSupportException(ex.Message, ex);
            }

            //the station have no place for additional drone
            catch(IDAL.DO.NotPossibleException ex)
            {
                throw new NotPossibleException(ex.Message, ex);
            }
            
        }


        #endregion Create

        #region Request
        /// <summary>
        /// the function return object from type T with the same id
        /// </summary>
        /// <typeparam name="T">type of object to return </typeparam>
        /// <param name="id">id of the object</param>
        /// <returns>the requested object (station, parcel, customer and drone)</returns>
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
                            location = new Location() { Latitude = s.Location.Latitude,
                                                        Longitude = s.Location.Longitude},
                            Name = s.Name,
                            //getting a list of all drones charging at s, and making a list of DroneCharge based on the drone list, can be null
                            //maybe we need to change this
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
                            location = new Location() { Longitude = c.Location.Longitude,
                                Latitude = c.Location.Latitude },
                            Name = c.Name,
                            Phone = c.Phone,
                            //query type, not work
                            //To = from ParcelToList entity1 in RequestList<ParcelToList>()
                            //     let entitiy2 = Request<Parcel>(entity1.Id)
                            //     where entitiy2.Receiver.Id == c.Id
                            //     select new ParcelAtCustomer()
                            //     {
                            //         Id = entitiy2.Id,
                            //         Customer = new CustomerParcel() { Id = c.Id, Name = c.Name},
                            //         Priority = entitiy2.Priority,
                            //         Status = EnumExtension.GetStatus(entitiy2.Delivered,entitiy2.PickedUp, entitiy2.Scheduled, entitiy2.Requested),
                            //         Weight = entitiy2.Weight
                            //     },
                            To = RequestList<ParcelToList>().ToList().Select(p=> Request<Parcel>(p.Id)).ToList() //list of all parcels sent to customer, can be empty
                            .FindAll(p => p.Receiver.Id == c.Id)
                            .Select(p => new ParcelAtCustomer()
                            {
                                Id = p.Id,
                                Customer = new CustomerParcel() { Id = c.Id, Name = c.Name },
                                Priority = p.Priority,
                                Status = EnumExtension.GetStatus(p.Delivered, p.PickedUp, p.Scheduled, p.Requested),
                                Weight = p.Weight
                            }).ToList(),

                            From = RequestList<ParcelToList>().ToList().Select(p=>Request<Parcel>(p.Id)).ToList() // list of all parcels sent from customer, can be empty
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
                        
                        //find the drone
                        DroneToList d = Drones.Find(b => b.Id == id);
                        
                        Parcel p  =default(Parcel);

                        //if the drone isn't assigned to a parcel, then we dont want to find this parcel
                        if (d.ParcelId != null)
                            p = Request<Parcel>((int)d.ParcelId);
                        ans = (T)Convert.ChangeType(new Drone()
                        {
                            Id = d.Id,
                            Battery = d.Battery,
                            Location = d.Location,
                            MaxWeight = d.MaxWeight,
                            Model = d.Model,
                            Parcel = d.ParcelId==null?null:d.Status == DroneStatuses.Delivery ? new ParcelDeliver()
                            { //if drone is delivering a parcel, create a ParcelDeliver instance
                                Id = (int)d.ParcelId,
                                Priority = p.Priority,
                                Receiver = p.Receiver,
                                Sender = p.Sender,
                                Status = (EnumParcelDeliver)(p.PickedUp == null ? 0 : 1),
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
                            Receiver = new CustomerParcel() { Id = pi.ReceiverId, Name = Request<Customer>(pi.ReceiverId).Name },
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


        /// <summary>
        /// the function return a list of object
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <returns>return the list (StationToList, CustomerToList, ParcelToList, DroneToList)</returns>
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
                        Delivered = RequestList<Parcel>().ToList().FindAll(p => p.Sender.Id == c.Id && p.Delivered != null).Count(),
                        NoDelivered = RequestList<Parcel>().ToList().FindAll(p => p.Sender.Id == c.Id && p.Delivered == null).Count(),
                        NoReceived = RequestList<Parcel>().ToList().FindAll(p => p.Receiver.Id == c.Id && p.Delivered != null).Count(),
                        Received = RequestList<Parcel>().ToList().FindAll(p => p.Receiver.Id == c.Id && p.Delivered == null).Count()
                    });

                case nameof(DroneToList):
                    return (IEnumerable<T>)Drones;
                case nameof(ParcelToList):
                    return (IEnumerable<T>)dal.RequestList<IDAL.DO.Parcel>().Select(p => new ParcelToList
                    {
                        Id = p.Id,
                        Priority = (Priorities)Enum.Parse(typeof(Priorities), p.Priority.ToString()),
                        ReceiverName = Request<Customer>(p.ReceiverId).Name,
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
            //find the drone
            DroneToList d = Drones.Find(x => x.Id == id);

            //if drone isn't available
            if (d.Status != DroneStatuses.Available)
                throw new DroneIsntAvailableException("drone isn't available\n");

            //need to change to requestList from bl
            List<Parcel> AllParcels = RequestList<Parcel>().ToList(); //getting list of all parcels
            AllParcels.RemoveAll(x => (int)x.Weight > (int)d.MaxWeight); //removing parcels that the drone can't take
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
                //there's no parcel the drone can take
                if (!found)
                    throw new NoParcelException("Not found parcel for the drone with id: " + id + '\n');
            }
        }

        /// <summary>
        /// function that makes the drone deliver the parcel to the receiver
        /// </summary>
        /// <param name="id">id of the drone</param>
        public void Deliver(int id)
        {
            Drone d = Request<Drone>(id);
           
            //find the drone in the drone list
            DroneToList a = Drones.Find(x => x.Id == id);

            //drone didn't assigned to a parcel, so the Parcel is null
            if (d.Parcel == null)
                throw new DroneIsntAssignedException("Drone isn't assigned to a parcel\n");

            //drone didn't pick up the parcel yet
            if (d.Parcel.Status == EnumParcelDeliver.PickUp)
                throw new ParcelWasntPickedUpException("Drone didn't pick up the parcel\n");
            
            //change data in list drones in bl, and dal
            dal.DeliverParcel(d.Parcel.Id);//called function in dal
            a.ParcelId = null;//update drone's parcel to null, because the drone isn't assigned to any parcel right now.
            a.Battery -= MinBattery(Location.distance(Request<Customer>(d.Parcel.Sender.Id).location,d.Location), d.Id);//update drone's battery
            a.Location = Request<Customer>(d.Parcel.Receiver.Id).location;//update drone's location to receiver location
            a.Status = DroneStatuses.Available;//change drone status to available
        }

        /// <summary>
        /// function that makes the drone pick up the parcel from the sender
        /// </summary>
        /// <param name="id">id of the drone</param>
        public void PickUp(int id)
        {
            Drone d = Request<Drone>(id);

            //DroneToList d = Drones.Find(x => x.Id == id);

            //if drone isn't assigned to a parcel
            if (d.Parcel == null)
                throw new DroneIsntAssignedException("Drone isn't assigned to a parcle\n");

            //drone already picked up the parcel
            if (d.Parcel.Status == EnumParcelDeliver.Delivery)
                throw new AlreadyPickedUpException("Parcel was picked up before\n");

            dal.PickUpParcel(d.Parcel.Id);//update data in dals
            DroneToList a = Drones.Find(x => x.Id == id);//get drone from bl

            Location sender  = Request<Customer>(d.Parcel.Sender.Id).location;//ger sender location
            a.Battery -= MinBattery(Location.distance(d.Location,sender), d.Id);//update drone's battery
            a.Location = sender;//update drone's location


        }

        /// <summary>
        /// the function release drone from charging
        /// </summary>
        /// <param name="id">id of the drone</param>
        /// <param name="t">time of charging (in hours)</param>
        public void ReleaseDrone(int id, double t)
        {
            //if the drone isn't exist, then request function would send an exception
            Drone a = Request<Drone>(id);

            DroneToList d = Drones.Find(x => x.Id == id);

            //if drone isn't charging
            if (d.Status != DroneStatuses.Maintenance)
                throw new DroneIsntChargeException("Drone with id: " + id + " isn't cahrging\n");


            d.Status = DroneStatuses.Available;//update drone status
            d.Battery += info[4] * t;//updating drone's battery
            dal.ReleaseDrone(d.Id);//update data in dal

        }

        /// <summary>
        /// the function send a drone to charge
        /// the function find the closest station to the drone with available slots, and send the drone their.
        /// </summary>
        /// <param name="id">id of the drone</param>
        public void SendDroneToCharge(int id)
        {
            DroneToList d = Drones.Find(x => x.Id == id);
            //the drone isn't available
            if (d.Status != DroneStatuses.Available)
                throw new DroneIsntAvailableException("Can't send the drone to charge\n");
            var stations = RequestList<Station>().ToList();
            //var stations = dal.RequestList<IDAL.DO.Station>().ToList(); //getting list of all stations
            stations.RemoveAll(x => x.AvailableSlots == 0); //removing stations with no available charge slots
            stations.OrderByDescending(x => Location.distance(d.Location, x.location)); //sorting station list by distance from drone to station
            bool found = false;
            while (!found && stations.Count != 0) //while there are stations left in list
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
            //the function didn't find a station with available slots
            if (!found)
                throw new NotFoundException("Not found a station with available slots\n");


        }

        /// <summary>
        /// function that updates station's details
        /// </summary>
        public void UpdateStation(int id, string name = null, int? slots = null)
        {
            Station s= Request<Station>(id); //getting station
            dal.Delete<IDAL.DO.Station>(id); //deleting old station
            dal.Create<IDAL.DO.Station>(new IDAL.DO.Station()
            {
                Id = id,
                Name = name == null ? s.Name : name,
                Location = new IDAL.DO.Location()
                {
                    Latitude = s.location.Latitude,
                    Longitude = s.location.Longitude
                },
                ChargeSlots = (int)(slots == null ? s.AvailableSlots : (slots - Request<Station>(id).Charging.Count()))
            }); //creating updated station
        }

        /// <summary>
        /// function that updates drone's details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        public void UpdateDrone(int id, string model = null)
        {
            Drone d = Request<Drone>(id); //getting drone
            dal.Delete<IDAL.DO.Drone>(id); //deleting old drone
            dal.Create<IDAL.DO.Drone>(new IDAL.DO.Drone()
            {
                Id = id,
                MaxWeight = (IDAL.DO.WeightCategories)d.MaxWeight,
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
            Customer c = Request<Customer>(id); //getting customer
            dal.Delete<IDAL.DO.Customer>(id); //deleting old customer
            dal.Create<IDAL.DO.Customer>(new IDAL.DO.Customer()
            {
                Id = id,
                Location = new IDAL.DO.Location()
                {
                    Latitude = c.location.Latitude,
                    Longitude = c.location.Longitude
                },              
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
                    if (n.Delivered == null) // if the parcel isn't delivered yet
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
                Parcel p = Request<Parcel>((int)d.ParcelId);
               
                if(p.Delivered == null) //if parcel wasn't delivered yet (distance is the distance to pick up parcel)
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

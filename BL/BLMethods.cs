using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

                        //if station already exists in the exact location
                        if (dal.RequestList<IDAL.DO.Station>(x=> x.Location.Latitude == s.location.Latitude && x.Location.Longitude == s.location.Longitude).Any())
                            throw new NotPossibleException("A Station already exists in the entered location\n");

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

                        //get the station with the exact location like the drone
                        Station station = RequestList<StationToList>().Select(s => Request<Station>(s.Id)).FirstOrDefault(s => s.location.Equals(d.Location));
                        
                        //check if the drone location is the same as exist station
                        //drone must be created with status of maintenance in an exist station
                        if (station.Equals(default(Station)))
                            throw new NotExistException("There is no station in this location\n");

                        //check if the station have place for the drone
                        if (station.AvailableSlots == 0)
                            throw new NotPossibleException("The station doesn't have enough place\n");

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
                            Battery = (double)r.Next(2000, 4000)/100, //double between 20 to 40 with 2 digits after point
                            Location = d.Location,
                            MaxWeight = d.MaxWeight,
                            Model = d.Model,
                            Status = DroneStatuses.Maintenance,
                            ParcelId = null//not assigned to any parcel
                        });

                        //update station info, reduce available slots by 1
                        dal.ChargeDrone(d.Id, station.Id);
                        break;

                    case Parcel p:
                        dal.Create<IDAL.DO.Parcel>(new IDAL.DO.Parcel()
                        {
                            Id = p.Id,
                            SenderId = p.Sender.Id,
                            ReceiverId = p.Receiver.Id,
                            Weight = (IDAL.DO.WeightCategories)p.Weight,//p.Weight == WeightCategories.Heavy? IDAL.DO.WeightCategories.Heavy,
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
            catch (IDAL.DO.NotPossibleException ex)
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
            try
            {
                switch (typeof(T).Name)
                {
                    case nameof(Station):
                        //get a IDAL.DO.station with this id, convert it to a BL.BO.station
                        IDAL.DO.Station s = dal.Request<IDAL.DO.Station>(id);
                        
                        ans = (T)Convert.ChangeType(new Station()
                        {
                            AvailableSlots = s.ChargeSlots,
                            Id = s.Id,
                            location = new Location()
                            {
                                Latitude = s.Location.Latitude,
                                Longitude = s.Location.Longitude
                            },
                            Name = s.Name,
                            Charging = from DroneToList dr in RequestList<DroneToList>(d => d.Status == DroneStatuses.Maintenance && (new Location() { Longitude = s.Location.Longitude, Latitude = s.Location.Latitude }).Equals(d.Location))
                                       select new DroneCharge()
                                       {
                                           Id = dr.Id,
                                           Battery = dr.Battery
                                       }
                        }, typeof(T));
                        break;


                    case nameof(Customer):
                        //get a customer from dal and convert it to cutomer of bl
                        IDAL.DO.Customer c = dal.Request<IDAL.DO.Customer>(id);
                        ans = (T)Convert.ChangeType(new Customer()
                        {
                            Id = c.Id,
                            location = new Location()
                            {
                                Longitude = c.Location.Longitude,
                                Latitude = c.Location.Latitude
                            },
                            Name = c.Name,
                            Phone = c.Phone,
                            To = from ptl in RequestList<ParcelToList>()
                                 let pd = dal.Request<IDAL.DO.Parcel>(ptl.Id)
                                 where pd.ReceiverId == c.Id
                                 select new ParcelAtCustomer()
                                 {
                                     Id = ptl.Id,
                                     Customer = new CustomerParcel()
                                     {
                                         Id = pd.SenderId,
                                         Name = ptl.SenderName
                                     },
                                     Priority = ptl.Priority,
                                     Status = ptl.Status,
                                     Weight = ptl.Weight
                                 },

                            From = from ptl in RequestList<ParcelToList>()
                                   let pd = dal.Request<IDAL.DO.Parcel>(ptl.Id)
                                   where pd.SenderId == c.Id
                                   select new ParcelAtCustomer()
                                   {
                                       Id = ptl.Id,
                                       Customer = new CustomerParcel()
                                       {
                                           Id = pd.ReceiverId,
                                           Name = ptl.ReceiverName
                                       },
                                       Priority = ptl.Priority,
                                       Status = ptl.Status,
                                       Weight = ptl.Weight
                                   }
                        }, typeof(T));
                        break;
                    case nameof(Drone):

                        //find the drone
                        DroneToList d = Drones.Find(b => b.Id == id);

                        if (d == null)
                            throw new NotExistException("drone doesn't exist\n");

                        Parcel p = default(Parcel);
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
                            Parcel = d.ParcelId == null ? null : d.Status == DroneStatuses.Delivery ? new ParcelDeliver()
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
                            //if the parcel wasn't assigned yet to a drone
                            Drone = a == null?null:new DroneParcel() { Id = a.Id, Battery = a.Battery, Location = a.Location },
                            Priority = (Priorities)pi.Priority,
                            Weight = (WeightCategories)pi.Weight,
                            Receiver = new CustomerParcel() { Id = pi.ReceiverId, Name = dal.Request<IDAL.DO.Customer>(pi.ReceiverId).Name },
                            Sender = new CustomerParcel() { Id = pi.SenderId, Name = dal.Request<IDAL.DO.Customer>(pi.SenderId).Name },
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
            catch (IDAL.DO.NotSupportException ex)
            {
                throw new NotSupportException(ex.Message, ex);
            }
            catch (IDAL.DO.NotExistException ex)//bug: exception unhandled
            {
                throw new NotExistException(ex.Message, ex);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return ans;
        }


        /// <summary>
        /// the function return a list of object
        /// </summary>
        /// <typeparam name="T">type of object can be: StationToList, CustomerToList, DroneToList, ParcelToList</typeparam>
        /// <returns>return the list</returns>
        public IEnumerable<T> RequestList<T>(Expression<Func<T, bool>> ex = null) where T : class
        {
            switch (typeof(T).Name)
            {
                case nameof(StationToList):

                    return (IEnumerable<T>)dal.RequestList<IDAL.DO.Station>(ex ==null?null:Expression.Lambda<Func<IDAL.DO.Station, bool>>(Expression.Convert(ex.Body, typeof(bool)), ex.Parameters)).Select(s => new StationToList()
                    {
                        Id = s.Id,
                        Name = s.Name,
                        Available = s.ChargeSlots,
                        Occupied = Drones.FindAll(d => d.Status == DroneStatuses.Maintenance && d.Location.Equals(GetStationLocation(s.Id))).Count()
                    });

                case nameof(CustomerToList):
                    IEnumerable<Parcel> parcels = RequestList<ParcelToList>().Select(p => Request<Parcel>(p.Id));
                   
                    return (IEnumerable<T>)dal.RequestList<IDAL.DO.Customer>(ex == null ? null : Expression.Lambda<Func<IDAL.DO.Customer, bool>>(Expression.Convert(ex.Body, typeof(bool)), ex.Parameters)).Select(c => new CustomerToList()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Phone = c.Phone,
                        Delivered = (from Parcel p in parcels
                                     where p.Sender.Id == c.Id && p.Delivered != null
                                     select p).Count(),
                        NoDelivered = (from Parcel p in parcels
                                       where p.Sender.Id == c.Id && p.Delivered == null
                                       select p).Count(),
                        NoReceived = (from Parcel p in parcels
                                      where p.Receiver.Id == c.Id && p.Delivered != null
                                      select p).Count(),
                        Received = (from Parcel p in parcels
                                    where p.Receiver.Id == c.Id && p.Delivered == null
                                    select p).Count()
                    });

                case nameof(DroneToList):
                    if (ex == null)
                        return (IEnumerable<T>)Drones;
                    return (IEnumerable<T>)Drones.FindAll(Expression.Lambda<Func<DroneToList, bool>>(Expression.Convert(ex.Body, typeof(bool)), ex.Parameters).Compile().Invoke);
                case nameof(ParcelToList):
                    return (IEnumerable<T>)dal.RequestList<IDAL.DO.Parcel>(ex == null ? null : Expression.Lambda<Func<IDAL.DO.Parcel, bool>>(Expression.Convert(ex.Body, typeof(bool)), ex.Parameters)).Select(p => new ParcelToList
                    {
                        Id = p.Id,
                        Priority = (Priorities)p.Priority,
                        ReceiverName = dal.Request<IDAL.DO.Customer>(p.ReceiverId).Name,
                        SenderName = dal.Request<IDAL.DO.Customer>(p.SenderId).Name,
                        Status = EnumExtension.GetStatus(p.Delivered, p.PickedUp, p.Scheduled, p.Requested),
                        Weight = (WeightCategories)p.Weight,

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

            if (d==null)
                throw new NotExistException("drone doesn't exist\n");
            //if drone isn't available
            if (d.Status != DroneStatuses.Available)
                throw new DroneIsntAvailableException("drone isn't available\n");

            //need to change to requestList from bl
            List<Parcel> AllParcels = RequestList<ParcelToList>().Select(p => Request<Parcel>(p.Id)).ToList(); //getting list of all parcels
            AllParcels.RemoveAll(x => x.Scheduled != null); //removing parcels that don't need to be assigned
            AllParcels.RemoveAll(x => (int)x.Weight > (int)d.MaxWeight); //removing parcels too heavy for drone

            var s = from p in AllParcels
                         orderby p.Priority, p.Weight, Location.distance(d.Location, GetCustomerLocation(p.Sender.Id)) descending
                         select p;

            List<Parcel> sorted = s.ToList();
            bool found = false;
            while (!found && sorted.Count() != 0) //while there are potential parcels left
            {
                var best = sorted.Last(); //geting parcel with shortest distance from drone to parcel's sender from list
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
                    sorted.Remove(best); //if the drone doesn't have enough battery to deliver best parcel we can find, remove it from list 
            }
            if (!found)
                throw new NotFoundException("Not found parcel for the drone with id: " + id + '\n');
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
            a.Battery -= MinBattery(Location.distance(d.Location, d.Parcel.Destination), d.Id);//update drone's battery
            //a.Battery = Math.Round(a.Battery, 3, MidpointRounding.ToPositiveInfinity); //rounding the battery so it won't look ugly
            a.ParcelId = null;//update drone's parcel to null, because the drone isn't assigned to any parcel right now.
            a.Location = d.Parcel.Destination;//update drone's location to receiver location
            a.Status = DroneStatuses.Available;//change drone status to available
        }

        /// <summary>
        /// function that makes the drone pick up the parcel from the sender
        /// </summary>
        /// <param name="id">id of the drone</param>
        public void PickUp(int id)
        {
            Drone d = Request<Drone>(id);

            //if drone isn't assigned to a parcel
            if (d.Parcel == null)
                throw new DroneIsntAssignedException("Drone isn't assigned to a parcle\n");

            //drone already picked up the parcel
            if (d.Parcel.Status == EnumParcelDeliver.Delivery)
                throw new AlreadyPickedUpException("Parcel was picked up before\n");

            dal.PickUpParcel(d.Parcel.Id);//update data in dals
            DroneToList a = Drones.Find(x => x.Id == id);//get drone from bl

            Location sender = Request<Customer>(d.Parcel.Sender.Id).location;//ger sender location
            a.Battery -= MinBattery(Location.distance(d.Location, sender), d.Id);//update drone's battery
            //a.Battery = Math.Round(a.Battery, 3, MidpointRounding.ToPositiveInfinity);//rounding the battery so it won't look ugly
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
            if (t < 0)
                throw new Exception("Number of Hours can't be negative\n");

            d.Status = DroneStatuses.Available;//update drone status
            d.Battery += info[4] * t;//updating drone's battery
           // d.Battery = Math.Round(d.Battery, 3, MidpointRounding.ToPositiveInfinity);//rounding the battery so it won't look ugly
            if (d.Battery > 100) //if battery became bigger than 100, lower it to 100
                d.Battery = 100; 
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

            if (d==null)
                throw new NotExistException("drone doesn't exist\n");

            if (d.Status != DroneStatuses.Available) //the drone isn't available
                throw new DroneIsntAvailableException("Can't send the drone to charge\n");
            List<Station> stations = RequestList<StationToList>().Select(s => Request<Station>(s.Id)).ToList();

            //var stations = dal.RequestList<IDAL.DO.Station>().ToList(); //getting list of all stations
            stations.RemoveAll(x => x.AvailableSlots == 0); //removing stations with no available charge slots
            stations.OrderByDescending(x => Location.distance(d.Location, x.location)); //sorting station list by distance from drone to station
            bool found = false;
            while (!found && stations.Count != 0) //while there are stations left in list and no station was found
            {
                double distance = Location.distance(d.Location, stations.Last().location);
                if (MinBattery(distance, d.Id) <= d.Battery) //if drone has enough battery to get to station
                {
                    found = true;
                    d.Battery -= MinBattery(distance, d.Id);
                    //d.Battery = Math.Round(d.Battery, 3, MidpointRounding.ToPositiveInfinity);//rounding the battery so it won't look ugly
                    d.Location = stations.Last().location;
                    d.Status = DroneStatuses.Maintenance;
                    dal.ChargeDrone(d.Id, stations.Last().Id); //is this it?
                }
                else
                    stations.RemoveAt(stations.Count - 1); //if the drone doesn't have enough battery to get to station, remove station from list 
            }
            if (!found) //the function didn't find a station with available slots
                throw new NotFoundException("Not found a station with available slots\n");
        }

        /// <summary>
        /// function that updates station's details
        /// </summary>
        public void UpdateStation(int id, string name = null, int? slots = null)
        {
            Station s = Request<Station>(id); //getting station
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
        /// <param name="id"> drone id </param>
        /// <param name="model"> drone model </param>
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
            DroneToList a = Drones.Find(x => x.Id == id);
            a.Model = model == null ? a.Model : model;
        }

        /// <summary>
        /// function that updates customer's details
        /// </summary>
        /// <param name="id"> customer id </param>
        /// <param name="name"> customer's name </param>
        /// <param name="phone"> customer's phone number </param>
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
                Phone = phone == null ? c.Phone : phone
            }); //creating updated customer
        }
        #endregion Update

        #region InternalMethod



        /// <summary>
        /// the function search for the closest station
        /// </summary>
        /// <param name="d">current location</param>
        /// <returns>reutrn the location of the closest location</returns>
        public Location ClosestStation(Location d)
        {
            List<Station> stations = RequestList<StationToList>().Select(s => Request<Station>(s.Id)).ToList(); //getting list of all stations
            double distance = Location.distance(stations.First().location, d);
            Location ans = stations.First().location;
            foreach (var b in stations)
            {
                if (Location.distance(b.location, d) < distance) //if station is closer than current closest station
                {
                    distance = Location.distance(b.location, d); //update shortest distance and id of closest station
                    ans = b.location;
                }
            }
            //return the location of the closest station
            return ans;
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
                if (p.Delivered == null) //if parcel wasn't delivered yet (distance is the distance to pick up parcel)
                    return info[0] * distance;
                else // distance is distance to deliver parcel
                {
                    double a = info[((int)p.Weight) + 1] * distance;
                    return info[((int)p.Weight) + 1] * distance; //return battery corresponding to parcel's weight and distance
                }
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

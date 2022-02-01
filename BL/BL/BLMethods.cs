using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using BO;
using BlApi;
namespace BL
{
    /// <summary>
    /// Partial BL class that contains all methods necessary for BL
    /// </summary>
    internal sealed partial class BL : IBL
    {

        #region Create
        /// <summary>
        /// generic function that creates new object(station, customer, drone, parcel) in dal
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Create<T>(T t) where T : class
        {
            try
            {
                lock (dal)
                {
                    switch (t)
                    {
                        case Station s:

                            //if station already exists in the exact location
                            if (dal.RequestList<DO.Station>(x => x.Location.Latitude == s.location.Latitude && x.Location.Longitude == s.location.Longitude).Any())
                                throw new NotPossibleException("A Station already exists in the entered location\n");

                            //create station in dal
                            dal.Create<DO.Station>(new DO.Station()
                            {
                                Id = s.Id,
                                Name = s.Name,
                                ChargeSlots = s.AvailableSlots,
                                Location = new DO.Location()
                                {
                                    Latitude = s.location.Latitude,
                                    Longitude = s.location.Longitude
                                }
                            });
                            break;

                        case Customer c:

                            //create customer in dal
                            dal.Create<DO.Customer>(new DO.Customer()
                            {
                                Id = c.Id,
                                Name = c.Name,
                                Phone = c.Phone,
                                Location = new DO.Location()
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
                            dal.Create<DO.Drone>(new DO.Drone()
                            {
                                Id = d.Id,
                                Model = d.Model,
                                MaxWeight = (DO.WeightCategories)d.MaxWeight,
                            });

                            //if the id already exist, dal.create would throw an exception
                            //create the drone in bl
                            Drones.Add(new DroneToList()
                            {
                                Id = d.Id,
                                Battery = (double)r.Next(2000, 4000) / 100, //double between 20 to 40 with 2 digits after point
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
                            dal.Create<DO.Parcel>(new DO.Parcel()
                            {
                                Id = p.Id,
                                SenderId = p.Sender.Id,
                                ReceiverId = p.Receiver.Id,
                                Weight = (DO.WeightCategories)p.Weight,//p.Weight == WeightCategories.Heavy? DO.WeightCategories.Heavy,
                                Priority = (DO.Priorities)p.Priority,
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
            }
            //catch exceptions from dal. exception from bl would go through

            //id already exist, need to convert the exception from dal to bl
            catch (DO.AlreadyExistException ex)
            {
                throw new AlreadyExistException(ex.Message, ex);
            }
            //id out of bounds, need to convert the exception from dal to bl
            catch (DO.OutOfBoundsException ex)
            {
                throw new OutOfBoundsExceptoin(ex.Message, ex);
            }
            //not support struct, need to convert the exception from dal to bl
            //this catch is not supposed to happen but just in case if there is a change in dal
            catch (DO.NotSupportException ex)
            {
                throw new NotSupportException(ex.Message, ex);
            }

            //the station have no place for additional drone
            catch (DO.NotPossibleException ex)
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
        [MethodImpl(MethodImplOptions.Synchronized)]
        public T Request<T>(int id) where T : class
        {
            T ans = default(T);
            try
            {
                lock (dal)
                {
                    switch (typeof(T).Name)
                    {
                        case nameof(Station):
                            //get a DO.station with this id, convert it to a BL.BO.station
                            DO.Station s = dal.Request<DO.Station>(id);

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
                            DO.Customer c = dal.Request<DO.Customer>(id);
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
                                     let pd = dal.Request<DO.Parcel>(ptl.Id)
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
                                       let pd = dal.Request<DO.Parcel>(ptl.Id)
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
                                    Distance = p.PickedUp ==null? Location.distance(d.Location, GetCustomerLocation(p.Sender.Id)) : Location.distance(GetCustomerLocation(p.Receiver.Id), GetCustomerLocation(p.Sender.Id))
                                } : null,
                                Status = d.Status
                            }, typeof(T));
                            break;
                        case nameof(Parcel):
                            DO.Parcel pi = dal.Request<DO.Parcel>(id);


                            DroneToList a = Drones.Find(d => d.Id == pi.DroneId);
                            ans = (T)Convert.ChangeType(new Parcel()
                            {
                                Id = pi.Id,
                                //if the parcel wasn't assigned yet to a drone
                                Drone = a == null ? null : new DroneParcel() { Id = a.Id, Battery = a.Battery, Location = a.Location },
                                Priority = (Priorities)pi.Priority,
                                Weight = (WeightCategories)pi.Weight,
                                Receiver = new CustomerParcel() { Id = pi.ReceiverId, Name = dal.Request<DO.Customer>(pi.ReceiverId).Name },
                                Sender = new CustomerParcel() { Id = pi.SenderId, Name = dal.Request<DO.Customer>(pi.SenderId).Name },
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
            }
            catch (DO.NotSupportException ex)
            {
                throw new NotSupportException(ex.Message, ex);
            }
            catch (DO.NotExistException ex)
            {
                throw new NotExistException(ex.Message, ex);
            }
            catch (Exception ex)
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
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<T> RequestList<T>(Expression<Func<T, bool>> ex = null) where T : class
        {
            lock (dal)
            {
                switch (typeof(T).Name)
                {
                    case nameof(StationToList):
                        return (IEnumerable<T>)dal.RequestList<DO.Station>().Select(s => new StationToList()
                        {
                            Id = s.Id,
                            Name = s.Name,
                            Available = s.ChargeSlots,
                            Occupied = Drones.FindAll(d => d.Status == DroneStatuses.Maintenance && d.Location.Equals(GetStationLocation(s.Id))).Count()
                        }).Where(ex == null ? x => true : Expression.Lambda<Func<StationToList, bool>>(Expression.Convert(ex.Body, typeof(bool)), ex.Parameters).Compile().Invoke).AsEnumerable();

                    case nameof(CustomerToList):
                        IEnumerable<Parcel> parcels = RequestList<ParcelToList>().Select(p => Request<Parcel>(p.Id)).AsEnumerable();

                        return (IEnumerable<T>)dal.RequestList<DO.Customer>().Select(c => new CustomerToList()
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
                        }).Where(ex == null ? x => true : Expression.Lambda<Func<CustomerToList, bool>>(Expression.Convert(ex.Body, typeof(bool)), ex.Parameters).Compile().Invoke).AsEnumerable();

                    case nameof(DroneToList):
                        if (ex == null)
                            return (IEnumerable<T>)Drones;
                        return (IEnumerable<T>)Drones.FindAll(ex == null ? x => true : Expression.Lambda<Func<DroneToList, bool>>(Expression.Convert(ex.Body, typeof(bool)), ex.Parameters).Compile().Invoke).AsEnumerable();
                    case nameof(ParcelToList):
                        return (IEnumerable<T>)dal.RequestList<DO.Parcel>().Select(p => new ParcelToList
                        {
                            Id = p.Id,
                            Priority = (Priorities)p.Priority,
                            ReceiverName = dal.Request<DO.Customer>(p.ReceiverId).Name,
                            SenderName = dal.Request<DO.Customer>(p.SenderId).Name,
                            Status = EnumExtension.GetStatus(p.Delivered, p.PickedUp, p.Scheduled, p.Requested),
                            Weight = (WeightCategories)p.Weight,

                        }).Where(ex == null ? x => true : Expression.Lambda<Func<ParcelToList, bool>>(Expression.Convert(ex.Body, typeof(bool)), ex.Parameters).Compile().Invoke).AsEnumerable();
                    default:
                        throw new NotSupportException("Not support " + typeof(T).Name + '\n');
                }
            }
        }
        #endregion Request

        #region Update

        /// <summary>
        /// A function that assigns a drone to deliver a parcel, and finds the best parcel for the drone
        /// according to the requirements
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AssignDrone(int id)
        {
            //find the drone
            DroneToList d = Drones.Find(x => x.Id == id);

            if (d == null)
                throw new NotExistException("drone doesn't exist\n");
            //if drone isn't available
            if (d.Status != DroneStatuses.Available)
                throw new DroneIsntAvailableException("drone isn't available\n");

            IEnumerable<Parcel> allParcels = RequestList<ParcelToList>(x => x.Status == ParcelStatuses.Created && (int)x.Weight <= (int)d.MaxWeight).Select(p => Request<Parcel>(p.Id)); //getting list of all parcels

            allParcels = from p in allParcels
                         orderby p.Priority descending, p.Weight descending, Location.distance(d.Location, GetCustomerLocation(p.Sender.Id))
                         select p;
            foreach (Parcel best in allParcels)
            {

                Location sender = GetCustomerLocation(best.Sender.Id);
                Location receiver = GetCustomerLocation(best.Receiver.Id);
                double distance =
                      Location.distance(d.Location, sender)
                    + Location.distance(sender, receiver)
                    + Location.distance(receiver, ClosestStation(receiver));
                double min = info[((int)best.Weight) + 1] * distance; //getting minimum battery required
                if (min <= d.Battery) //checking if drone has enough battery
                {
                    lock (dal)
                    {
                        d.Status = DroneStatuses.Delivery;
                        Parcel selected = Request<Parcel>(best.Id);
                        d.ParcelId = selected.Id;
                        dal.AssignParcel(selected.Id, d.Id);
                        return;
                    }
                }
            }
            throw new NotFoundException("Not found parcel for the drone with id: " + id + '\n');
        }

        /// <summary>
        /// function that makes the drone deliver the parcel to the receiver
        /// </summary>
        /// <param name="id">id of the drone</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
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
            lock (dal)
            {
                a.Battery -= MinBattery(Location.distance(d.Location, d.Parcel.Destination), d.Id);//update drone's battery
                dal.DeliverParcel(d.Parcel.Id);//called function in dal
                a.ParcelId = null;//update drone's parcel to null, because the drone isn't assigned to any parcel right now.
                a.Location = d.Parcel.Destination;//update drone's location to receiver location
                a.Status = DroneStatuses.Available;//change drone status to available
            }
        }

        /// <summary>
        /// function that makes the drone pick up the parcel from the sender
        /// </summary>
        /// <param name="id">id of the drone</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PickUp(int id)
        {
            Drone d = Request<Drone>(id);

            //if drone isn't assigned to a parcel
            if (d.Parcel == null)
                throw new DroneIsntAssignedException("Drone isn't assigned to a parcel\n");

            //drone already picked up the parcel
            if (d.Parcel.Status == EnumParcelDeliver.Delivery)
                throw new AlreadyPickedUpException("Parcel was picked up before\n");

            lock (dal)
            {
                DroneToList a = Drones.Find(x => x.Id == id);//get drone from bl
                Location sender = Request<Customer>(d.Parcel.Sender.Id).location;//ger sender location
                a.Battery -= MinBattery(Location.distance(d.Location, sender), d.Id);//update drone's battery
                dal.PickUpParcel(d.Parcel.Id);//update data in dals
                a.Location = sender;//update drone's location
            }
        }

        /// <summary>
        /// the function release drone from charging
        /// </summary>
        /// <param name="id">id of the drone</param>
        /// <param name="t">time of charging (in hours)</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ReleaseDrone(int id)
        {
            //if the drone isn't exist, then request function would send an exception
            Drone a = Request<Drone>(id);

            DroneToList d = Drones.Find(x => x.Id == id);

            //if drone isn't charging
            if (d.Status != DroneStatuses.Maintenance)
                throw new DroneIsntChargeException("Drone with id: " + id + " isn't cahrging\n");
            lock (dal)
            {
                d.Status = DroneStatuses.Available;//update drone status
                var c = dal.Request<DO.DroneCharge>(d.Id);
                double minutes = (DateTime.Now - c.Start).TotalMinutes;
                d.Battery += info[4] * minutes;//updating drone's battery
                                         // d.Battery = Math.Round(d.Battery, 3, MidpointRounding.ToPositiveInfinity);//rounding the battery so it won't look ugly
                if (d.Battery > 100) //if battery became bigger than 100, lower it to 100
                    d.Battery = 100;
                dal.ReleaseDrone(d.Id);//update data in dal
            }
        }

        /// <summary>
        /// the function send a drone to charge
        /// the function find the closest station to the drone with available slots, and send the drone their.
        /// </summary>
        /// <param name="id">id of the drone</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SendDroneToCharge(int id)
        {
            DroneToList d = Drones.Find(x => x.Id == id);

            if (d == null)
                throw new NotExistException("drone doesn't exist\n");

            if (d.Status != DroneStatuses.Available) //the drone isn't available
                throw new DroneIsntAvailableException("Can't send the drone to charge\n");
            var s = ClosestAvailableStation(d.Location);
            if (s == default(Station)) //the function didn't find a station with available slots
                throw new NotFoundException("Not found a station with available slots\n");
            double distance = Location.distance(d.Location, s.location);
            if (MinBattery(distance, d.Id) <= d.Battery) //if drone has enough battery to get to station
            {
                lock (dal)
                {
                    d.Battery -= MinBattery(distance, d.Id);
                    //d.Battery = Math.Round(d.Battery, 3, MidpointRounding.ToPositiveInfinity);//rounding the battery so it won't look ugly
                    d.Location = s.location;
                    d.Status = DroneStatuses.Maintenance;
                    dal.ChargeDrone(d.Id, s.Id); //is this it?
                }
            }
        }

        /// <summary>
        /// function that updates station's details
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateStation(int id, string name = null, int? slots = null)
        {
            lock (dal)
            {
                Station s = Request<Station>(id); //getting station
                dal.Delete<DO.Station>(id); //deleting old station
                dal.Create<DO.Station>(new DO.Station()
                {
                    Id = id,
                    Name = name == null ? s.Name : name,
                    Location = new DO.Location()
                    {
                        Latitude = s.location.Latitude,
                        Longitude = s.location.Longitude
                    },
                    ChargeSlots = (int)(slots == null ? s.AvailableSlots : (slots - Request<Station>(id).Charging.Count()))
                }); //creating updated station
            }
        }

        /// <summary>
        /// function that updates drone's details
        /// </summary>
        /// <param name="id"> drone id </param>
        /// <param name="model"> drone model </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDrone(int id, string model = null)
        {
            lock (dal)
            {
                Drone d = Request<Drone>(id); //getting drone
                dal.Delete<DO.Drone>(id); //deleting old drone
                dal.Create<DO.Drone>(new DO.Drone()
                {
                    Id = id,
                    MaxWeight = (DO.WeightCategories)d.MaxWeight,
                    Model = model == null ? d.Model : model
                }); //creating updated drone
                DroneToList a = Drones.Find(x => x.Id == id);
                a.Model = model == null ? a.Model : model;
            }
        }

        /// <summary>
        /// function that updates customer's details
        /// </summary>
        /// <param name="id"> customer id </param>
        /// <param name="name"> customer's name </param>
        /// <param name="phone"> customer's phone number </param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateCustomer(int id, string name = null, string phone = null)
        {
            lock (dal)
            {
                Customer c = Request<Customer>(id); //getting customer
                dal.Delete<DO.Customer>(id); //deleting old customer
                dal.Create<DO.Customer>(new DO.Customer()
                {
                    Id = id,
                    Location = new DO.Location()
                    {
                        Latitude = c.location.Latitude,
                        Longitude = c.location.Longitude
                    },
                    Name = name == null ? c.Name : name,
                    Phone = phone == null ? c.Phone : phone
                }); //creating updated customer
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteParcel(int id)
        {
            lock (dal)
                dal.Delete<DO.Parcel>(id);
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CustomerDeliver(int id)
        {
            var p = Request<Parcel>(id);
            if(p.Drone == null)
                throw new DroneIsntAssignedException("A Drone isn't assigned to the parcel\n");
            Deliver(p.Drone.Id);
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CustomerPickUp(int id)
        {
            var p = Request<Parcel>(id);
            if (p.Drone == null)
                throw new DroneIsntAssignedException("A Drone isn't assigned to the parcel\n");
            PickUp(p.Drone.Id);
        }
        #endregion Update

        #region InternalMethod 
        //make these private?

        /// <summary>
        /// the function search for the closest station
        /// </summary>
        /// <param name="d">current location</param>
        /// <returns>reutrn the location of the closest location</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Location ClosestStation(Location d)
        {
            List<Station> stations = RequestList<StationToList>().Select(s => Request<Station>(s.Id)).ToList(); //getting list of all stations
            stations.OrderBy(x => Location.distance(d, x.location)); //ordering stations by distance
            return stations.First().location;
        }
        /// <summary>
        /// function the returns the minimum battery needed for a drone to fly a given distance
        /// based on it's status and parcel weight (if it's carrying a parcel)
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double MinBattery(double distance, int id)
        {
            DroneToList d = Drones.Find(x => x.Id == id);
            if (d.Status == DroneStatuses.Available) //if drone is available, return corresponding battery per entered distance
                return info[0] * distance;
            else //if a parcel is assigned to drone
            {
                Parcel p = Request<Parcel>((int)d.ParcelId);
                if (p.PickedUp == null) //if parcel wasn't picked up yet (distance is the distance to pick up parcel)
                    return info[0] * distance;
                else // distance is distance to deliver parcel
                    return info[((int)p.Weight) + 1] * distance; //return battery corresponding to parcel's weight and distance
            }
        }
        /// <summary>
        /// function that returns customer's location based on their id
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Location GetCustomerLocation(int id) //get customers location
        {
            var c = Request<Customer>(id);
            return c.location;
        }

        /// <summary>
        /// function that returns station's location based on it's id
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Location GetStationLocation(int id) //get station's location
        {
            var s = Request<Station>(id);
            return s.location;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Station ClosestAvailableStation(Location d)
        {
            List<Station> stations = RequestList<StationToList>().Select(s => Request<Station>(s.Id)).ToList(); //getting list of all stations
            stations.RemoveAll(x => x.AvailableSlots == 0); //removing stations that aren't available
            stations.OrderBy(x => Location.distance(d, x.location)); //ordering stations by distance
            return stations.FirstOrDefault();
        }
        #endregion InternalMethod

        public void Simulator(int id, Action update, Func<bool> checkStop)
        {
            new Simulator(this, id, update, checkStop);
        }
    }
}

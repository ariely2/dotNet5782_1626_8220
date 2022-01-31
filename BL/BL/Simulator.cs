using System;
//using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using BO;
using System.Threading;
using static BL.BL;


namespace BL
{
    class Simulator
    {
        const int wait = 1000;
        const double speed = 1;
        public Simulator(BL bl, int id, Action update, Func<bool> checkStop)
        {
            DroneToList d = bl.Drones.Find(x => x.Id == id);
            double distance = 0;
            double battery = d.Battery;
            while (!checkStop())
            {
                Thread.Sleep(wait);
                switch (d.Status)
                {
                    case DroneStatuses.Available:
                        try //first, try to assign a parcel to the drone
                        {
                            bl.AssignDrone(id);
                        }
                        catch(NotFoundException) //if there's no parcel that the drone can deliver, send drone to charge at closest available station if it's battery isn't full
                        {
                            if (d.Battery != 100) //if battery is 100, the drone took all the parcels it can take because they can't be too far from him in the area we configured.
                            {
                                var s = bl.ClosestAvailableStation(d.Location);
                                if (s == default(Station)) //if there's no available station the drone can go to, send it to the closest station
                                {
                                    var c = bl.ClosestStation(d.Location);
                                    if (distance == 0)
                                        distance = Location.distance(d.Location, c);
                                    if (bl.MinBattery(distance, d.Id) <= d.Battery)
                                    {
                                        if (distance <= speed)
                                        {
                                            d.Battery -= bl.MinBattery(distance, d.Id);
                                            d.Location = c;
                                            distance = 0;
                                        }
                                        else
                                        {
                                            distance -= speed; //updating the distance from the destination
                                            d.Battery -= bl.MinBattery(speed, d.Id);
                                        }
                                    } //if the drone can't go to any station, and can't deliver any parcel, it'll stay where it is
                                }
                                else
                                {
                                    if (distance == 0)
                                        distance = Location.distance(d.Location, s.location);
                                    if (distance <= speed) //if we can get to the station now
                                    {
                                        d.Battery = battery; //getting the battery the drone had when it started flying to the station
                                        bl.SendDroneToCharge(id);
                                        battery = d.Battery;
                                        distance = 0;
                                    }
                                    else
                                    {
                                        distance -= speed;
                                        d.Battery -= bl.MinBattery(speed, d.Id);
                                    }
                                }
                            }
                        }
                        update();
                        break;
                    case DroneStatuses.Delivery:
                        Parcel p = bl.Request<Parcel>((int)d.ParcelId);
                        ParcelDeliver a = bl.Request<Drone>(d.Id).Parcel;
                        if(distance == 0)
                            distance = a.Distance;
                        if (distance > speed) //if we can't get to the destination now
                        {
                            distance -= speed;
                            d.Battery -= bl.MinBattery(speed, d.Id);
                        }
                        else
                        {
                            d.Battery = battery; //getting the battery the drone had when it started the delivery/pickup
                            if (p.PickedUp == null)
                                bl.PickUp(id);
                            else
                                bl.Deliver(id);
                            battery = d.Battery;
                            distance = 0;
                        }
                        update();
                        break;
                    case DroneStatuses.Maintenance:
                        if (d.Battery == 100)
                        {
                            d.Battery = battery; //getting the battery the drone had when it started charging
                            bl.ReleaseDrone(d.Id);
                            battery = 100;
                        }
                        else
                        {
                            var c = bl.dal.Request<DO.DroneCharge>(d.Id);
                            d.Battery = battery + bl.info[4] * (DateTime.Now - c.Start).TotalMinutes;
                            if (d.Battery > 100) //if battery became bigger than 100, lower it to 100
                                d.Battery = 100;
                        }
                        update();
                        break;
                }
            }
        }
    }
}

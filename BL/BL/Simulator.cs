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
        const double speed = 0.2; //use speed!
        public Simulator(BL bl, int id, Action update, Func<bool> checkStop)
        {
            DroneToList d = bl.Drones.Find(x => x.Id == id);
            while (checkStop.Invoke() == false)
            {
                Thread.Sleep(wait);
                switch (d.Status)
                {
                    case DroneStatuses.Available:
                        try
                        {
                            bl.AssignDrone(id);
                        }
                        catch(NotFoundException)
                        {
                            try
                            {
                                bl.SendDroneToCharge(id);
                            }
                            catch(NotFoundException)
                            {
                                //send to closest station location, to wait till can charge, if not enough batter - stay
                            }
                        }
                        break;
                    case DroneStatuses.Delivery:
                        Parcel p = bl.Request<Parcel>((int)d.ParcelId);
                        if (p.PickedUp == null)
                            bl.PickUp(id);
                        else
                            bl.Deliver(id);
                        break;
                    case DroneStatuses.Maintenance:
                        if (d.Battery == 100)
                        {
                            d.Status = DroneStatuses.Available;//update drone status
                            bl.dal.ReleaseDrone(d.Id);//update data in dal //is it ok to use dal here? do i need to call release drone in bl?
                        }
                        else
                        {
                            var c = bl.dal.Request<DO.DroneCharge>(d.Id);
                            d.Battery += bl.info[4] * (DateTime.Now - c.Start).TotalHours;
                            if (d.Battery > 100) //if battery became bigger than 100, lower it to 100
                                d.Battery = 100;
                        }
                        break;
                }
            }
        }
    }
}

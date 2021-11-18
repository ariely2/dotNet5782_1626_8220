using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
using IDAL.DalObject;

namespace IBL.BO
{
    public partial class BL:IBL
    {
        private IDal dal;
        public List<DroneToList> drones = new List<DroneToList>(); //public?
        //static??
        public double AvailableUse; 
        public double LightUse; 
        public double MediumUse; 
        public double HeavyUse;
        public double ChargeRate;
        public static Random r = new Random();
        public BL() 
        { 
            dal = new DalObject();
            double[] info = dal.GetBatteryUsageInfo();
            AvailableUse = info[0];
            LightUse = info[1];
            MediumUse = info[2];
            HeavyUse = info[3];
            ChargeRate = info[4];
            
            foreach(var d in dal.RequestList<IDAL.DO.Drone>())
            {
                drones.Add(new DroneToList()
                {
                    Id = d.Id,
                    Model = d.Model,
                    MaxWeight = (BO.WeightCategories)d.MaxWeight
                });
                //if(....)
                //{
                //    drones.Last().Status = DroneStatuses.Delivery;
                //    if(p.Status = ParcelStatuses.Assigned)
                //        drones.Last().Location = ClosestStation();
                //    else
                //        drones.Last().Location = p.Sender.location;
                //    //battery
                //}
                //else
                //{
                //    drones.Last().Status = (DroneStatuses)r.Next(0, 2);
                //    if(drones.Last().Status == DroneStatuses.Maintenance)
                //    {
                //        //location
                //        drones.Last().Battery = r.Next(0, 21);
                //    }
                //    else // if the drone's status is "Available"
                //    {
                            //location
                            //battery
                //    }
                }
            }
        }
    }
}

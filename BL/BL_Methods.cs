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
        public void AssignDrone(Drone d)
        {
            
        }

        public void Create<T>(T t) where T : struct
        {

            try
            {
                dal.Create<T>(t);
            }
            catch
            {

            }
        }

        public void Deliver(Drone d)
        {
            throw new NotImplementedException();
        }

        public void PickUp(Drone d)
        {
            throw new NotImplementedException();
        }

        public void ReleaseDrone(Drone d, double t)
        {
            throw new NotImplementedException();
        }

        public T Request<T>(int id) where T : struct
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> RequestList<T>() where T : struct
        {
            throw new NotImplementedException();
        }

        public void SendDroneToCharge(Drone d)
        {
            throw new NotImplementedException();
        }

    }
}

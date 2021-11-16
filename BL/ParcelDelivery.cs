using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class ParcelDelivery
    {
        public int Id { set; get; }
        public Priorities Priority { set; get; }
        public CustomerParcel Sender { set; get; }
        public CustomerParcel Receiver { set; get; }
        public override string ToString()
        {
            return $"Id:       {Id}\n" +
                   $"Priority: {Priority}\n" +
                   $"Sender:   {Sender}" +
                   $"Receiver: {Receiver}";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class ParcelDeliver
    {
        public int Id { set; get; }
        public bool Status { set; get; }//false - wait for pickup, true - in delivery
        public Priorities Priority { set; get; }
        public WeightCategories Weight { set; get; }
       
        public CustomerParcel Sender { set; get; }
        public CustomerParcel Receiver { set; get; }
        
        public GeoCoordinate Source { set; get; }
        public GeoCoordinate Destination { set; get; }
        public double Distance { set; get; }
        public override string ToString()
        {
            return $"Id:          {Id}\n" +
                   $"Status:       " + (Status ? "PickUp" : "Delivery") + '\n' +
                   $"Priority:    {Priority}\n" +
                   $"Weight:      {Weight}\n" +
                   $"Sender:      {Sender}" +
                   $"Receiver:    {Receiver}" +
                   $"Source:      {Source}" +
                   $"Destination: {Destination}" +
                   $"Distance:    {Distance}\n";

        }
    }
}

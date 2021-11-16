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
        public WeightCategories Weight { set; get; }
        public Priorities Priority { set; get; }
        
        public bool State { set; get; }//false - wait for pickup, true - in delivery
        public GeoCoordinate Source { set; get; }
        public GeoCoordinate Destination { set; get; }
        public double Distance { set; get; }
        public override string ToString()
        {
            return $"Id:          {Id}\n" +
                   $"Weight:      {Weight}\n" +
                   $"Priority:    {Priority}\n" +
                   $"State:       " + (State ? "PickUp" : "Delivery") + '\n' +
                   $"Source:      {Source}" +
                   $"Destination: {Destination}" +
                   $"Distance:    {Distance}\n";

        }
    }
}

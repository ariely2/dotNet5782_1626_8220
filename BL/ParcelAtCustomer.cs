using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class ParcelAtCustomer
    {
        public int Id { set; get; }
        public WeightCategories Weight { set; get; }
        public Priorities Priority { set; get; }
        public ParcelStatuses Status { set; get; }
        public CustomerParcel Customer { set; get; }

        public override string ToString()
        {
            return $"Id: {Id}\n" +
                   $"Weight: {Weight}\n" +
                   $"Priority: {Priority}\n" +
                   $"Parcel Status: {Status}\n" +
                   $"Customer: {Customer}";
        }
    }
}

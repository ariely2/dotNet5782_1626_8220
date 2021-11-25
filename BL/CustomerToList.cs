using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class CustomerToList
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Phone { set; get; }
        public int Delivered { set; get; }//Number of parcels sent and delivered to the target
        public int NoDelivered { set; get; }//Number of parcels the customer sent but not yet delivered to the target
        public int Received { set; get; }//Number of parcels the customer received
        public int NoReceived { set; get; }//nubmer of parcels on the way to the customer

        public override string ToString()
        {
            return Print.print<CustomerToList>(this);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class ParcelToList
    {
        public int Id { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public WeightCategories Weight { set; get; }
        public Priorities Priority { set; get; }
        public ParcelStatuses Status { set; get; }


        public override string ToString()
        {
            return Print.print<ParcelToList>(this);
        }
    }
}

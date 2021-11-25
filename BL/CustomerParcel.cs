using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class CustomerParcel
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public override string ToString()
        {
            return Print.print<CustomerParcel>(this);
        }
    }
}

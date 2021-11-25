using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class StationToList
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public int Available { set; get; }
        public int Occupied { set; get; }

        public override string ToString()
        {
            return Print.print<StationToList>(this);
        }
    }
}

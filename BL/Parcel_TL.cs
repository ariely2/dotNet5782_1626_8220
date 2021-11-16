using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class Parcel_TL
    {
        public int Id { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }

        public override string ToString()
        {
            return $"Id:           {Id}\n" +
                   $"SenderName:   {SenderName}\n" +
                   $"ReceiverName: {ReceiverName}\n";
        }
    }
}

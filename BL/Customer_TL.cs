using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class Customer_TL
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Phone { set; get; }
        public int Delivered { set; get; }
        public int NoDelivered { set; get; }
        public int Received { set; get; }
        public int NoReceived { set; get; }

        public override string ToString()
        {
            return $"Id:           {Id}\n" +
                   $"Name:         {Name}\n" +
                   $"Phoen:        {Phone}\n" +
                   $"Delivered:    {Delivered}\n" +
                   $"No Delivered: {NoDelivered}\n" +
                   $"Received:     {Received}\n" +
                   $"No Received:  {NoReceived}\n";
        }

    }
}

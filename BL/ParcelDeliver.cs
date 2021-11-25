﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class ParcelDeliver
    {
        public int Id { set; get; }
        public EnumParcelDeliver Status { set; get; }//pickup = 0, delivery = 1
        public Priorities Priority { set; get; }
        public WeightCategories Weight { set; get; }
       
        public CustomerParcel Sender { set; get; }
        public CustomerParcel Receiver { set; get; }
        
        public Location Source { set; get; }
        public Location Destination { set; get; }
        public double Distance { set; get; }
        public override string ToString()
        {
            return Print.print<ParcelDeliver>(this);

        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    namespace DO
    {
        public static class EnumExtension
        {
            static Random R = new Random();
            public static T RandomEnumValue<T>()
            {
                var v = Enum.GetValues(typeof(T));
                return (T)v.GetValue(R.Next(v.Length));
            }
        }
        public enum WeightCategories { Light, Medium, Heavy}
        public enum Priorities { Normal, Fast, Emergency}
        public enum DroneStatuses { Avaliable, Maintaince, Delivery}
    }
}

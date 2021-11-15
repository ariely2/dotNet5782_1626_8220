using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
using IDAL.DalObject;

namespace IBL.BO
{
    public partial class BL:IBL
    {
        IDal DataMethods;
        public static double AvailableUse; 
        public static double LightUse; 
        public static double MediumUse; 
        public static double HeavyUse;
        public static double ChargeRate;
        
        public BL() 
        { 
            DataMethods = new DalObject();
            double[] info = DataMethods.GetBatteryUsageInfo();
            AvailableUse = info[0];
            LightUse = info[1];
            MediumUse = info[2];
            HeavyUse = info[3];
            ChargeRate = info[4];

        }
    }
}

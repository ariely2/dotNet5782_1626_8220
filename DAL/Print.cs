using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    class Print
    {
        public static string print<T>(T t)

        {
            string s = "";
            foreach (PropertyInfo finfo in typeof(T).GetProperties())
            {
                s += (string)(finfo.Name + ": " + finfo.GetValue(t));
            }
            return s;
        }
    }
}

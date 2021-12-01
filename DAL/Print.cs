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
        //we want to print like that:
        //"{proprty name}: "(value1)
        //                  (value2)
        //..
        //so, we need to save a variable that save the amount of spaces before the values.
        //to calculate the its value, before we called the function ToString, we need to add the property name + 2 (": ),

        static int n = 0;
        public static string print<T>(T t)

        {
            string ans = "";

            //create string of space
            //
            string space = "";
            for (int i = 0; i < n; i++)
                space += ' ';
            foreach (PropertyInfo finfo in typeof(T).GetProperties())
            {

                n += finfo.Name.Count() + 2;
                if (finfo.GetValue(t) == null)
                    ans += (string)(space + finfo.Name + ": N/A\n");
                else
                    ans += (string)(space + finfo.Name + ": " + finfo.GetValue(t).ToString() + '\n');
                n -= (finfo.Name.Count() + 2);
            }
            return ans;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    class Print
    {

        //this variable represent the amount of spaces in start of every line.
        static int n = 0;

        /// <summary>
        /// the function print the properties in the object using refactoring
        /// we add n spaces in the start of every line in case of struct inside another struct,because We want to print the values ​​hierarchically
        /// </summary>
        /// <typeparam name="T">type of t</typeparam>
        /// <param name="t">object of type T</param>
        /// <returns>
        /// the funciton return a string in this format:
        /// property name1: (value1)
        /// property name2: (value2)
        /// .......
        /// </returns>
        public static string print<T>(T t)

        {
            string ans = "";

            //create string of n spaces

            string space = "";
            for (int i = 0; i < n; i++)
                space += ' ';
            foreach (PropertyInfo finfo in typeof(T).GetProperties())
            {

                n += finfo.Name.Count() + 2;
                //if the value is null, then we can't use ToString(), so instead we add N/A
                if (finfo.GetValue(t) == null)
                    ans += (string)(finfo.Name + ": N/A\n");
                else if (finfo.GetValue(t) is List<DroneCharge>)
                {
                    ans += (string)(finfo.Name + ": " + '\n');
                    foreach (var item in finfo.GetValue(t) as List<DroneCharge>)
                        ans += '\t' + print<DroneCharge>(item).Replace("\n", "\n\t") + "\n";
                }
                else if (finfo.GetValue(t) is List<ParcelAtCustomer>)
                {
                    ans += (string)(finfo.Name + ": " + '\n');
                    foreach (var item in finfo.GetValue(t) as List<ParcelAtCustomer>)
                        ans += '\t' + print<ParcelAtCustomer>(item).Replace("\n", "\n\t") + "\n";
                }
                else
                    ans += (string)(finfo.Name + ": " + finfo.GetValue(t).ToString().Replace("\n", "\n\t") + "\n");
                n -= (finfo.Name.Count() + 2);
            }
            return ans;
        }
    }
}

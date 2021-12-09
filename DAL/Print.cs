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

        //this variable represent the amount of spaces in start of every line.
        public static int n = 0;

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
                    ans += (string)(space + finfo.Name + ": N/A\n");
                else
                    ans += (string)(space + finfo.Name + ": " + finfo.GetValue(t).ToString() + '\n');
                n -= (finfo.Name.Count() + 2);
            }
            return ans;
        }
    }
}

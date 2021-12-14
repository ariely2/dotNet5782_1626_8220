using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class Print
    {

        //this variable represent the amount of spaces in start of every line.
        public static int n = 0;
        //this variable tell if we need to add the space string or not
        private static bool help = false;
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

            foreach (PropertyInfo finfo in typeof(T).GetProperties())
            {

                if (help)
                    ans += new string(' ', n);


                //add the name of the obj to the string

                ans += (finfo.Name + ": ");
                help = false;

                //increase n
                n += finfo.Name.Count() + 2;

                //if the value is null, then we can't use ToString(), so instead we add N/A
                if (finfo.GetValue(t) == null)
                    ans += ("N/A");

                //if the value is IEnumerable and not a string, then we go through all the values on the list
                else if (finfo.PropertyType != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(finfo.PropertyType))
                {
                    int num = 1;

                    foreach (var p in (System.Collections.IEnumerable)finfo.GetValue(t))
                    {
                        help = true;
                        if (num != 1)
                            ans += new string(' ', n) + $"#{num.ToString()}\n";
                        else
                            ans += $"#{num.ToString()}\n";
                        ans += p.ToString();
                        num++;
                    }
                }

                else
                {
                    ans += (finfo.GetValue(t).ToString());
                }

                //if we already add \n to the end of the string, then we don't want to do it again.
                if (!ans.EndsWith('\n'))
                    ans += '\n';
                n -= (finfo.Name.Count() + 2);
                help = true;
            }

            return ans;
        }
    }
}

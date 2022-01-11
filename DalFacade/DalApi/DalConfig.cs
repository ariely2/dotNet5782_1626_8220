using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DalApi
{
    public class DalConfig
    {
        internal static string DalName;
        internal static Dictionary<string, string> DalPackages;

        /// <summary>
        /// Static constructor extracts Dal packages list and Dal type from
        /// Dal configuration file dal-config.xml
        /// </summary>
        public DalConfig()
        {
            //load the xml file
            XElement dalconfig = XElement.Load(@"xml\dal-config.xml");

            DalName = dalconfig.Element("dal").Value;
            DalPackages = (from pkg in dalconfig.Element("dal-packages").Elements()
                           select pkg
                           ).ToDictionary(p => "" + p.Name, p => p.Value);
        }
    }
    public class DalConfigException : Exception
    {
        public DalConfigException(string msg) : base(msg) { }
        public DalConfigException(string msg, Exception ex) : base(msg, ex) { }
    }
    
}

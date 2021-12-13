using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		/// <summary>
		/// the class represent a location
		/// </summary>
		public struct Location
		{
			//longitude coordinate
			public double Longitude { set; get; }

			//latitude coordinate
			public double Latitude { set; get; }

			/// <summary>
			/// the function return a string with longitude and latitude in sexagesimal representation
			/// </summary>
			/// <returns>return the location in sexagesimal representations</returns>
			public override string ToString()
			{
				return $"Longitude: {SexagesimalRepresentation(Longitude, true)}\n" +
					   $"  Latitude:  {SexagesimalRepresentation(Latitude, false)}\n";
			}

			/// <summary>
			/// the function convert longitude or latitude to sexagesimal representation
			/// </summary>
			/// <param name="a"></param>
			/// <param name="type">represent if a is longitude or latitude. true -> a is longitude. false -> a is latitude</param>
			/// <returns>reutrn string of the sexagesimal representation</returns>
			public static string SexagesimalRepresentation(double a, bool type)
			{

				string ans;
				double degrees = a * (a < 0 ? -1 : 1);
				ans = string.Format("{0}{1}", (int)degrees, ((char)176));

				double minutes = (degrees - (int)degrees) * 60;
				ans += ' ' + string.Format("{0}\'", (int)minutes);

				double seconds = (minutes - (int)minutes) * 60;
				ans += ' ' + string.Format("{0:0.000}\"", seconds);

				return ans + (!type ? (a < 0 ? 'S' : 'N') : (a < 0 ? 'W' : 'E'));
			}

			/// <summary>
			/// reutrn the distance between 2 coordinates
			/// was taken from: https://www.geeksforgeeks.org/program-distance-two-points-earth/
			/// </summary>
			/// <param name="g1">geo coordinate 1</param>
			/// <param name="g2">geo coordinate 2</param>
			/// <returns>the distance between the 2 coordinates</returns>
			public static double distance(Location g1, Location g2)
			{
				var baseRad = Math.PI * g1.Latitude / 180;
				var targetRad = Math.PI * g2.Latitude / 180;
				var theta = g1.Longitude - g2.Longitude;
				var thetaRad = Math.PI * theta / 180;

				double dist =
					Math.Sin(baseRad) * Math.Sin(targetRad) + 
					Math.Cos(baseRad) *Math.Cos(targetRad) * Math.Cos(thetaRad);
				dist = Math.Acos(dist);
				dist = dist * 180 / Math.PI;
				dist = dist * 60 * 1.1515;
				return dist * 1.609344;
			}
		}


	}
}

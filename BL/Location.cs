using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
	namespace BO
	{
		public class Location
		{
			public double Longitude { set; get; }
			public double Latitude { set; get; }

			/// <summary>
			/// the function return a string with longitude and latitude in sexagesimal representation
			/// </summary>
			/// <returns>string of longitude and latitude</returns>
			public override string ToString()
			{
				return $"Longitude: {SexagesimalRepresentation(Longitude, true)}\n" + 
					   $"Latitude:  {SexagesimalRepresentation(Latitude, false)}\n";
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

				return ans + (type ? (a < 0 ? 'S' : 'N') : (a < 0 ? 'W' : 'E'));
			}


			/// <summary>
			/// convert degree to randian
			/// </summary>
			/// <param name="angle">the degree</param>
			/// <returns>the radian</returns>
			public static double toRadians(double angle)
			{
				return (angle * Math.PI) / 180;
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

				// The math module contains
				// a function named toRadians
				// which converts from degrees
				// to radians.

				double lon1 = toRadians(g1.Longitude);
				double lon2 = toRadians(g2.Longitude);
				double lat1 = toRadians(g1.Latitude);
				double lat2 = toRadians(g2.Latitude);

				// Haversine formula
				double dlon = lon2 - lon1;
				double dlat = lat2 - lat1;
				double a = Math.Pow(Math.Sin(dlat / 2), 2) +
						   Math.Cos(lat1) * Math.Cos(lat2) *
						   Math.Pow(Math.Sin(dlon / 2), 2);

				double c = 2 * Math.Asin(Math.Sqrt(a));

				// Radius of earth in
				// kilometers.

				double r = 3956;

				// calculate the result
				return (c * r);
			}

		}
	}
}

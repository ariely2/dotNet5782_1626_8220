using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
	namespace DO
	{
		public class GeoCoordinate
		{
			public double Longitude { set; get; }
			public double Latitude { set; get; }

			/// <summary>
			/// the function return a string with longitude and latitude in sexagesimal
			/// </summary>
			/// <returns>string of longitude and latitude</returns>
			public override string ToString()
			{
				return $@"
Longitude: {SexagesimalRepresentation(Longitude)}
Latitude:  {SexagesimalRepresentation(Latitude)}";
			}

			//need to fix this function
			/// <summary>
			/// 
			/// </summary>
			/// <param name="a"></param>
			/// <returns></returns>
			public static string SexagesimalRepresentation(double a)
			{
				string ans;
				double degrees = a;
				ans = a.ToString();
				ans += ((char)176);


				double minutes = (degrees - (int)degrees) * 60;
				ans += ' ' + minutes.ToString() + ' ' + '\'';

				double seconds = (minutes - (int)minutes) * 60;
				ans += ' ' + seconds.ToString() + '\'' + '\'';
				return ans;
			}


			public static double toRadians(double angleIn10thofaDegree)
			{
				// Angle in 10th
				// of a degree
				return (angleIn10thofaDegree * Math.PI) / 180;
			}

			/// <summary>
			/// reutrn the distance between 2 coordinates
			/// was taken from: https://www.geeksforgeeks.org/program-distance-two-points-earth/
			/// </summary>
			/// <param name="lon1">longitude of coordinate 1</param>
			/// <param name="lat1">latitude of coordinate 1</param>
			/// <param name="lon2">longitude of coordinate 2</param>
			/// <param name="lat2">latitude of coordinate 2</param>
			/// <returns>the distance between the 2 coordinates</returns>
			public static double distance(double lon1, double lat1, double lon2, double lat2)
			{

				// The math module contains
				// a function named toRadians
				// which converts from degrees
				// to radians.
				lon1 = toRadians(lon1);
				lon2 = toRadians(lon2);
				lat1 = toRadians(lat1);
				lat2 = toRadians(lat2);

				// Haversine formula
				double dlon = lon2 - lon1;
				double dlat = lat2 - lat1;
				double a = Math.Pow(Math.Sin(dlat / 2), 2) +
						   Math.Cos(lat1) * Math.Cos(lat2) *
						   Math.Pow(Math.Sin(dlon / 2), 2);

				double c = 2 * Math.Asin(Math.Sqrt(a));

				// Radius of earth in
				// kilometers. Use 3956
				// for miles
				double r = 6371;

				// calculate the result
				return (c * r);
			}
		}


	}
}

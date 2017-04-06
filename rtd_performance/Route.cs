using System;
using System.Collections.Generic;
using System.IO;

namespace rtd
{

	public class Route
	{
		public struct route_t
		{
			public string route_id;
		}

		// CSV column Index
		const int ROUTE_ID = 0;

		const string routesFileName = "routes.txt";

		public static Dictionary<string, route_t> routes = new Dictionary<string, route_t>() { };
		static public List<RouteInstance> route_instances;

		public Route()
		{
			initializeRoutes();
			initializeRouteInstances();
		}

		public static void outputResults()
		{
			Console.WriteLine("###################################################################################");
			Console.WriteLine(string.Format("Outputting hourly route bus timeliness for {0} from {1} to {2}",
			                                DateTime.Now.ToString("yyyy/MM/dd"),
			                                DateTime.Now.AddHours(-1).ToString("HH:mm:ss"),
			                                DateTime.Now.ToString("HH:mm:ss")));

			foreach (RouteInstance route_instance in route_instances)
			{
				if (route_instance.getTotalTrips() > 0)
				{
					Console.WriteLine(string.Format("Route: {0}, average time: {1} seconds, buses: {2}, average status: {3}",
					                                route_instance.routeId,
									  				Math.Round(route_instance.getAverageTime(), 2),
													route_instance.getTotalTrips(),
					                                route_instance.getAverageTime() < 0 ? "LATE" : "EARLY"));
				}
			}
			Console.WriteLine("###################################################################################\n");
		}

		public static void reset()
		{
			initializeRouteInstances();
		}

		private static void initializeRouteInstances()
		{
			route_instances = new List<RouteInstance>();

			foreach (string route_id in routes.Keys)
			{
				route_instances.Add(new RouteInstance(route_id));
			}
		}

		private static void initializeRoutes()
		{
			StreamReader file = new StreamReader(routesFileName);
			string line;
			string[] row = new string[12];

			while ((line = file.ReadLine()) != null)
			{
				row = line.Split(',');
				route_t thisRoute = new route_t
				{
					route_id = row[ROUTE_ID]
				};
				routes.Add(thisRoute.route_id, thisRoute); // add this stop to the stops dictionary
			}
			file.Close();
		}

		public static RouteInstance getRouteById(string route_id)
		{
			foreach (RouteInstance route_instance in route_instances)
			{
				if (route_instance.routeId.Equals(route_id))
				{
					return route_instance;
				}
			}

			return null;
		}
	}

	public class RouteInstance
	{
		public string routeId;

		private double totalDeltaTime = 0;
		private long count = 0;
		private List<string> trips;

		public RouteInstance(string id)
		{
			routeId = id;
			trips = new List<string>();
		}

		public void addTime(long deltaTime)
		{
			totalDeltaTime += deltaTime;
			++count;
		}

		public void addTrip(string tripId)
		{
			if (!trips.Contains(tripId))
			{
				trips.Add(tripId);
			}
		}

		/**
		 * Returns the total number of different trips (i.e., different buses) along the route within the period 
		 * tracked.
		 */
		public long getTotalTrips()
		{
			return trips.Count;
		}

		/**
		 * Returns the route's average delta time between scheduled arrival time and current predicted arrival time
		 * within the period tracked. The value is positive if the bus is early on average and negative if the bus is
		 * late on average.
		 */
		public double getAverageTime()
		{
			if (count == 0)
			{
				return 0;
			}

			return (double) totalDeltaTime / count;
		}
	}

}

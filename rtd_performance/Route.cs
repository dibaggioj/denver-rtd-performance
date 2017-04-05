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

			// TODO: output average delta time and output trip count (sorted from latest average to earliest average maybe)
			foreach (RouteInstance route_istance in route_instances)
			{
				Console.WriteLine("Average time: " + route_istance.getAverageTime() + ", buses: " + route_istance.getTotalTrips());
			}
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

		public long getTotalTrips()
		{
			return trips.Count;
		}

		public double getAverageTime()
		{
			if (count == 0)
			{
				//throw new Exception("No times recorded");
				return 0;
			}

			return (double) totalDeltaTime / count;
		}
	}

}

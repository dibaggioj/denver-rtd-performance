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
		static public List<RouteInstance> routes_instances;

		public Route()
		{
			initializeRoutes();
			initializeRouteInstances();
		}

		static void outputResults()
		{

			// TODO: output average delta time and output trip count (sorted from latest average to earliest average maybe)
			foreach (RouteInstance route_istance in routes_instances)
			{
				Console.WriteLine("Average time: " + route_istance.getAverageTime() + ", buses: " + route_istance.getTotalTrips());
			}
		}

		static void reset()
		{
			initializeRouteInstances();
		}

		static void initializeRouteInstances()
		{
			routes_instances = new List<RouteInstance>();

			foreach (string route_id in routes.Keys)
			{
				routes_instances.Add(new RouteInstance(route_id));
			}
		}

		static void initializeRoutes()
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
	}

	public class RouteInstance
	{
		public string routeId;
		public double totalDeltaTime = 0;
		public long count = 0;
		public long totalTrips = 0;
		public List<string> trips;

		// TODO: handle trip count

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

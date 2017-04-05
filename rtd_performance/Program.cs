using System;
using System.IO;
using System.Net;
using ProtoBuf;
using transit_realtime;

/**
 * Denver RTD Bus Allocation Performance Reporter
 *
 * IWKS 5120 Lab 6
 * 
 * Modified from JKB test-transit
 * 
 * April 6, 2017
 *
 * Nanu Ahluwalia
 * John DiBaggio
 * Kyle Etsler
 */

/* Denver RTD Data Formats

Sample of the Trip Updates Feed
-------------------------------
header {
    gtfs_realtime_version: "1.0"
    incrementality: FULL_DATASET
    timestamp: 1449176392
}
entity {
    id: "1449176392_109470943"
    trip_update {
        trip {
            trip_id: "109470943"
            schedule_relationship: SCHEDULED
            route_id: "0"
            direction_id: 0
        }
        stop_time_update {
            stop_sequence: 6
            arrival {
                time: 1449176381
            }
            departure {
               time: 1449176381
            }
            stop_id: "25676"
            schedule_relationship: SCHEDULED
        }
        stop_time_update {
            stop_sequence: 7
            arrival {
                time: 1449176479
            }
            departure {
                time: 1449176479
            }
            stop_id: "22454"
            schedule_relationship: SCHEDULED
        }
        stop_time_update {
            stop_sequence: 8
            arrival {
                time: 1449176585
            }
            departure {
                time: 1449176585
            }
            stop_id: "20378"
            schedule_relationship: SCHEDULED
        }
        vehicle {
            id: "6010"
            label: "6010"
        }
        timestamp: 1449042054
    }
}

Sample of the Vehicle Positions Feed
------------------------------------
header {
    gtfs_realtime_version: "1.0"
    incrementality: FULL_DATASET
    timestamp: 1449042263
}
entity {
    id: "1449042263_1505"
    vehicle {
        trip {
            trip_id: "109486700"
            schedule_relationship: SCHEDULED
            route_id: "AB"
            direction_id: 1
        }
        position {
            latitude: 39.8419
            longitude: -104.676231
            bearing: 161
        }
        current_status: IN_TRANSIT_TO
        timestamp: 1449042245
        stop_id: "22903"
        vehicle {
            id: "1505"
            label: "1505"
        }
    }
}
*/

namespace rtd
{
    class Program
    {

		// file with comma-separated client id and secret on a single line: {client id},{client secret}
		const string rtd_service_filename = "rtd_service.txt";

		static string client_id = "";
		static string client_secret = "";
		static Stop stop_inst;
		static Trip trip_inst;
		static Route route_inst;

		static void InitRTDServiceCredentials()
		{
			StreamReader file = new StreamReader(rtd_service_filename);
			string line = file.ReadLine();
			if (line != null)
			{
				string[] row = line.Split(',');
				client_id = row[0];
				client_secret = row[1];
			}
			file.Close();
		}

		static void InitStaticRTDData()
		{
			stop_inst = new Stop();    // initialize static stop dictionary from RTD data file
			trip_inst = new Trip();    // initialize static trip dictionary from RTD data file
			route_inst = new Route(); // initialize static route dictionary from RTD data file
		}

		static void GetAndProcessTripUpdate()
		{
			Uri myUri = new Uri("http://www.rtd-denver.com/google_sync/TripUpdate.pb");
			WebRequest myWebRequest = HttpWebRequest.Create(myUri);

			HttpWebRequest myHttpWebRequest = (HttpWebRequest)myWebRequest;

			NetworkCredential myNetworkCredential = new NetworkCredential(client_id, client_secret);

			CredentialCache myCredentialCache = new CredentialCache();
			myCredentialCache.Add(myUri, "Basic", myNetworkCredential);

			myHttpWebRequest.PreAuthenticate = true;
			myHttpWebRequest.Credentials = myCredentialCache;

			FeedMessage feed = Serializer.Deserialize<FeedMessage>(myWebRequest.GetResponse().GetResponseStream());

			foreach (FeedEntity entity in feed.entity)
			{
				TripUpdate trip_update = entity.trip_update;
				if (trip_update == null)
				{
					continue;
				}

				string trip_id = trip_update.trip != null ? trip_update.trip.trip_id : null;
				if (trip_id == null 
				    || trip_update.stop_time_update == null 
				    || trip_update.stop_time_update.Count < 1 
				    || !Trip.trips.ContainsKey(trip_id))
				{
					continue;
				}

				TripUpdate.StopTimeUpdate current_next_stop_time_update = trip_update.stop_time_update[0];
				if (current_next_stop_time_update == null)
				{
					continue;
				}

				uint current_first_stop_seq = current_next_stop_time_update.stop_sequence;
				TripUpdate.StopTimeEvent current_first_stop_arrival = current_next_stop_time_update.arrival;
				if (current_first_stop_arrival == null)
				{
					continue;
				}

				long current_next_stop_arrival_time = current_first_stop_arrival.time;
				int stop_seq = Convert.ToInt32(current_first_stop_seq);
				Trip.trip_t static_trip = Trip.trips[trip_id];
				if (stop_seq < 1 || static_trip.tripStops.Count < stop_seq)
				{
					continue;
				}

				Trip.trip_stops_t static_next_trip_stop = static_trip.tripStops[stop_seq - 1];    // sequence numbers are 1-indexed
				if (static_next_trip_stop.arrive_time <= 0)
				{
					continue;
				}

				long delta_time = static_next_trip_stop.arrive_time - current_next_stop_arrival_time;

				Console.WriteLine("predicted arrival time: " + current_next_stop_arrival_time + " vs scheduled arrival time: " + static_next_trip_stop.arrive_time);
				Console.WriteLine("difference: " + delta_time);

				RouteInstance route = Route.getRouteById(trip_update.trip.route_id);
				if (route == null)
				{
					continue;
				}

				route.addTrip(trip_id);
				route.addTime(delta_time);

				Console.WriteLine(route.routeId + ", " + route.getAverageTime() + ", " + route.getTotalTrips());
			}
		}

		//static void GetAndProcessVehiclePosition()
		//{
		//	myUri = new Uri("http://www.rtd-denver.com/google_sync/VehiclePosition.pb");
		//	WebRequest myWebRequest = HttpWebRequest.Create(myUri);

		//	HttpWebRequest myHttpWebRequest = (HttpWebRequest)myWebRequest;

		//	NetworkCredential myNetworkCredential = new NetworkCredential(client_id, client_secret);

		//	CredentialCache myCredentialCache = new CredentialCache();
		//	myCredentialCache.Add(myUri, "Basic", myNetworkCredential);

		//	myHttpWebRequest.PreAuthenticate = true;
		//	myHttpWebRequest.Credentials = myCredentialCache;

		//	FeedMessage feed = Serializer.Deserialize<FeedMessage>(myWebRequest.GetResponse().GetResponseStream());

		//	foreach (FeedEntity entity in feed.entity)
		//	{
		//		if (entity.vehicle != null)
		//		{
		//			if (entity.vehicle.trip != null)
		//			{
		//				if (entity.vehicle.trip.route_id != null)
		//				{
		//					Console.WriteLine("Route ID = " + entity.vehicle.trip.route_id);
		//					Console.WriteLine("Vehicle ID = " + entity.vehicle.vehicle.id);
		//					Console.WriteLine("Current Position Information:");
		//					Console.WriteLine("Current Latitude = " + entity.vehicle.position.latitude);
		//					Console.WriteLine("Current Longitude = " + entity.vehicle.position.longitude);
		//					Console.WriteLine("Current Bearing = " + entity.vehicle.position.bearing);
		//					Console.WriteLine("Current Status = " + entity.vehicle.current_status + " StopID: " + entity.vehicle.stop_id);
		//					if (Stop.stops.ContainsKey(entity.vehicle.stop_id))
		//					{
		//						Console.WriteLine("The name of this StopID is \"" + Stop.stops[entity.vehicle.stop_id].stop_name + "\"");
		//						Console.WriteLine("The Latitude of this StopID is \"" + Stop.stops[entity.vehicle.stop_id].stop_lat + "\"");
		//						Console.WriteLine("The Longitude of this StopID is \"" + Stop.stops[entity.vehicle.stop_id].stop_long + "\"");
		//						string wheelChairOK = "IS NOT";
		//						if (Stop.stops[entity.vehicle.stop_id].wheelchair_access)
		//						{
		//							wheelChairOK = "IS";
		//						}
		//						Console.WriteLine("This stop is " + wheelChairOK + " wheelchair accessible");
		//					}

		//					Console.WriteLine("Trip ID = " + entity.vehicle.trip.trip_id);
		//					if (Trip.trips.ContainsKey(entity.vehicle.trip.trip_id))
		//					{
		//						if (entity.vehicle.current_status.ToString() == "IN_TRANSIT_TO")
		//						{
		//							if (Stop.stops.ContainsKey(entity.vehicle.stop_id))
		//							{
		//								Console.WriteLine("Vehicle in transit to: " + Stop.stops[entity.vehicle.stop_id].stop_name);
		//								Trip.trip_t trip = Trip.trips[entity.vehicle.trip.trip_id];
		//								foreach (Trip.trip_stops_t stop in trip.tripStops)
		//								{
		//									if (stop.stop_id == entity.vehicle.stop_id)
		//									{
		//										Console.WriteLine(".. and is scheduled to arrive there at " + stop.arrive_time);
		//									}
		//								}
		//							}
		//						}
		//						Console.WriteLine();
		//					}
		//				}
		//			}
		//		}
		//	}

		//	Console.WriteLine("Press any key to continue");
		//	Console.ReadLine();
		//}

		static void Main(string[] args)
		{
			InitRTDServiceCredentials();

			InitStaticRTDData();

			GetAndProcessTripUpdate();

			/**
			 * TODO:
			 * 
			 * Add outer 1-hr loop for output and restart - call these at end:
			 *     Route.outputResults();
			 *     Route.reset();
			 * 
			 * Add inner 1-minute loop inside of the outer loop to the update data - should just need to call this 
			 * each time:
			 *     GetAndProcessTripUpdate();
			 * 
			 */ 

			Console.WriteLine("Press any key to continue");
		}   
    }
}





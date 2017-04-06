using System;
using System.Timers;
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
		static Timer updateTimer;

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
			Console.Write("Getting RTD static data...");
			stop_inst = new Stop();    // initialize static stop dictionary from RTD data file
			trip_inst = new Trip();    // initialize static trip dictionary from RTD data file
			route_inst = new Route();  // initialize static route dictionary from RTD data file
			Console.WriteLine("Done!\nInitialzing program.\n");
		}

		static void initTimer()
		{
			updateTimer = new Timer();
			updateTimer.Elapsed += UpdateTimer_Tick;
			updateTimer.Interval = 60 * 60 * 1000;
			updateTimer.Start();
		}

		static void displayGreeting()
		{
			Console.WriteLine("Authors: John DiBaggio, Kyle Etsler, Nanu Ahluwalia\n"
								+ "IoT Lab 6 - Spring 2017\n\n");
		}

		static void displayExit()
		{
			Console.WriteLine("This program will continue to run until you exit. Press enter to close the program.\n"
								+ "This program will update every 1 hour with data...");
			Console.ReadLine();
		}

		/**
-		 * Get minutely updated data from service
-		 * Process data and store for output at the end of the hour
-		 */
		static void updateData()
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

				RouteInstance route = Route.getRouteById(trip_update.trip.route_id);
				if (route == null)
				{
					continue;
				}

				route.addTrip(trip_id);
				route.addTime(delta_time);
			}
		}

		static void GetAndProcessTripUpdate(object sender, EventArgs e)
		{
			updateData();
		}

		/**
-		 * Output data collected over past hour and restart recording
-		 */
		private static void UpdateTimer_Tick(object sender, EventArgs e)
		{
			Route.outputResults();
			Route.reset();
			Timer minTimer;
			minTimer = new Timer();
			minTimer.Elapsed += GetAndProcessTripUpdate;
			minTimer.Interval = 60 * 1000;
			minTimer.Start();
		}


		static void Main(string[] args)
		{
			displayGreeting();
			InitRTDServiceCredentials();
			InitStaticRTDData();
			updateData();
			initTimer();
			displayExit();
		}
	}
}





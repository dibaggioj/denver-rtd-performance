using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace rtd
{
	class TripStopTime
	{
		public long time = 0;			// unix time
		public bool normalized = true;	// true if hh was in 00-23 originally, false if hh was in 24-28 originally

		public TripStopTime(string timeStr)
		{
			timeStr = normalizeTimeTo24Hr(timeStr);
			convertTimeToUnix(timeStr);
		}

		private void convertTimeToUnix(string timeStr)
		{
			try
			{
				if (normalized)
				{
					timeStr = DateTime.Now.ToString("yyyy/MM/dd") + " " + timeStr;
				}
				else
				{
					timeStr = DateTime.Now.AddDays(1).ToString("yyyy/MM/dd") + " " + timeStr;	// adjust for hh 24-28
				}

				DateTime dateTimeFinal = DateTime.ParseExact(timeStr, "yyyy/MM/dd HH:mm:ss", null);
				DateTime dateTimeInitial = new DateTime(1969, 12, 31, 18, 0, 0, dateTimeFinal.Kind);    // TODO: add daylight savings time support
				time = (long) (dateTimeFinal.Subtract(dateTimeInitial)).TotalSeconds;
			}
			catch (FormatException e)
			{
				Console.WriteLine("FormatException thrown while parsing trip stop time '" + timeStr + "' " + e.ToString());
			}
		}

		private string normalizeTimeTo24Hr(string timeStr)
		{
			string originalTime = timeStr;

			timeStr = Regex.Replace(timeStr, @"(24)(:\d{2}:\d{2})", "00$2");
			timeStr = Regex.Replace(timeStr, @"(25)(:\d{2}:\d{2})", "01$2");
			timeStr = Regex.Replace(timeStr, @"(26)(:\d{2}:\d{2})", "02$2");
			timeStr = Regex.Replace(timeStr, @"(27)(:\d{2}:\d{2})", "03$2");
			timeStr = Regex.Replace(timeStr, @"(28)(:\d{2}:\d{2})", "04$2");

			normalized = timeStr.Equals(originalTime);

			return timeStr;
		}
	}

    class Trip
    {
		public struct trip_stop_time
		{
			public long time;
			public bool is24hrScale;
		}

        public struct trip_stops_t
        {
            public string stop_seq;
            public string stop_id;
			public long arrive_time;
			public long depart_time;
        }

        public struct trip_t
        {
            public string trip_id;
            public List<trip_stops_t> tripStops;
        }

        const int TRIP_TRIP_ID = 0;
        const int TRIP_STOP_ARRIVE_TIME = 1;
        const int TRIP_STOP_DEPT_TIME = 2;
        const int TRIP_STOP_STOP_ID = 3;
        const int TRIP_STOP_STOP_SEQ = 4;

        const string stopTimesFileName = "stop_times.txt";

        public static Dictionary<string, trip_t> trips = new Dictionary<string, trip_t>() { };

        public Trip()
        {
            // constructor
            initializeTrips();
        }

        static void initializeTrips()
        {
            StreamReader file = new StreamReader(stopTimesFileName);
            string line;
            string[] row = new string[9];

            trip_t thisTrip;
            List<trip_stops_t> stops;
			bool readTitles = false;

            while ((line = file.ReadLine()) != null)
            {
				if (!readTitles)
				{
					readTitles = true;
				}
				else
				{
					row = line.Split(',');

					// There are 2 cases to consider
					//      1) this is the first time we have seen this trip ID
					//      2) we are build a list of stops for this trip

					// If this is the first time we have seen this trip ID, start a new dictionary entry
					// otherwise keep going with the current trip

					string tripID = row[TRIP_TRIP_ID];

					TripStopTime arrival = new TripStopTime(row[TRIP_STOP_ARRIVE_TIME]);
					TripStopTime departure = new TripStopTime(row[TRIP_STOP_DEPT_TIME]);

					if (!trips.ContainsKey(tripID)) // we have never seen this trip ID; start a new trip
					{
						thisTrip = new trip_t { };
						thisTrip.trip_id = tripID;
						stops = new List<trip_stops_t>();
						stops.Add(new trip_stops_t
						{
							stop_id = row[TRIP_STOP_STOP_ID],
							stop_seq = row[TRIP_STOP_STOP_SEQ],
							arrive_time = arrival.time,
							depart_time = departure.time
						});  
						thisTrip.tripStops = stops;
						trips.Add(tripID, thisTrip); // add this trip to the trips dictionary
					}
					else // we have seen this trip ID; append this stop to the stops list
					{
						trips[tripID].tripStops.Add(new trip_stops_t
						{
							stop_id = row[TRIP_STOP_STOP_ID],
							stop_seq = row[TRIP_STOP_STOP_SEQ],
							arrive_time = arrival.time,
							depart_time = departure.time
						});

					}
				}

            }
            file.Close();
        }
    }
}

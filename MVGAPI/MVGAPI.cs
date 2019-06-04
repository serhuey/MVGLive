using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MVGAPI
{
    public static class MVGAPI
    {
        static string StationType = "station";
        static string api_key = "5af1beca494712ed38d313714d4caff6";
        static string query_url_name = "https://www.mvg.de/fahrinfo/api/location/queryWeb?q=";  //for station names

#pragma warning disable 169, 414
        static string query_url_id = "https://www.mvg.de/fahrinfo/api/location/query?q=";       // #for station ids, not used now 
#pragma warning restore 169, 414

        static string departure_url = "https://www.mvg.de/fahrinfo/api/departure/";
        static string departure_url_postfix = "?footway=0";

        /// <summary>
        /// Get XAML-prepared departures for the given station ID
        /// </summary>
        /// <param name="stationID"></param>
        /// <returns></returns>
        public static ObservableCollection<PreparedDeparture> GetPreparedDepartures(string stationID)
        {
            ObservableCollection<PreparedDeparture> preparedDepartures = null;
            DeserializedDepartures[] departureResponse = GetDeserializedDepartures(stationID);
            if (departureResponse != null)
            {
                preparedDepartures = new ObservableCollection<PreparedDeparture>();
                Array.Sort(departureResponse, delegate (DeserializedDepartures dp1, DeserializedDepartures dp2)
                {
                    return dp1.departureTime.CompareTo(dp2.departureTime);
                });
                foreach (DeserializedDepartures dp in departureResponse)
                {
                    DateTime now = DateTime.Now;
                    DateTime localDateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(dp.departureTime).DateTime.ToLocalTime();
                    TimeSpan difference = localDateTimeOffset.Subtract(now);

                    PreparedDeparture fd = new PreparedDeparture();
                    fd.product = dp.product;
                    fd.label = dp.label;
                    fd.destination = dp.destination;
                    fd.minutesToDeparture = ((int)difference.TotalMinutes).ToString() + " min";

                    fd.departureTime = String.Format("{0:D2}:{1:D2}:{2:D2}", localDateTimeOffset.Hour, localDateTimeOffset.Minute, localDateTimeOffset.Second);
                    fd.sev = dp.sev;
                    preparedDepartures.Add(fd);
                }
            }
            return preparedDepartures;
        }

        /// <summary>
        /// Get deserialized departures for the station with ID
        /// </summary>
        /// <param name="stationID">Unique station ID</param>
        /// <returns></returns>
        public static DeserializedDepartures[] GetDeserializedDepartures(string stationID)
        {
            Departures dD;
            try
            {
                string jsonResponse = GetJsonDepartures(stationID);
                dD = JsonConvert.DeserializeObject<Departures>(jsonResponse);
                if (dD != null && dD.departures.Length > 0)
                {
                    return dD.departures;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Get Locations string for the query string
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        static string GetLocations(string query)
        {
            string jsonString;
            string url;

            if (int.TryParse(query, out int iQuery))
            {
                url = query_url_name + iQuery.ToString();
            }
            else
            {
                url = query_url_name + query;
            }
            jsonString = PerformApiRequest(url);
            return jsonString;
        }

        /// <summary>
        /// Get Location class for the station name in German
        /// </summary>
        /// <param name="stationName">Name of the desired station in German</param>
        /// <returns></returns>
        static Location GetStations(string stationName)
        {
            try
            {
                Locations locs = JsonConvert.DeserializeObject<Locations>(GetLocations(stationName));
                if (locs != null && locs.locations.Length > 0 && locs.locations[0].type == StationType)
                {
                    return locs.locations[0];
                }
            }
            catch
            {
            }
            return null;
        }

        /// <summary>
        /// Get numeric ID for the station name in German
        /// </summary>
        /// <param name="stationName">Name of the desired station in German</param>
        /// <returns>Station ID, if station name exists, -1 otherwise</returns>
        static public int GetIdForStation(string stationName)
        {
            Location locs;

            locs = GetStations(stationName);
            if (locs != null && locs.id != 0)
            {
                return locs.id;
            }
            return -1;
        }

        /// <summary>
        /// Get JSON departure string for the stationID
        /// </summary>
        /// <param name="stationID">Unique numeric station ID</param>
        /// <returns></returns>
        static public string GetJsonDepartures(string stationID)
        {
            string url = departure_url + stationID + departure_url_postfix;
            string result = PerformApiRequest(url);
            return result;
        }

        /// <summary>
        /// Perform API request to the mvg web service
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        static public string PerformApiRequest(string url)
        {
            HttpWebRequest requests = (HttpWebRequest)WebRequest.Create(url);
            requests.ContentType = "application/json; charset=utf-8";
            requests.Headers.Add("X-MVG-Authorization-Key", api_key);
            requests.Method = "GET";

            string result;
            HttpWebResponse response = requests.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                result = reader.ReadToEnd();
            }
            return result;
        }

    }
}

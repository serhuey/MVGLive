using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace MVGAPI
{
    public static class MVGAPI
    {
        static public bool NoConnection { get; set; }
        static readonly string stationType = "station";
        static readonly string rootUrlName = "https://www.mvg.de/api/fahrinfo";
        static readonly string queryUrlName = rootUrlName + "/location/queryWeb?q=";  
        static readonly string departureUrl = rootUrlName + "/departure/";
        static readonly string departureUrlPostfix = "?footway=0";
#pragma warning disable 169, 414
        static readonly string queryUrlId = rootUrlName + "/location/query?q=";       // #for station ids, not used now 
#pragma warning restore 169, 414

        /// <summary>
        /// Get deserialized departures for the station with ID
        /// </summary>
        /// <param name="stationID">Unique station ID</param>
        /// <returns></returns>
        public static DeserializedDepartures[] GetDeserializedDepartures(string stationID)
        {
            if(string.IsNullOrEmpty(stationID))
            {
                return null;
            }

            Departures dD;

            try
            {
                string jsonResponse = GetJsonDepartures(stationID);

                if (string.IsNullOrEmpty(jsonResponse)) return null;

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
            string jsonstring;
            string url;

            if (int.TryParse(query, out int iQuery))
            {
                url = queryUrlName + iQuery.ToString();
            }
            else
            {
                url = queryUrlName + query;
            }
            jsonstring = PerformApiRequest(url);
            return jsonstring;
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
                string locations = GetLocations(stationName);
                if (string.IsNullOrEmpty(locations)) return null;

                Locations locs = JsonConvert.DeserializeObject<Locations>(locations);
                if (locs != null && locs.locations.Length > 0 && locs.locations[0].type == stationType)
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
        /// <returns>Station ID, if station name exists, "" otherwise</returns>
        static public string GetIdForStation(string stationName)
        {
            Location locs;

            locs = GetStations(stationName);
            if (locs != null && !string.IsNullOrEmpty(locs.id))
            {
                return locs.id;
            }
            return "";
        }

        /// <summary>
        /// Get JSON departure string for the stationID
        /// </summary>
        /// <param name="stationID">Unique numeric station ID</param>
        /// <returns></returns>
        static public string GetJsonDepartures(string stationID)
        {
            string url = departureUrl + stationID + departureUrlPostfix;
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
            requests.Method = "GET";

            NoConnection = false;
            string result = null;

            try
            {
                HttpWebResponse response = requests.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    result = reader.ReadToEnd();
                }
            }
            catch(Exception ex) when (ex is WebException || ex is System.Net.Sockets.SocketException || ex is ObjectDisposedException)
            {
                NoConnection = true;
            }

            return result;
        }

        /// <summary>
        /// Format new API deserialized departure to an old one. 
        /// </summary>
        /// <param name="deserializedDepartures">Array with deserialized departures in new format on the beginning and in an old format on the exit</param>
        static public void FormatNewAPItoOld(ref DeserializedDepartures[] deserializedDepartures)
        {
            foreach (DeserializedDepartures dD in deserializedDepartures)
            {
                dD.departureTime = dD.departureTime + dD.delay * 1000 * 60;
            }
        }

        /// <summary>
        /// Create array without duplcates
        /// </summary>
        /// <param name="deserializedDepartures"></param>
        static public void DeleteDuplicates(ref DeserializedDepartures[] deserializedDepartures)
        {
            List<DeserializedDepartures> departuresNewList = new List<DeserializedDepartures>();
            List<int> arrayHashes = new List<int>();

            for (int i = 0; i < deserializedDepartures.Length; i++)
            {
                int hash = deserializedDepartures[i].GetHashCode();
                if (!arrayHashes.Contains(hash))
                {
                    arrayHashes.Add(hash);
                    departuresNewList.Add(deserializedDepartures[i]);
                }
            }

            deserializedDepartures = departuresNewList.ToArray();
        }
    }
}

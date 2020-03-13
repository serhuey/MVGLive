// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace MVGAPI
{
    public static class MVGAPI
    {
        static public bool IsConnected { get; set; } = false;
        private const string StationType = "station";
        private const string RootUrlName = "https://www.mvg.de/api/fahrinfo";
        private const string QueryUrlName = RootUrlName + "/location/queryWeb?q=";
        private const string DepartureUrl = RootUrlName + "/departure/";
        private const string DepartureUrlPostfix = "?footway=0";
        private const string IdMvvStations = "MVVStations.txt";
        private const int DefaultRequestTimeOut = 15000;
        private static ConcurrentDictionary<string, string> localIdСache;
        private readonly static ConcurrentQueue<string> stationIdRequestQueue = new ConcurrentQueue<string>();
        private static readonly BackgroundWorker stationIdRequestBackgroundWorker = new BackgroundWorker();



        /// <summary>
        /// Static constructor
        /// </summary>
        static MVGAPI()
        {
            BuildLocalStationIdCash();
            stationIdRequestBackgroundWorker.DoWork += new DoWorkEventHandler(StationIdRequestBackgroundWorker_DoWork);
            stationIdRequestBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(StationIdRequestBackgroundWorker_RunWorkerCompleted);
        }


        /// <summary>
        /// Get deserialized departures for the station with ID
        /// </summary>
        /// <param name="stationID">Unique station ID</param>
        /// <returns></returns>
        public static DeserializedDepartures[] GetDeserializedDepartures(string stationID)
        {
            if (string.IsNullOrEmpty(stationID))
            {
                return null;
            }

            Departures dD;

            try
            {
                string jsonResponse = GetJsonDepartures(stationID);

                if (string.IsNullOrEmpty(jsonResponse))
                {
                    IsConnected = false;
                    return null;
                }

                dD = JsonConvert.DeserializeObject<Departures>(jsonResponse);
                if (dD != null && dD.departures.Length > 0)
                {
                    IsConnected = true;
                    return dD.departures;
                }
                else
                {
                    IsConnected = false;
                    return null;
                }
            }
            catch (Exception ex)
            {
                IsConnected = false;
                throw new Exception("Exception in GetDeserializedDepartures", ex);
            }
        }

        /// <summary>
        /// Get Locations string for the query string
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private static string GetLocations(string query, int requestTimeOut)
        {
            string jsonstring;
            string url;

            if (int.TryParse(query, out int iQuery))
            {
                url = QueryUrlName + iQuery.ToString();
            }
            else
            {
                url = QueryUrlName + query;
            }
            jsonstring = PerformApiRequest(url, requestTimeOut);
            return jsonstring;
        }

        /// <summary>
        /// Get Location class for the station name in German
        /// </summary>
        /// <param name="stationName">Name of the desired station in German</param>
        /// <returns></returns>
        private static Location GetStations(string stationName, int requestTimeOut = DefaultRequestTimeOut)
        {
            try
            {
                string locations = GetLocations(stationName, requestTimeOut);
                if (string.IsNullOrEmpty(locations)) return null;

                Locations locs = JsonConvert.DeserializeObject<Locations>(locations);
                if (locs != null && locs.locations.Length > 0 && locs.locations[0].type == StationType)
                {
                    return locs.locations[0];
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception in GetStations", ex);
            }
            return null;
        }

        /// <summary>
        /// Get ID for the station name in German
        /// If there is no ID in local cache, put the station name in the request ID queue to avoid program freezing.
        /// </summary>
        /// <param name="stationName">Name of the desired station in German</param>
        /// <returns>Station ID, if station name exists, "" otherwise</returns>
        static public string GetIdForStation(string stationName)
        {
            if (string.IsNullOrEmpty(stationName)) return "";

            if (localIdСache.ContainsKey(stationName))
            {
                return localIdСache[stationName];
            }
            else
            {
                if (!stationIdRequestQueue.Contains(stationName))
                {
                    stationIdRequestQueue.Enqueue(stationName);

                    Console.WriteLine("Elements in Queue:" + stationIdRequestQueue.Count.ToString());
                    if (!stationIdRequestBackgroundWorker.IsBusy)
                    {
                        stationIdRequestBackgroundWorker.RunWorkerAsync();
                    }
                }
                return "";
            }
        }

        /// <summary>
        /// Background worker RunWorkerCompleted event handler
        /// Start new background worker cycle if the ID request queue is not empty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void StationIdRequestBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!stationIdRequestQueue.IsEmpty && !stationIdRequestBackgroundWorker.IsBusy)
            {
                stationIdRequestBackgroundWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Asynchronically get station ID
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void StationIdRequestBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (stationIdRequestQueue.TryDequeue(out string stationName))
            {
                Location locs;

                locs = GetStations(stationName);
                if (!localIdСache.Keys.Contains(stationName))
                {
                    if (locs != null && !string.IsNullOrEmpty(locs.id))
                    {
                        localIdСache.TryAdd(stationName, locs.id);
                    }
                    else
                    {
                        localIdСache.TryAdd(stationName, "");
                    }
                }
            }
        }

        /// <summary>
        /// Get JSON departure string for the stationID
        /// </summary>
        /// <param name="stationID">Unique numeric station ID</param>
        /// <returns></returns>
        static public string GetJsonDepartures(string stationID)
        {
            string url = DepartureUrl + stationID + DepartureUrlPostfix;
            string result = PerformApiRequest(url);

            return result;
        }

        /// <summary>
        /// Perform API request to the mvg web service
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        static public string PerformApiRequest(string url, int requestTimeOut = DefaultRequestTimeOut)
        {
            HttpWebRequest requests = (HttpWebRequest)WebRequest.Create(url);
            requests.ContentType = "application/json; charset=utf-8";
            requests.Method = "GET";
            requests.Timeout = requestTimeOut;

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
            catch (Exception ex) when (ex is WebException || ex is System.Net.Sockets.SocketException || ex is ObjectDisposedException)
            {
                result = null;
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
                dD.departureTime += dD.delay * 1000 * 60;
            }
        }

        /// <summary>
        /// Create array without duplicates
        /// </summary>
        /// <param name="deserializedDepartures"></param>
        static public void DeleteDuplicates(ref DeserializedDepartures[] deserializedDepartures)
        {
            List<DeserializedDepartures> departuresNewList = new List<DeserializedDepartures>();
            List<int> arrayHashes = new List<int>();

            for (int i = 0; i < deserializedDepartures.Length; i++)
            {
                int hash = (deserializedDepartures[i].departureId + deserializedDepartures[i].destination).GetHashCode();
                if (!arrayHashes.Contains(hash))
                {
                    arrayHashes.Add(hash);
                    departuresNewList.Add(deserializedDepartures[i]);
                }
            }

            deserializedDepartures = departuresNewList.ToArray();
        }

        /// <summary>
        /// Sort deserializedDepartures
        /// Primary key - departureTime,
        /// Secondary key - product+label
        /// </summary>
        static public void Sort(ref DeserializedDepartures[] deserializedDepartures)
        {
            Array.Sort(deserializedDepartures, delegate (DeserializedDepartures dp1, DeserializedDepartures dp2)
            {
                if (dp1.departureTime == dp2.departureTime)
                {
                    return (dp1.product + dp1.label).CompareTo(dp2.product + dp2.label);
                }
                else
                {
                    return (dp1.departureTime).CompareTo(dp2.departureTime);
                }
            });
        }

        /// <summary>
        /// Build local dictionary with stations ID from embedded resource file
        /// This file was made from xls file downloaded here: https://www.opendata-oepnv.de/ht/de/organisation/verkehrsverbuende/mvv/startseite
        /// Each line of this file must content two strings (key and value) separated with tab
        /// </summary>
        static private void BuildLocalStationIdCash()
        {
            string fileContent = ReadResource(IdMvvStations);
            string separator = "\t";
            string[] splitContent;
            string line;

            localIdСache = new ConcurrentDictionary<string, string>();

            using (StringReader reader = new StringReader(fileContent))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    splitContent = line.Split(separator.ToCharArray());

                    if (splitContent == null ||
                        splitContent.Length < 2 ||
                        string.IsNullOrEmpty(splitContent[0]) ||
                        string.IsNullOrEmpty(splitContent[1]) ||
                        localIdСache.ContainsKey(splitContent[0]))
                    {
                        continue;
                    }

                    localIdСache.TryAdd(splitContent[0], splitContent[1]);
                }
            }
        }

        /// <summary>
        /// Read embedded resource file as a string
        /// </summary>
        /// <param name="name">Name of the embedded resource file</param>
        /// <returns>String from resource file or empty string if file doesn't exists</returns>
        private static string ReadResource(string name)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] resources = assembly.GetManifestResourceNames();
            string resourcePath = resources.Single(str => str.EndsWith(name));

            if (string.IsNullOrEmpty(resourcePath))
            {
                return "";
            }

            // This is not very obvious. See https://docs.microsoft.com/en-us/visualstudio/code-quality/ca2202?view=vs-2019&redirectedfrom=MSDN
            Stream stream = null;
            string returnedString;
            try
            {
                stream = assembly.GetManifestResourceStream(resourcePath);
                using (StreamReader reader = new StreamReader(stream))
                {
                    stream = null;
                    returnedString = reader.ReadToEnd();
                }
            }
            finally
            {
                stream?.Dispose();
            }
            return returnedString;
        }
    }
}
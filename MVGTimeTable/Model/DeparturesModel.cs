// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MVGAPI;
using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Threading;
using MVGTimeTable;

namespace MVGTimeTable.Model
{
    internal class DeparturesModel : INotifyPropertyChanged, IDisposable
    {
        #region Fields

        private const int noConnectionLongTimeout = 240;
        private const int noConnectionShortTimeout = 50;
        private readonly string stationName;
        private readonly int timerRefreshInterval;
        private readonly int timerRefreshStartInterval;
        private BackgroundWorker backgroundWorker;
        private ConnectionState connectionStatus = ConnectionState.NOT_CONNECTED;
        private DeserializedDepartures[] departureResponse;
        private DeserializedDepartures[] newDepartureResponse;
        private int? savedResponseHashCode = 0;
        private string savedStationID;
        private string savedStationName;
        private string stationID;
        private DateTime timeConnectionLost;
        private DispatcherTimer timerRefresh;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Current status of the internet connection
        /// </summary>
        public ConnectionState ConnectionStatus
        {
            get
            {
                return connectionStatus;
            }
            set
            {
                connectionStatus = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConnectionStatus)));
            }
        }

        /// <summary>
        /// Departures data
        /// </summary>
        public DeserializedDepartures[] DepartureResponse
        {
            get
            {
                return departureResponse;
            }
            set
            {
                departureResponse = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DepartureResponse)));
            }
        }

        public bool GleisColumnPresent { get; set; } = false;
        public bool SGleisColumnPresent { get; set; } = false;

        #endregion Properties

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        /// ************************************************************************************************
        /// <param name="stationName">Station name in German</param>
        /// <param name="timerRefreshInterval">Timer interval in seconds</param>
        /// <param name="timerRefreshStartInterval">Timer start delay. It's usable if main form includes more
        /// than one TimeTable control.</param>
        public DeparturesModel(string stationName = "Hauptbahnhof", int timerRefreshInterval = 15, int timerRefreshStartInterval = 1)
        {
            this.stationName = stationName;
            this.timerRefreshInterval = timerRefreshInterval;
            this.timerRefreshStartInterval = timerRefreshStartInterval;
            ConnectionStatus = ConnectionState.JUST_STARTED;
            DepartureResponse = null;
        }

        /// ************************************************************************************************
        /// <summary>
        /// Starts model's work.
        /// </summary>
        public void Start()
        {
            // Data Refresh timer
            timerRefresh = new DispatcherTimer();
            timerRefresh.Tick += TimerRefresh_Tick;
            timerRefresh.Interval = TimeSpan.FromSeconds(timerRefreshStartInterval); //interval is updated in the TimerRefresh_Tick
            timerRefresh.Start();

            // Background Worker for asynchronic data receiving
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
        }

        /// ************************************************************************************************
        /// <summary>
        /// Starts new asynchronous data receiving
        /// </summary>
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            stationID = GetStationID(stationName);
            if (!string.IsNullOrEmpty(stationID))
            {
                newDepartureResponse = MVGAPI.MVGAPI.GetDeserializedDepartures(stationID);
            }
            else
            {
                newDepartureResponse = null;
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Backgroundworker RunWorkerCompleted event handler
        /// </summary>
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DateTime now = DateTime.Now;
            ConnectionStatus = GetNewConnectionStatus(now);

            if ((newDepartureResponse == null || newDepartureResponse.Length == 0) && (ConnectionStatus == ConnectionState.SHORT_DISCONNECT || ConnectionStatus == ConnectionState.LONG_DISCONNECT))
            {
                newDepartureResponse = departureResponse; // use last departure responce
            }

            if (newDepartureResponse != null)
            {
                int? responseHashCode;

                MVGAPI.MVGAPI.FormatNewAPItoOld(ref newDepartureResponse);
                MVGAPI.MVGAPI.DeleteDuplicates(ref newDepartureResponse);
                MVGAPI.MVGAPI.Sort(ref newDepartureResponse);

                responseHashCode = GetResponseHashCode(newDepartureResponse, now);
                if (responseHashCode != savedResponseHashCode)
                {
                    savedResponseHashCode = responseHashCode;
                    ParseDestination.ProcessForkLines(ref newDepartureResponse);
                    DepartureResponse = newDepartureResponse;
                }
            }
            else
            {
                DepartureResponse = null;
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Gets new connection status for current time
        /// </summary>
        /// <param name="now">Current time</param>
        /// <returns>New connection status</returns>
        private ConnectionState GetNewConnectionStatus(DateTime now)
        {
            ConnectionState newConnectionStatus;
            double noConnectionSeconds;

            if (MVGAPI.MVGAPI.IsConnected)
            {
                newConnectionStatus = ConnectionState.CONNECTED;
            }
            else
            {
                if (ConnectionStatus == ConnectionState.CONNECTED || ConnectionStatus == ConnectionState.JUST_STARTED)
                {
                    timeConnectionLost = now;
                }
                noConnectionSeconds = (now - timeConnectionLost).TotalSeconds;
                if (ConnectionStatus != ConnectionState.JUST_STARTED)
                {
                    newConnectionStatus = noConnectionSeconds <= noConnectionShortTimeout ? ConnectionState.SHORT_DISCONNECT :
                                          (noConnectionSeconds <= noConnectionLongTimeout ? ConnectionState.LONG_DISCONNECT : ConnectionState.NOT_CONNECTED);
                }
                else
                {
                    newConnectionStatus = ConnectionState.JUST_STARTED;
                }
            }
            return newConnectionStatus;
        }

        /// ************************************************************************************************
        /// <summary>
        /// Calculates Hash Code for the DeserializedDeparture array with actual time
        /// </summary>
        /// <param name="deserializedDepartures">DeserializedDeparture array</param>
        /// <param name="now">Actual time</param>
        /// <returns>Hash code or null</returns>
        private int? GetResponseHashCode(DeserializedDepartures[] deserializedDepartures, DateTime now)
        {
            StringBuilder allValuesString = new StringBuilder();

            foreach (DeserializedDepartures deserializedDeparture in deserializedDepartures)
            {
                Common.GetTimes(now, deserializedDeparture.departureTime, deserializedDeparture.delay, out _, out _, out TimeSpan difference);

                allValuesString.Append(difference.Minutes.ToString());
                allValuesString.Append(deserializedDeparture.delay.ToString());
                allValuesString.Append(deserializedDeparture.departureId);
                allValuesString.Append(deserializedDeparture.departureTime.ToString());
                allValuesString.Append(deserializedDeparture.destination);
                allValuesString.Append(deserializedDeparture.label);
                allValuesString.Append(deserializedDeparture.platform);
                allValuesString.Append(deserializedDeparture.product);
                allValuesString.Append(deserializedDeparture.cancelled);
                allValuesString.Append(deserializedDeparture.sev.ToString());
            }
            return allValuesString.ToString().GetHashCode();
        }

        /// ************************************************************************************************
        /// <summary>
        /// Gets string with the unique station ID
        /// </summary>
        /// <param name="stationName">Simple station name in German, e.g. "Böhmerwaldplatz"</param>
        /// <returns>Station ID</returns>
        private string GetStationID(string stationName)
        {
            if (!string.IsNullOrEmpty(stationName))
            {
                GleisColumnPresent = Common.MultiplatformTramStations.Contains(stationName.ToUpperInvariant());
                SGleisColumnPresent = Common.MultiplatformSbahnStations.Contains(stationName.ToUpperInvariant());

                if (stationName != savedStationName)
                {
                    savedStationID = MVGAPI.MVGAPI.GetIdForStation(stationName);
                    if (MVGAPI.MVGAPI.IsConnected)
                    {
                        savedStationName = stationName;
                    }
                    return savedStationID;
                }
                else
                {
                    return savedStationID;
                }
            }
            return null;
        }

        /// ************************************************************************************************
        /// <summary>
        /// Refresh Timer Event Handler, runs background worker
        /// </summary>
        private void TimerRefresh_Tick(object sender, EventArgs e)
        {
            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync();
            }
            timerRefresh.Interval = TimeSpan.FromSeconds(timerRefreshInterval);
        }

        public void Dispose()
        {
            ((IDisposable)backgroundWorker).Dispose();
        }
    }
}
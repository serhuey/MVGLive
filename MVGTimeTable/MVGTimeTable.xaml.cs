// Copyright (c) Sergei Grigorev. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using MVGAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;


namespace MVGTimeTable
{

    public partial class MVGTimeTable : UserControl, IDisposable
    {
        public PreparedDeparture[] Departures;

        public bool NoConnection => !savedConnectionStatus;

        public string PrimaryForegroundColor
        {
            get;
            set;
        }
        public string SecondaryForegroundColor
        {
            get;
            set;
        }
        public string PrimaryBackgroundColor
        {
            get;
            set;
        }
        public string SecondaryBackgroundColor
        {
            get;
            set;
        }

        public ObservableCollection<PreparedDeparture> departureDataSource;

        public delegate void ConnectionStatusChanged(object sender, EventArgs e);
        public event ConnectionStatusChanged ConnectionStatusChangedEvent;

        private DeserializedDepartures[] departureResponse;

        private bool propertiesDefined = false;
        private BackgroundWorker backgroundWorker1;
        private DispatcherTimer timerRefresh;

        private int timerRefreshInterval;
        private int timerRefreshStartInterval;

        private string stationName;
        private string stationID;
        private string savedStationName;
        private string savedStationID;

        private int bindCounter = 0;
        private readonly int secondsToFirstUpdate = 4;

        private int? oldResponseHashCode = 0;

        private readonly Dictionary<string, double> savedWidths = new Dictionary<string, double>
        {
            { Common.ColumnName[Column.Line], 0 },
            { Common.ColumnName[Column.Destination], 0 },
            { Common.ColumnName[Column.TimeToDeparture], 0 },
            { Common.ColumnName[Column.DepartureTime], 0 },
            { Common.ColumnName[Column.Platform], 0 }
        };

        private int fontSize;

        //private int noConnectionTimeoutCounter = 0;
        private DateTime timeConnectionLost;
        private bool savedConnectionStatus;
        private const int noConnectionTimeoutThreshold1 = 50;       // in seconds
        private const int noConnectionTimeoutThreshold2 = 240;      // in seconds

        public MVGTimeTable()
        {
            InitializeComponent();
            SetColumnSizeEventsHandlers();
        }


        /// ************************************************************************************************
        /// <summary>
        /// Set MVGTimeTable Control properties. It must be performed before start the control.
        /// </summary>
        /// <param name="_stationName">Station name in German</param>
        /// <param name="_timerRefreshInterval">Interval between two Http requests</param>
        /// <param name="_timerRefreshStartInterval">Pause before first request</param>
        /// <param name="_fontSize">Font size in pixels</param>
        /// <param name="_fontFamily">Font family</param>
        /// ************************************************************************************************
        public void SetProperties(
                                    FontFamily _fontFamily,
                                    string _stationName = "Hauptbahnhof",
                                    int _timerRefreshInterval = 15,
                                    int _timerRefreshStartInterval = 1,
                                    int _fontSize = 20
                                  )
        {
            stationName = _stationName;

            timerRefreshInterval = _timerRefreshInterval;
            timerRefreshStartInterval = _timerRefreshStartInterval;

            fontSize = _fontSize;
            listViewTimeTable.FontFamily = _fontFamily;
            listViewTimeTable.FontSize = fontSize;

            PrimaryForegroundColor = Common.PrimaryForegroundColor;
            SecondaryForegroundColor = Common.SecondaryForegroundColor;
            PrimaryBackgroundColor = Common.PrimaryBackgroundColor;
            SecondaryBackgroundColor = Common.SecondaryBackgroundColor;

            Common.CreateIconsDictionaryFromSVG(out Common.icons, fontSize);

            propertiesDefined = true;
        }

        /// ************************************************************************************************
        /// <summary>
        /// Start control's work. This function can be called after the calling SetProperties function only.
        /// </summary>
        /// ************************************************************************************************
        public void Start()
        {
            if (!propertiesDefined)
            {
                throw new Exception("Properties of the MVGTimeTable user control are not defined");
            }

            departureDataSource = new ObservableCollection<PreparedDeparture>();
            listViewTimeTable.ItemsSource = departureDataSource;

            // Table Refresh timer
            timerRefresh = new DispatcherTimer();
            timerRefresh.Tick += TimerRefresh_Tick;
            timerRefresh.Interval = TimeSpan.FromSeconds(timerRefreshStartInterval); //interval is updated in the TimerRefresh_Tick
            timerRefresh.Start();

            // Background Worker for asynchronic data receiving
            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.DoWork += new DoWorkEventHandler(BackgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundWorker1_RunWorkerCompleted);
        }


        /// ************************************************************************************************
        /// <summary>
        /// Add handlers to the Column Property Changed events for all columns except Destination
        /// </summary>
        /// ************************************************************************************************
        private void SetColumnSizeEventsHandlers()
        {
            GridView gridView = listViewTimeTable.View as GridView;
            for (int i = 0; i < gridView.Columns.Count; ++i)
            {
                if (i != Common.UtmostColumn)
                {
                    ((INotifyPropertyChanged)gridView.Columns[i]).PropertyChanged += ColumnWidthChanged;
                }
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Get string with the unique station ID
        /// </summary>
        /// <param name="_stationName">Simple station name in German, e.g. "Böhmerwaldplatz"</param>
        /// <returns>Station ID</returns>
        /// ************************************************************************************************
        private string GetStationID(string _stationName)
        {
            if (!string.IsNullOrEmpty(_stationName))
            {
                if (_stationName != savedStationName)
                {
                    savedStationID = MVGAPI.MVGAPI.GetIdForStation(_stationName);
                    if (MVGAPI.MVGAPI.IsConnected)
                    {
                        savedStationName = _stationName;
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
        /// Refresh Timer Event Handler
        /// </summary>
        /// ************************************************************************************************ 
        private void TimerRefresh_Tick(object sender, EventArgs e)
        {
            if (bindCounter != 1 && !backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                // Measurement of the table is not very fast, so Autosize for first time is called in secondsToFirstUpdate
                // measure table to get the true ActualSize of columns
                SetUtmostColumnWidth();
            }
            timerRefresh.Interval = TimeSpan.FromSeconds((bindCounter == 0) ? secondsToFirstUpdate : timerRefreshInterval);

            if (bindCounter < 2)
            {
                bindCounter++;
            }

        }

        /// ************************************************************************************************
        /// <summary>
        /// Start new asynchronous data receiving 
        /// </summary>
        /// ************************************************************************************************
        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            stationID = GetStationID(stationName);
            if (!string.IsNullOrEmpty(stationID))
            {
                departureResponse = MVGAPI.MVGAPI.GetDeserializedDepartures(stationID) ?? departureResponse;
            }
            else
            {
                departureResponse = null;
            }
        }


        /// ************************************************************************************************
        /// <summary>
        /// Format received data
        /// </summary>
        /// ************************************************************************************************
        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            int? responseHashCode;
            DateTime now = DateTime.Now;

            //Check connection status 
            if (MVGAPI.MVGAPI.IsConnected)
            {
                if (!savedConnectionStatus)
                {
                    savedConnectionStatus = true;
                    ConnectionStatusChangedEvent?.Invoke(this, new EventArgs());
                }
            }
            else
            {
                if (savedConnectionStatus)
                {
                    savedConnectionStatus = false;
                    timeConnectionLost = DateTime.Now;
                    ConnectionStatusChangedEvent?.Invoke(this, new EventArgs());
                }
            }
            double noConnectionSeconds = savedConnectionStatus ? 0.0 : (DateTime.Now - timeConnectionLost).TotalSeconds;

            //Build departureDataSource if connection is OK or disconnect is less than noConnectionTimeoutThreshold
            if (noConnectionSeconds <= noConnectionTimeoutThreshold2 && departureResponse != null)
            {
                responseHashCode = GetResponseHashCode(departureResponse, now);
                if (responseHashCode != oldResponseHashCode)
                {
                    oldResponseHashCode = responseHashCode;

                    MVGAPI.MVGAPI.FormatNewAPItoOld(ref departureResponse);
                    MVGAPI.MVGAPI.DeleteDuplicates(ref departureResponse);
                    MVGAPI.MVGAPI.Sort(ref departureResponse);
                    ParseDestination.ProcessForkLines(ref departureResponse);

                    BuildDepartureDataSource(now, departureResponse, ref departureDataSource);

                    //Use previous data if connection is lost for short time, but add warnings on the very top of the table
                    if (noConnectionSeconds > noConnectionTimeoutThreshold1)
                    {
                        departureDataSource.Insert(0, SetServiceMessage(Common.WarnMessageType[MessageType.Warning], Common.Messages[MessageType.Warning]));
                        departureDataSource.Insert(0, SetServiceMessage(Common.WarnMessageType[MessageType.NoConnection], Common.Messages[MessageType.NoConnection]));
                        bindCounter = 0;
                    }
                }
            }
            else
            {
                //Show only warning string if disconnection is longer than noConnectionTimeoutThreshold or no response was received
                departureDataSource.Clear();
                departureDataSource.Add(SetServiceMessage(Common.WarnMessageType[MessageType.NoConnection], Common.Messages[MessageType.NoConnection]));
                bindCounter = 0;
            }

            AutoSizeColumns();
            SetUtmostColumnWidth();
        }


        /// ************************************************************************************************
        /// <summary>
        /// Build departureDataSourse from departureResponse for now
        /// </summary>
        /// <param name="deserializedDepartures"></param>
        /// <param name="preparedDepartureCollection"></param>
        /// <returns>Hash code for departure response</returns>
        /// ************************************************************************************************
        private void BuildDepartureDataSource(DateTime now, DeserializedDepartures[] deserializedDepartures, ref ObservableCollection<PreparedDeparture> preparedDepartureCollection)
        {
            preparedDepartureCollection?.Clear();

            foreach (DeserializedDepartures dp in deserializedDepartures)
            {
                // Don't include cancelled lines into data source for visualization
                if (!dp.cancelled)
                {
                    GetTimes(now, dp.departureTime, dp.delay, out DateTime actualTime, out _, out TimeSpan difference);

                    PreparedDeparture fd = new PreparedDeparture
                    {
                        Product = dp.product,
                        Label = dp.label,
                        Destination = dp.destination,
                        Platform = dp.platform,
                        Station = stationName,
                        LineBackgroundColor = dp.lineBackgroundColor,
                        MinutesToDeparture = BuildMinutesToDeparture(difference),
                        DepartureTime = string.Format(CultureInfo.InvariantCulture, "{0:D2}:{1:D2}", actualTime.Hour, actualTime.Minute),
                        FontSize = fontSize.ToString(CultureInfo.InvariantCulture),
                        Sev = dp.sev,
                        Delay = ""
                    };

                    preparedDepartureCollection.Add(fd);
                }
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Builds minutes to departure string. If minutes >= 60 format of string changes to Hours:Minutes
        /// </summary>
        /// <param name="difference">Difference between now and time of departure</param>
        /// <returns>Minutes or Hours to departure string</returns>
        /// ************************************************************************************************
        private string BuildMinutesToDeparture(TimeSpan difference)
        {
            StringBuilder minutesToDeparture = new StringBuilder();

            if ((int)difference.TotalMinutes < 60)
            {
                minutesToDeparture.Append(((int)difference.TotalMinutes).ToString(CultureInfo.InvariantCulture));
                minutesToDeparture.Append(Common.TimeSignSeparator);
                minutesToDeparture.Append(Common.MinutesSign);
            }
            else
            {
                int hours = (int)difference.TotalMinutes / 60;
                int minutes = (int)difference.TotalMinutes % 60;
                minutesToDeparture.Append(string.Format(CultureInfo.InvariantCulture, "{0:D1}:{1:D2}", hours, minutes));
                minutesToDeparture.Append(Common.TimeSignSeparator);
                minutesToDeparture.Append(Common.HoursSign);
            }
            return minutesToDeparture.ToString();
        }


        /// ************************************************************************************************
        /// <summary>
        /// Calculate actual and scheduled times of departure in DateTime format and difference between now and actual departure time
        /// </summary>
        /// <param name="dpTime">Time from DeserializedDeparture in Unix timespan format</param>
        /// <param name="dpDelay">Delay in minutes from DeserializedDeparture</param>
        /// <param name="actualTime">Actual time of departure</param>
        /// <param name="scheduledTime">Scheduled time of departure</param>
        /// <param name="difference">Difference between Actual time and now</param>
        /// ************************************************************************************************
        private static void GetTimes(DateTime now, long dpTime, long dpDelay, out DateTime actualTime, out DateTime scheduledTime, out TimeSpan difference)
        {
            actualTime = DateTimeOffset.FromUnixTimeMilliseconds(dpTime).DateTime.ToLocalTime();
            scheduledTime = DateTimeOffset.FromUnixTimeMilliseconds(dpTime - dpDelay * 1000 * 60).DateTime.ToLocalTime();
            difference = actualTime.Subtract(now);
        }

        /// ************************************************************************************************
        /// <summary>
        /// Calculate Hash Code for the DeserializedDeparture array with actual time 
        /// </summary>
        /// <param name="deserializedDepartures">DeserializedDeparture array</param>
        /// <param name="now">Actual time</param>
        /// <returns>Hash code or null</returns>
        /// ************************************************************************************************
        private int? GetResponseHashCode(DeserializedDepartures[] deserializedDepartures, DateTime now)
        {
            StringBuilder allValuesString = new StringBuilder();

            foreach (DeserializedDepartures deserializedDeparture in deserializedDepartures)
            {
                GetTimes(now, deserializedDeparture.departureTime, deserializedDeparture.delay, out _, out _, out TimeSpan difference);

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
        /// Set Service Message as PreparedDeparture
        /// </summary>
        /// <param name="type">Type of the Service Message</param>
        /// <param name="message">Service message</param>
        /// <returns>Prepared departure with Service Message</returns>
        /// ************************************************************************************************
        private PreparedDeparture SetServiceMessage(string type, string message)
        {
            PreparedDeparture fd = new PreparedDeparture
            {
                DepartureTime = "",
                Label = "",
                MinutesToDeparture = "",
                Product = type,
                Destination = message,
                FontSize = fontSize.ToString(CultureInfo.InvariantCulture)
            };
            return fd;
        }

        /// ************************************************************************************************
        /// <summary>
        /// Column Width Changed Event Handler
        /// </summary>
        /// ************************************************************************************************
        private void ColumnWidthChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is GridViewColumn && e.PropertyName == "ActualWidth")
            {
                GridViewColumn gridViewColumn = (GridViewColumn)sender;
                if (gridViewColumn.ActualWidth > 0 && gridViewColumn.ActualWidth != savedWidths[gridViewColumn.Header.ToString()])
                {
                    savedWidths[gridViewColumn.Header.ToString()] = gridViewColumn.ActualWidth;
                }
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Autosize Columns of ListView. It's needed after update of the binded data.
        /// </summary>
        /// ************************************************************************************************
        private void AutoSizeColumns()
        {
            if (listViewTimeTable.View is GridView gridView)
            {
                for (int i = 0; i < gridView.Columns.Count; ++i)
                {
                    if (gridView.Columns[i].Header.ToString() == Common.ColumnName[Column.Destination]) continue;
                    if (double.IsNaN(gridView.Columns[i].Width))
                    {
                        gridView.Columns[i].Width = gridView.Columns[i].ActualWidth;
                    }
                    gridView.Columns[i].Width = double.NaN;
                }
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Calculate width of the utmost column after an autosizing
        /// </summary>
        /// ************************************************************************************************
        private void SetUtmostColumnWidth()
        {
            listViewTimeTable.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            listViewTimeTable.Arrange(new Rect(0, 0, listViewTimeTable.DesiredSize.Width, listViewTimeTable.DesiredSize.Height));

            GridView gridView = listViewTimeTable.View as GridView;

            if (Common.UtmostColumn >= gridView.Columns.Count) return;

            double fullColumnsWidth = 0;

            foreach (string key in savedWidths.Keys)
            {
                if (key == Common.ColumnName[Column.Destination]) continue;
                if (savedWidths[key] == 0) // There is at least one unmeasured column. I don't know why it happens but it's true.
                {
                    return;
                }
                fullColumnsWidth += savedWidths[key];
            }
            double utmostWidth = this.ActualWidth - fullColumnsWidth;
            if (utmostWidth > 0)
            {
                gridView.Columns[Common.UtmostColumn].Width = utmostWidth;
            }

        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //dispose managed resources
                ((IDisposable)backgroundWorker1).Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}


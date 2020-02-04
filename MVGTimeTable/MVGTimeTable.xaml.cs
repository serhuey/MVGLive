// Copyright (c) Sergei Grigorev. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using MVGAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;


namespace MVGTimeTable
{

    public partial class MVGTimeTable : UserControl, IDisposable
    {
        #region Properties
        public PreparedDeparture[] Departures { get; set; }
        public bool NoConnection => (noConnectionTimeoutCounter == 0) ? false : true; 
        #endregion

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

        private Dictionary<string, double> savedWidths = new Dictionary<string, double>
        {
            { Common.ColumnName[Column.Line], 0 },
            { Common.ColumnName[Column.Destination], 0 },
            { Common.ColumnName[Column.TimeToDeparture], 0 },
            { Common.ColumnName[Column.DepartureTime], 0 },
            { Common.ColumnName[Column.Platform], 0 }
        };

        private int fontSize;
        private string fontFamily;

        private int noConnectionTimeoutCounter = 0;
        private const int noConnectionTimeoutThreshold1 = 3;
        private const int noConnectionTimeoutThreshold2 = 10;


        public MVGTimeTable()
        {
            InitializeComponent();
            SetColumnSizeEventsHandlers();
        }

        /// <summary>
        /// Set MVGTimeTable Control properties. It must be performed before start the control.
        /// </summary>
        /// <param name="_stationName">Station name in German</param>
        /// <param name="_timerRefreshInterval">Interval between two Http requests</param>
        /// <param name="_timerRefreshStartInterval">Pause before first request</param>
        /// <param name="_fontSize">Font size in pixels</param>
        /// <param name="_fontFamily">Font family</param>
        public void SetProperties(
                                    string _stationName = "Hauptbahnhof",
                                    int _timerRefreshInterval = 15,
                                    int _timerRefreshStartInterval = 1,
                                    int _fontSize = 20,
                                    string _fontFamily = "Segoe UI"
                                  )
        {
            stationName = _stationName;

            timerRefreshInterval = _timerRefreshInterval;
            timerRefreshStartInterval = _timerRefreshStartInterval;

            fontSize = _fontSize;
            fontFamily = _fontFamily;

            listViewTimeTable.FontFamily = new FontFamily(fontFamily);
            listViewTimeTable.FontSize = fontSize;

            SetStackPanelMargin("stackPanelProductLabel", new Thickness(fontSize / 3.0, 0, fontSize / 2.0, 0));
            SetStackPanelMargin("stackPanelMainDestination", new Thickness(fontSize / 2.0, 0, fontSize / 2.0, 0));
            SetStackPanelMargin("stackPanelMinutes", new Thickness(fontSize / 3.0, 0, fontSize / 3.0, 0));

            Common.CreateIconsDictionaryFromPNG(out Common.icons, fontSize); //Create from SVG will be added in the future

            propertiesDefined = true;
        }

        /// <summary>
        /// Start control's work. This function can be called after the calling SetProperties function only.
        /// </summary>
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

        /// <summary>
        /// Add handlers to the Column Property Changed events for all columns except Destination
        /// </summary>
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

        /// <summary>
        /// Get string with the unique station ID
        /// </summary>
        /// <param name="_stationName">Simple station name in German, e.g. "Böhmerwaldplatz"</param>
        /// <returns>Station ID</returns>
        private string GetStationID(string _stationName)
        {
            if (!string.IsNullOrEmpty(_stationName))
            {
                if (_stationName != savedStationName)
                {
                    savedStationID = MVGAPI.MVGAPI.GetIdForStation(_stationName);
                    if (MVGAPI.MVGAPI.IsConnected) savedStationName = _stationName;
                    return savedStationID;
                }
                else
                {
                    return savedStationID;
                }
            }
            return null;
        }

        /// <summary>
        /// Refresh Timer Event Handler
        /// </summary>
        void TimerRefresh_Tick(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync();
            timerRefresh.Interval = TimeSpan.FromSeconds(timerRefreshInterval);
        }


        /// <summary>
        /// Start new asynchronous data receiving 
        /// </summary>
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

        /// <summary>
        /// Format received data
        /// </summary>
        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Check connection status 
            if (MVGAPI.MVGAPI.IsConnected)
            {
                if (noConnectionTimeoutCounter != 0)
                {
                    noConnectionTimeoutCounter = 0;
                    ConnectionStatusChangedEvent?.Invoke(this, new EventArgs());
                }
            }
            else
            {
                noConnectionTimeoutCounter++;
                if (noConnectionTimeoutCounter == 1)
                {
                    ConnectionStatusChangedEvent?.Invoke(this, new EventArgs());
                }
            }

            if (noConnectionTimeoutCounter <= noConnectionTimeoutThreshold2 && departureResponse != null)
            {
                //Build departureDataSource if connection is OK or disconnect is less than noConnectionTimeoutThreshold
                MVGAPI.MVGAPI.FormatNewAPItoOld(ref departureResponse);
                MVGAPI.MVGAPI.DeleteDuplicates(ref departureResponse);
                MVGAPI.MVGAPI.Sort(ref departureResponse);
                BuildDepartureDataSource(departureResponse, ref departureDataSource);

                //Use previous data if connection is lost for short time, but add warnings on the very top of the table
                if (noConnectionTimeoutCounter > noConnectionTimeoutThreshold1)
                {
                    departureDataSource.Insert(0, SetServiceMessage(Common.WarnMessageType[MessageType.Warning], Common.Messages[MessageType.Warning]));
                    departureDataSource.Insert(0, SetServiceMessage(Common.WarnMessageType[MessageType.NoConnection], Common.Messages[MessageType.NoConnection]));
                }
            }
            else
            {
                //Show only warning string if disconnection is longer than noConnectionTimeoutThreshold or no response was received
                departureDataSource.Clear();
                departureDataSource.Add(SetServiceMessage(Common.WarnMessageType[MessageType.NoConnection], Common.Messages[MessageType.NoConnection]));
            }

            AutoSizeColumns();
            SetUtmostColumnWidth();
        }

        /// <summary>
        /// Build departureDataSourse from departureResponse
        /// </summary>
        private void BuildDepartureDataSource(DeserializedDepartures[] deserializedDepartures, ref ObservableCollection<PreparedDeparture> preparedDepartureCollection)
        {
            DateTime now = DateTime.Now;
            preparedDepartureCollection?.Clear();

            foreach (DeserializedDepartures dp in deserializedDepartures)
            {
                DateTime localDateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(dp.departureTime).DateTime.ToLocalTime();
                DateTime scheduledTime = DateTimeOffset.FromUnixTimeMilliseconds(dp.departureTime - dp.delay * 1000 * 60).DateTime.ToLocalTime();
                TimeSpan difference = localDateTimeOffset.Subtract(now);

                PreparedDeparture fd = new PreparedDeparture
                {
                    Product = dp.product,
                    Label = dp.label,
                    Destination = dp.destination,
                    Platform = dp.platform,
                    Station = stationName
                };

                if ((int)difference.TotalMinutes < 60)
                {
                    fd.MinutesToDeparture = ((int)difference.TotalMinutes).ToString(CultureInfo.InvariantCulture) + Common.TimeSignSeparator + Common.MinutesSign;
                }
                else
                {
                    int hours = (int)difference.TotalMinutes / 60;
                    int minutes = (int)difference.TotalMinutes % 60;
                    fd.MinutesToDeparture = string.Format(CultureInfo.InvariantCulture, "{0:D1}:{1:D2}", hours, minutes) + Common.TimeSignSeparator + Common.HoursSign;
                }

                fd.DepartureTime = string.Format(CultureInfo.InvariantCulture, "{0:D2}:{1:D2}", localDateTimeOffset.Hour, localDateTimeOffset.Minute);
                fd.FontSize = fontSize.ToString(CultureInfo.InvariantCulture);
                fd.Sev = dp.sev;
                fd.Delay = "";
                preparedDepartureCollection.Add(fd);
            }
        }

        /// <summary>
        /// Set Service Message as PreparedDeparture
        /// </summary>
        /// <param name="type">Type of the Service Message</param>
        /// <param name="message">Service message</param>
        /// <returns></returns>
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

        /// <summary>
        /// Column Width Changed Event Handler
        /// </summary>
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

        /// <summary>
        /// Autosize Columns of ListView. It's needed after update of the binded data.
        /// </summary>
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

        /// <summary>
        /// Calculate width of the utmost column after an autosizing
        /// </summary>
        private void SetUtmostColumnWidth()
        {
            // measure table to get the true ActualSize of columns
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
            gridView.Columns[Common.UtmostColumn].Width = this.ActualWidth - fullColumnsWidth;
        }

        /// <summary>
        /// Set margins of the Stack Panel
        /// </summary>
        /// <param name="stackPanelName">Name of the Stack Panel</param>
        /// <param name="margin">Margin of the Stack Panel</param>
        /// <returns>True if operation is successfull</returns>
        private bool SetStackPanelMargin(string stackPanelName, Thickness margin)
        {
            bool success = false;
            object ob = this.FindName(stackPanelName);
            if (ob is StackPanel)
            {
                ((StackPanel)ob).Margin = margin;
                success = true;
            }
            return success;
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


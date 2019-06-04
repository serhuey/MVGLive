﻿using MVGAPI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace MVGTimeTable
{

    public partial class MVGTimeTable : UserControl, IDisposable
    {
        #region Properties
        public PreparedDeparture[] departures { get; set; }
        #endregion

        private bool propertiesDefined = false;
        private BackgroundWorker backgroundWorker1;
        private DispatcherTimer timerRefresh;
        private DispatcherTimer timerClock;

        private int timerRefreshInterval;
        private int timerRefreshStartInterval;

        private string stationName;
        private string savedStationName;
        private string savedStationID;

        private int fontSize;
        private string fontFamily;
        private DeserializedDepartures[] departureResponse;
        public ObservableCollection<PreparedDeparture> departureDataSource;



        public MVGTimeTable()
        {
           InitializeComponent();
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
                                    string   _stationName               = "Hauptbahnhof",
                                    int      _timerRefreshInterval      = 15,
                                    int      _timerRefreshStartInterval = 1,
                                    int      _fontSize                  = 20,
                                    string   _fontFamily                = "Arial"
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

            propertiesDefined = true;
        }

        /// <summary>
        /// Start control's work. This function can be called after the calling SetProperties function only.
        /// </summary>
        public void Start()
        {
            if(!propertiesDefined)
            {
                throw new Exception("Properties of the MVGTimeTable user control are not defined");
            }

            departureDataSource = new ObservableCollection<PreparedDeparture>();
            listViewTimeTable.ItemsSource = departureDataSource;
            departureDataSource.CollectionChanged += departureDataSource_CollectionChanged;

            // Table Refresh timer
            timerRefresh = new DispatcherTimer();
            timerRefresh.Interval = TimeSpan.FromSeconds(timerRefreshStartInterval); //will be updated in handler
            timerRefresh.Tick += timerRefresh_Tick;
            timerRefresh.Start();

            // Clock Refresh Timer
            timerClock = new DispatcherTimer();
            timerClock.Interval = TimeSpan.FromSeconds(1);
            timerClock.Tick += timerClock_Tick;
            timerClock.Start();

            // Background Worker for asynchronic data receiving
            backgroundWorker1 = new BackgroundWorker();
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
        }


        /// <summary>
        /// Set margins of the Stack Panel
        /// </summary>
        /// <param name="stackPanelName">Name of the Stack Panel</param>
        /// <param name="margin">Margin of the Stack Panel</param>
        /// <returns></returns>
        private bool SetStackPanelMargin(string stackPanelName, Thickness margin)
        {
            bool result = false;
            object ob = this.FindName(stackPanelName);
            if (ob is StackPanel)
            {
                ((StackPanel)ob).Margin = margin;
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Handler for Collection Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void departureDataSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            AutoSizeColumns();
        }

        /// <summary>
        /// Autosize Columns of ListView. It's needed after update of the binded data.
        /// </summary>
        private void AutoSizeColumns()
        {
            GridView gv = listViewTimeTable.View as GridView;
            if (gv != null)
            {
                foreach (var c in gv.Columns)
                {
                    // Code below was found in GridViewColumnHeader.OnGripperDoubleClicked() event handler (using Reflector)
                    // i.e. it is the same code that is executed when the gripper is double clicked
                    if (double.IsNaN(c.Width))
                    {
                        c.Width = c.ActualWidth;
                    }
                    c.Width = double.NaN;
                }
            }
        }

        /// <summary>
        /// Get string with the unique station ID
        /// </summary>
        /// <param name="_stationName">Simple station name in German, e.g. "Böhmerwaldplatz"</param>
        /// <returns></returns>
        private string GetStationID(string _stationName)
        {
            if (!string.IsNullOrEmpty(_stationName))
            {
                if (_stationName != savedStationName)
                {
                    savedStationName = _stationName;
                    savedStationID = MVGAPI.MVGAPI.GetIdForStation(_stationName).ToString();
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timerRefresh_Tick(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync();
            timerRefresh.Interval = TimeSpan.FromSeconds(timerRefreshInterval);
        }

        /// <summary>
        /// Clock Timer Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timerClock_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            string time = now.Hour.ToString() + ":" + now.Minute.ToString() + ":" + now.Second.ToString();
        }

        /// <summary>
        /// Start new asynchronous data receiving 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string stationID = GetStationID(stationName);
            if (!string.IsNullOrEmpty(stationID))
            {
                departureResponse = MVGAPI.MVGAPI.GetDeserializedDepartures(stationID) ?? departureResponse;
#if DEBUG
                Console.WriteLine("Departure request " + stationName);
#endif
            }
        }

        /// <summary>
        /// Format received data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (e.Error == null)
            {
                departureDataSource.Clear();

                if (departureResponse != null)
                {
                    Array.Sort(departureResponse, delegate (DeserializedDepartures dp1, DeserializedDepartures dp2)
                    {
                        return dp1.departureTime.CompareTo(dp2.departureTime);
                    });
                    foreach (DeserializedDepartures dp in departureResponse)
                    {
                        StringBuilder sb = new StringBuilder();

                        DateTime now = DateTime.Now;
                        DateTime localDateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(dp.departureTime).DateTime.ToLocalTime();
                        TimeSpan difference = localDateTimeOffset.Subtract(now);

                        PreparedDeparture fd = new PreparedDeparture();
                        fd.product = dp.product;
                        fd.label = dp.label;
                        fd.destination = dp.destination;
                        fd.minutesToDeparture = ((int)difference.TotalMinutes).ToString() + " min";
#if DEBUG
                        Console.WriteLine(fd.minutesToDeparture);
#endif
                        fd.departureTime = String.Format("{0:D2}:{1:D2}:{2:D2}", localDateTimeOffset.Hour, localDateTimeOffset.Minute, localDateTimeOffset.Second);
                        fd.fontSize = fontSize.ToString();
                        fd.sev = dp.sev;
                        departureDataSource.Add(fd);
                        sb.Append(dp.product + " " + dp.label + " " + dp.destination + " " + ((int)difference.TotalMinutes).ToString() + "min." + Environment.NewLine);
                    }
                }
            }
            else
            {
                departureDataSource.Clear();
                PreparedDeparture fd = new PreparedDeparture();
                fd.departureTime = "";
                fd.label = "";
                fd.minutesToDeparture = "";
                fd.product = "";
                fd.destination = "Failed to connect https://www.mvg.de";
                departureDataSource.Add(fd);
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
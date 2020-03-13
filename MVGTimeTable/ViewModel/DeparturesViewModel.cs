// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MVGAPI;
using MVGTimeTable.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace MVGTimeTable.ViewModel
{
    public class DeparturesViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly DeparturesModel departuresModel;
        private ConnectionState connectionStatus;

        public string StationName { get; }
        public FontFamily TableFontFamily { get; }
        public double TableFontSize { get; }
        public FontFamily HeaderFontFamily { get; }
        public double HeaderFontSize { get; }

        public string FirstBackgroundColor { get; } = Common.FirstBackgroundColor;
        public string SecondBackgroundColor { get; } = Common.SecondBackgroundColor;
        public string HeaderBackgroundColor { get; } = Common.HeaderBackgroundColor;
        public string HeaderForegroundColor { get; } = Common.HeaderForegroundColor;

        public Thickness HeaderMargin { get; } = new Thickness(0);

        public ObservableCollection<PreparedDeparture> PreparedDepartures { get; set; } = new ObservableCollection<PreparedDeparture>();

        public DeparturesViewModel(string stationName, FontFamily fontFamily, double fontSize, int timerRefreshInterval, int timerRefreshStartInterval) :
            this(stationName, fontFamily, fontSize, timerRefreshInterval, timerRefreshStartInterval, fontFamily, fontSize)
        {

        }

        /// ************************************************************************************************
        /// <summary>
        /// Constructor with partameters
        /// </summary>
        /// <param name="stationName">Station name in German. E.g. "Böhmerwaldplatz"</param>
        /// <param name="fontFamily">Font family</param>
        /// <param name="fontSize">Font size - the main parameter for scaling</param>
        /// <param name="timerRefreshInterval">Refresh interval of the departure table in seconds</param>
        /// <param name="timerRefreshStartInterval">Delay of the first update of the departure table in seconds</param>
        /// <param name="headerFontFamily">Header font family</param>
        /// <param name="headerFontSize">Header font size</param>
        /// ************************************************************************************************
        public DeparturesViewModel(string stationName, FontFamily fontFamily, double fontSize, int timerRefreshInterval, int timerRefreshStartInterval, FontFamily headerFontFamily, double headerFontSize)
        {
            StationName = stationName;
            departuresModel = new DeparturesModel(stationName, timerRefreshInterval, timerRefreshStartInterval);
            TableFontSize = fontSize;
            TableFontFamily = fontFamily;
            HeaderFontFamily = headerFontFamily;
            HeaderFontSize = headerFontSize;

            Common.CreateIconsDictionaryFromSVG(out Common.icons, TableFontSize);

            AddHeaderToDepartureDataSource(PreparedDepartures, empty: true);
            PreparedDepartures.Add(SetServiceMessage(Common.WarnMessageType[MessageType.Waiting], Common.Messages[MessageType.Waiting]));

            departuresModel.PropertyChanged += DeparturesModel_PropertyChanged;
            departuresModel.Start();
        }

        /// ************************************************************************************************
        /// <summary>
        /// Model PropertyChanged event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// ************************************************************************************************
        private void DeparturesModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(departuresModel.DepartureResponse):
                    BuildDepartureDataSource(DateTime.Now, departuresModel.DepartureResponse, PreparedDepartures);
                    break;

                case nameof(departuresModel.ConnectionStatus):
                    connectionStatus = departuresModel.ConnectionStatus;
                    break;
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Build departureDataSourse from departureResponse for now
        /// </summary>
        /// <param name="deserializedDepartures"></param>
        /// <param name="preparedDepartureCollection"></param>
        /// <returns>Hash code for departure response</returns>
        /// ************************************************************************************************
        private void BuildDepartureDataSource(DateTime now, DeserializedDepartures[] deserializedDepartures, ObservableCollection<PreparedDeparture> preparedDepartures)
        {
            preparedDepartures.Clear();

            if (connectionStatus == ConnectionState.NOT_CONNECTED)
            {
                AddHeaderToDepartureDataSource(preparedDepartures, empty: true);
                preparedDepartures.Add(SetServiceMessage(Common.WarnMessageType[MessageType.NoConnection], Common.Messages[MessageType.NoConnection]));
            }
            else
            {
                if (connectionStatus == ConnectionState.JUST_STARTED && (deserializedDepartures == null || (deserializedDepartures != null && deserializedDepartures.Length == 0)))
                {
                    AddHeaderToDepartureDataSource(preparedDepartures, empty: true);
                    preparedDepartures.Add(SetServiceMessage(Common.WarnMessageType[MessageType.Waiting], Common.Messages[MessageType.Waiting]));
                }
                else
                {
                    AddHeaderToDepartureDataSource(preparedDepartures, empty: false);

                    if (connectionStatus == ConnectionState.LONG_DISCONNECT)
                    {
                        preparedDepartures.Add(SetServiceMessage(Common.WarnMessageType[MessageType.NoConnection], Common.Messages[MessageType.NoConnection]));
                        preparedDepartures.Add(SetServiceMessage(Common.WarnMessageType[MessageType.Warning], Common.Messages[MessageType.Warning]));
                    }
                    if (deserializedDepartures != null)
                    {
                        foreach (DeserializedDepartures dp in deserializedDepartures)
                        {
                            // Don't include cancelled lines into data source for visualization
                            if (!dp.cancelled)
                            {
                                Common.GetTimes(now, dp.departureTime, dp.delay, out DateTime actualTime, out _, out TimeSpan difference);

                                PreparedDeparture fd = new PreparedDeparture
                                {
                                    Product = dp.product,
                                    Label = dp.label,
                                    Destination = dp.destination,
                                    Platform = dp.platform,
                                    Station = StationName,
                                    LineBackgroundColor = dp.lineBackgroundColor,
                                    MinutesToDeparture = BuildMinutesToDeparture(difference),
                                    DepartureTime = string.Format(CultureInfo.InvariantCulture, "{0:D2}:{1:D2}", actualTime.Hour, actualTime.Minute),
                                    FontSize = TableFontSize.ToString(CultureInfo.InvariantCulture),
                                    Sev = dp.sev,
                                    Delay = dp.delay.ToString(),
                                    FontFamily = TableFontFamily
                                };

                                preparedDepartures.Add(fd);
                            }
                        }
                    }
                }
            }
        }

        private void AddHeaderToDepartureDataSource(ObservableCollection<PreparedDeparture> preparedDepartures, bool empty = false)
        {
            PreparedDeparture hd = new PreparedDeparture
            {
                Product = Common.HeaderProduct,
                Label = empty ? "" : "Linie",
                Destination = empty ? "" : "Ziel",
                Platform = empty ? "" : ((departuresModel.SGleisColumnPresent) ? "Gleis" : (departuresModel.GleisColumnPresent ? "HSt." : "")),
                Station = StationName,
                LineBackgroundColor = "",
                MinutesToDeparture = empty ? "" : "Abfahrt",
                DepartureTime = empty ? "" : "Zeit",
                FontSize = empty ? "1" : HeaderFontSize.ToString(CultureInfo.InvariantCulture),
                Sev = false,
                Delay = "",
                FontFamily = HeaderFontFamily
            };
            preparedDepartures.Add(hd);
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
                FontSize = TableFontSize.ToString(CultureInfo.InvariantCulture)
            };
            return fd;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    departuresModel.Dispose();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
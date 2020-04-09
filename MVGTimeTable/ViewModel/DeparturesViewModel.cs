// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MVGAPI;
using MVGTimeTable.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace MVGTimeTable.ViewModel
{
    public class DeparturesViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly DeparturesModel departuresModel;
        private ConnectionState connectionStatus;

        public string StationName { get; private set; }
        public FontFamily TableFontFamily { get; private set; }
        public int TableFontSize { get; private set; }
        public FontFamily HeaderFontFamily { get; private set; }
        public int HeaderFontSize { get; private set; }

        public string TableBackgroundColor1 { get; set; }
        public string TableBackgroundColor2 { get; set; }
        public string HeaderBackgroundColor { get; set; }
        public string HeaderForegroundColor { get; set; }
        public string TableForegroundColor1 { get; set; }
        public string TableForegroundColor2 { get; set; }
        public string TableForegroundColor3 { get; set; }
        public string NoConnectionForegroundColor { get; set; }
        public string WarningForegroundColor { get; set; }

        public Thickness HeaderMargin { get; } = new Thickness(0);
        public Dictionary<string, double> MinColumnsSize { get; set; } = new Dictionary<string, double>();
        public ObservableCollection<PreparedDeparture> PreparedDepartures { get; set; } = new ObservableCollection<PreparedDeparture>();

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="stationNumberProperty"></param>
        /// <param name="timerRefreshInterval"></param>
        /// <param name="timerRefreshStartInterval"></param>
        /// <param name="tableFontFamily"></param>
        /// <param name="headerFontFamily"></param>
        public DeparturesViewModel(ApplicationSettingsBase settings, string stationNumberProperty, int timerRefreshInterval, int timerRefreshStartInterval, FontFamily tableFontFamily, FontFamily headerFontFamily)
        {
            TableFontFamily = tableFontFamily;
            HeaderFontFamily = headerFontFamily;

            CopySettingsToProperties(settings, stationNumberProperty);
            Common.CreateIconsDictionaryFromSVG(out Common.icons, TableFontSize, TableForegroundColor1, TableForegroundColor2, TableForegroundColor3);

            AddHeaderToDepartureDataSource(PreparedDepartures, empty: true);
            PreparedDepartures.Add(SetServiceMessage(Common.WarnMessageType[MessageType.Waiting], Common.Messages[MessageType.Waiting]));
            FillMinColumnsSize();

            departuresModel = new DeparturesModel(StationName, timerRefreshInterval, timerRefreshStartInterval);
            departuresModel.PropertyChanged += DeparturesModel_PropertyChanged;
            departuresModel.Start();
        }

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="stationNumberProperty"></param>
        private void CopySettingsToProperties(ApplicationSettingsBase settings, string stationNumberProperty)
        {
            PropertyInfo[] properties = GetType().GetProperties();
            object obj;
            string propertyName;
            SettingsPropertyCollection settingsProperties = settings.Properties;
            List<string> settingsPropertiesNames = new List<string>();
            foreach(SettingsProperty settingsProperty in settingsProperties)
            {
                settingsPropertiesNames.Add(settingsProperty.Name);
            }
            foreach (PropertyInfo pInfo in properties)
            {
                propertyName = pInfo.Name;

                if (string.Compare(propertyName, nameof(StationName)) == 0)
                {
                    propertyName = stationNumberProperty;
                }

                if(!settingsPropertiesNames.Contains(propertyName))
                {
                    continue;
                }
                try
                {
                    obj = settings[propertyName];
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.ToString());
                    continue;
                }

                if (pInfo.PropertyType == settings[propertyName].GetType())
                {
                    var settingValue = settings[propertyName];
                    pInfo.SetValue(this, settingValue);
                }
            }
            if(string.IsNullOrEmpty(StationName))
            {
                throw new ArgumentException("The station name for DeparturesViewModel is not correct");
            }
        }


        /// ************************************************************************************************
        /// <summary>
        /// Model PropertyChanged event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="preparedDepartures"></param>
        /// <param name="empty"></param>
        private void AddHeaderToDepartureDataSource(ObservableCollection<PreparedDeparture> preparedDepartures, bool empty = false)
        {
            PreparedDeparture hd = new PreparedDeparture
            {
                Product = Common.HeaderProduct,
                Label = empty ? "" : Common.HeaderTitles["Line"],
                Destination = empty ? "" : Common.HeaderTitles["Destination"],
                Platform = empty ? "" : (departuresModel.SGleisColumnPresent ? Common.HeaderTitles["PlatformS"] : (departuresModel.GleisColumnPresent ? Common.HeaderTitles["PlatformT"] : "")),
                Station = StationName,
                LineBackgroundColor = "",
                MinutesToDeparture = empty ? "" : Common.HeaderTitles["TimeToDeparture"],
                DepartureTime = empty ? "" : Common.HeaderTitles["DepartureTime"],
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

        private void FillMinColumnsSize()
        {
            foreach(string column in Common.HeaderTitles.Keys)
            {
                MinColumnsSize.Add(column, Common.MeasureText(Common.HeaderTitles[column], HeaderFontFamily.ToString(), HeaderFontSize));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        /// ************************************************************************************************
        protected void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// ************************************************************************************************
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
// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MVGLive
{
    /// <summary>
    /// 
    /// </summary>
    public class StationEnabledIvc : IValueConverter
    {
        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null || !(value is MainWindowType)) return null;

            int stationNumber = 0;
            int displayedStations = 0;

            switch((string)parameter)
            {
                case "Station1parameter": stationNumber = 1; break;
                case "Station2parameter": stationNumber = 2; break;
                case "Station3parameter": stationNumber = 3; break;
                case "Station4parameter": stationNumber = 4; break;
            }

            switch((MainWindowType)value)
            {
                case MainWindowType.OneDestination: displayedStations = 1; break;
                case MainWindowType.TwoDestinationsVertical: displayedStations = 2; break;
                case MainWindowType.TwoDestinationsHorizontal: displayedStations = 2; break;
                case MainWindowType.ThreeDestinations: displayedStations = 3; break;
                case MainWindowType.FourDestinations: displayedStations = 4; break;
            }

            return (Visibility)(stationNumber <= displayedStations ? Visibility.Visible : Visibility.Collapsed);
        }

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

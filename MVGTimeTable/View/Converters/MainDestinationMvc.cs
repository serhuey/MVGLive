// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MVGTimeTable
{
    /// <summary>
    /// Returns destination string without additional destination if it exists
    /// </summary>
    public class MainDestinationMvc : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || values[0] == null)
            {
                return "";
            }

            string mainDestination = ParseDestination.GetMainDestination(values[0].ToString(), removeUS: true, removeSplitMarkers: true);
            string label = values[1].ToString();
            if(ParseDestination.IsMarkerPresent(label.ToUpperInvariant() , Common.LufthansaMarkers))
            {
                mainDestination = label + ": " + mainDestination;
            }
            return mainDestination;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
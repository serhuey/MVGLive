// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MVGTimeTable
{
    /// <summary>
    /// Returns string with line number for lines without graphic logo like most of U- and S-Bahn
    /// </summary>
    public class LabelMvc : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || values[0] == null || values[1] == null) return "";

            string product = values[0].ToString().ToUpperInvariant();
            string label = values[1].ToString().ToUpperInvariant();

            if (product.Contains(Common.WarnMessageType[MessageType.NoConnection]) ||
                product.Contains(Common.WarnMessageType[MessageType.Warning]) ||
                product.Contains(Common.WarnMessageType[MessageType.Waiting]))
            {
                return "";
            }

            if (ParseDestination.IsMarkerPresent(label, Common.LufthansaMarkers))
            {
                return "";
            }

            if (ParseDestination.IsMarkerPresent(product, Common.BusMarkers))
            {
                return label;
            }

            if (ParseDestination.IsMarkerPresent(product, Common.TramMarkers))
            {
                return string.IsNullOrEmpty(Array.Find(Common.TramIconKey, str => str.Contains(Common.DefaultTramIconKey.ToUpperInvariant() + label))) ? label : "";
            }

            if (ParseDestination.IsMarkerPresent(product, Common.UBahnMarkers))
            {
                return string.IsNullOrEmpty(Array.Find(Common.UBahnIconKey, str => str.Contains(label))) ? label : "";
            }

            if (ParseDestination.IsMarkerPresent(product, Common.SBahnMarkers))
            {
                return string.IsNullOrEmpty(Array.Find(Common.SBahnIconKey, str => str.Contains(label))) ? label : "";
            }


            return values[1].ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
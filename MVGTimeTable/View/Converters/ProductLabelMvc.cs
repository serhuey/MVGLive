// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Windows.Data;

namespace MVGTimeTable
{
    /// <summary>
    /// Sets graphic logo
    /// </summary>
    public class ProductLabelMvc : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null ||
                values.Length < 4 ||
                values[0] == null ||
                values[1] == null ||
                values[2] == null ||
                values[3] == null)
                return null;

            string iconKey = null;
            string product = values[0].ToString().ToUpperInvariant();
            string label = values[1].ToString().ToUpperInvariant();
            string destination = values[2].ToString().ToUpperInvariant();
            bool sev = (bool)values[3];

            bool bus = ParseDestination.IsMarkerPresent(product, Common.BusMarkers);
            bool night = label.Length > 0 && ParseDestination.IsMarkerPresent(label[0].ToString(), Common.NightLineMarkers);
            bool tram = ParseDestination.IsMarkerPresent(product, Common.TramMarkers);
            bool ubahn = ParseDestination.IsMarkerPresent(product, Common.UBahnMarkers);
            bool sbahn = ParseDestination.IsMarkerPresent(product, Common.SBahnMarkers);
            bool xbus = ParseDestination.IsMarkerPresent(label, Common.ExpressLineMarkers) && ParseDestination.IsMarkerPresent(label[0].ToString(), Common.ExpressLineMarkers);
            bool noConnection = product.Contains(Common.WarnMessageType[MessageType.NoConnection]);
            bool warning = product.Contains(Common.WarnMessageType[MessageType.Warning]);
            bool wait = product.Contains(Common.WarnMessageType[MessageType.Waiting]);
            bool lufthansa = ParseDestination.IsMarkerPresent(label, Common.LufthansaMarkers);

            if (bus)
            {
                iconKey = sev ? (night ? Common.NSevBusIconKey : Common.SevBusIconKey) :
                                (night ? Common.NBusIconKey :
                                        (xbus ? Common.ExpressBusIconKey : Common.BusIconKey)
                                );
            }

            if (tram)
            {
                iconKey = Array.Find(Common.TramIconKey, str => str.Contains(Common.DefaultTramIconKey.ToUpperInvariant() + label))
                          ?? (night ? Common.NTramIconKey : (sev ? Common.SevTramIconKey : Common.DefaultTramIconKey));
            }

            if (ubahn)
            {
                iconKey = Array.Find(Common.UBahnIconKey, str => str.Contains(label)) ?? Common.DefaultUBahnIconKey;
            }

            if (sbahn)
            {
                iconKey = Array.Find(Common.SBahnIconKey, str => str.Contains(label)) ?? Common.DefaultSBahnIconKey;
            }

            if (ParseDestination.IsMarkerPresent(destination, Common.AirportMarkers))
            {
                if (Common.AirportIconKeys.ContainsKey(label))
                {
                    iconKey = Common.AirportIconKeys[label];
                }
            }

            if(lufthansa)
            {
                iconKey = Common.LufthansaBusIconKey;
            }

            if (wait)
            {
                iconKey = Common.WaitingIconKey;
            }

            if (warning)
            {
                iconKey = Common.WarningIconKey;
            }

            if (noConnection)
            {
                iconKey = Common.NoConnectionIconKey;
            }

            if (!string.IsNullOrEmpty(iconKey) && Common.icons.ContainsKey(iconKey = iconKey.ToUpperInvariant()))
            {
                return Common.icons[iconKey];
            }
            else
            {
                return null;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
// Copyright (c) Sergei Grigorev. All rights reserved.  
// Licensed under the MIT License. See LICENSE file in the project root for full license information.  

using System;
using System.Globalization;
using System.Windows.Data;

namespace MVGTimeTable
{
    /// <summary>
    /// Set graphic logo
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

            bool bus = product.Contains(Common.BusMarker);
            bool night = label.Length > 0 && label[0].ToString() == Common.NightLineMarker;
            bool tram = product.Contains(Common.TramMarker);
            bool ubahn = product.Contains(Common.UBahnMarker);
            bool sbahn = product.Contains(Common.SBahnMarker);
            bool xbus = label.Contains(Common.ExpressLineMarker) && label[0].ToString() == Common.ExpressLineMarker;
            bool noConnection = product.Contains(Common.WarnMessageType[MessageType.NoConnection]);
            bool warning = product.Contains(Common.WarnMessageType[MessageType.Warning]);

            if (bus)
            {
                iconKey = (sev ? (night ? Common.NSevBusIconKey : Common.SevBusIconKey) : (night ? Common.NBusIconKey : (xbus ? Common.ExpressBusIconKey : Common.BusIconKey)));
            }

            if (tram)
            {
                iconKey = night ? Common.NTramIconKey : Common.TramIconKey;
            }

            if (ubahn)
                iconKey = Array.Find<string>(Common.UBahnIconKey, str => str.Contains(label)) ?? Common.DefaultUBahnIconKey;

            if (sbahn)
                iconKey = Array.Find<string>(Common.SBahnIconKey, str => str.Contains(label)) ?? Common.DefaultSBahnIconKey;

            if (destination.Contains(Common.AirportMarker))
            {
                if (Common.AirportIconKeys.ContainsKey(label))
                {
                    iconKey = Common.AirportIconKeys[label];
                }
            }

            if (warning) iconKey = Common.WarningIconKey;

            if (noConnection) iconKey = Common.NoConnectionIconKey;

            if (!string.IsNullOrEmpty(iconKey) && Common.icons.ContainsKey(iconKey = iconKey.ToLowerInvariant()))
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

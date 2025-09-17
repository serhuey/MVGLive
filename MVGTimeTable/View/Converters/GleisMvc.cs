// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace MVGTimeTable
{
    public class GleisMvc : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null ||
                values.Length < 3 ||
                values[0] == null ||
                values[1] == null ||
                values[2] == null)
                return null;

            string product = values[0].ToString();
            string gleis = values[1].ToString();
            string station = values[2].ToString().ToUpperInvariant();

            if (ParseDestination.IsMarkerPresent(product, Common.UBahnMarkers) ||
                string.Compare(product.ToUpperInvariant(), Common.HeaderProduct) == 0)
            {
                return null;
            }

            if ((Common.MultiplatformTramStations.Contains(station) || 
                Common.MultiplatformSbahnStations.Contains(station)) &&
                !string.IsNullOrWhiteSpace(gleis))
            {
                if (ParseDestination.IsMarkerPresent(product, Common.SBahnMarkers))
                {
                    if (int.TryParse(Regex.Match(gleis, @"\d+").Value, out int index) && index > 0 && index < Common.SGleisIconKey.Length)
                    {
                        string iconKey = Common.SGleisIconKey[index].ToUpperInvariant();
                        if (!string.IsNullOrEmpty(iconKey) && Common.icons.ContainsKey(iconKey))
                        {
                            return Common.icons[iconKey];
                        }
                    }
                }
                else 
                if (ParseDestination.IsMarkerPresent(product, Common.TramMarkers) ||
                    ParseDestination.IsMarkerPresent(product, Common.BusMarkers) ||
                    ParseDestination.IsMarkerPresent(product, Common.NightLineMarkers) ||
                    ParseDestination.IsMarkerPresent(product, Common.ExpressLineMarkers))
                {

                    if (int.TryParse(Regex.Match(gleis, @"\d+").Value, out int index) && index > 0 && index < Common.GleisIconKey.Length)
                    {
                        string iconKey = Common.GleisIconKey[index].ToUpperInvariant();
                        if (!string.IsNullOrEmpty(iconKey) && Common.icons.ContainsKey(iconKey))
                        {
                            return Common.icons[iconKey];
                        }
                    }
                }
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
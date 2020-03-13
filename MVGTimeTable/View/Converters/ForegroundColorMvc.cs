// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MVGTimeTable
{
    internal class ForegroundColorMvc : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush brush;
            string foregroundColor = "#FFFFFFFF";

            if (values != null && values.Length != 0 && values[0] != null && parameter != null)
            {
                string product = values[0].ToString();

                if (product == Common.WarnMessageType[MessageType.Warning])
                {
                    foregroundColor = Common.WarningForegroundColor;
                }
                else if (product == Common.WarnMessageType[MessageType.NoConnection])
                {
                    foregroundColor = Common.NoConnectionForegroundColor;
                }
                else if (product == Common.HeaderProduct)
                {
                    foregroundColor = Common.HeaderForegroundColor;
                }
                else
                {

                    switch (parameter.ToString())
                    {
                        case "labelProduct":
                            foregroundColor = Common.FirstForegroundColor;
                            break;
                        case "labelMainDestination":
                            foregroundColor = Common.FirstForegroundColor;
                            break;
                        case "labelAdditionalDestination":
                            foregroundColor = Common.SecondForegroundColor;
                            break;
                        case "labelGleis":
                            foregroundColor = Common.HeaderForegroundColor;
                            break;
                        case "labelTimeToDeparture":
                            foregroundColor = Common.FirstForegroundColor;
                            break;
                        case "labelDepartureTime":
                            foregroundColor = Common.SecondForegroundColor;
                            break;
                    }
                }
            }

            brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(foregroundColor));
            return brush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
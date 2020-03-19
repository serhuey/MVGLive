// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MVGTimeTable.ViewModel;
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

            if (values != null && values.Length != 0 && values[0] != null && values[1]!= null && parameter != null)
            {
                string product = values[0].ToString();
                DeparturesViewModel viewModel = values[1] as DeparturesViewModel;

                if (product == Common.WarnMessageType[MessageType.Warning])
                {
                    foregroundColor = viewModel.WarningForegroundColor ?? foregroundColor;
                }
                else if (product == Common.WarnMessageType[MessageType.NoConnection])
                {
                    foregroundColor = viewModel.NoConnectionForegroundColor ?? foregroundColor;
                }
                else if (product == Common.HeaderProduct)
                {
                    foregroundColor = viewModel.HeaderForegroundColor;
                }
                else
                {

                    switch (parameter.ToString())
                    {
                        case "labelProduct":
                            foregroundColor = viewModel.TableForegroundColor1 ?? foregroundColor;
                            break;
                        case "labelMainDestination":
                            foregroundColor = viewModel.TableForegroundColor1 ?? foregroundColor;
                            break;
                        case "labelAdditionalDestination":
                            foregroundColor = viewModel.TableForegroundColor2 ?? foregroundColor;
                            break;
                        case "labelGleis":
                            foregroundColor = viewModel.HeaderForegroundColor ?? foregroundColor;
                            break;
                        case "labelTimeToDeparture":
                            foregroundColor = viewModel.TableForegroundColor1 ?? foregroundColor;
                            break;
                        case "labelDepartureTime":
                            foregroundColor = viewModel.TableForegroundColor2 ?? foregroundColor;
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
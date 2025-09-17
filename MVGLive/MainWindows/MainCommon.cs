// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MVGTimeTable.ViewModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MVGLive.MainWindows
{
    /// <summary>
    /// Common function for all main windows
    /// </summary>
    public static class MainCommon
    {
        /// ************************************************************************************************
        /// <summary>
        /// Set all tables parameters
        /// </summary>
        /// <param name="labels">Labels array in right order</param>
        /// <param name="tables">Tables array in right order</param>
        public static void SetupTables(Label[] labels, MVGTimeTable.MVGTimeTable[] tables)
        {
            if (labels != null && labels.Length > 0 &&
                tables != null && tables.Length > 0)
            {
                int length = Math.Min(labels.Length, tables.Length);

                int interval = Properties.Settings.Default.TimerRefreshInterval;
                int startInterval = interval / length;
                int currentStartInterval = 0;

                for (int i = 0; i < length; ++i)
                {
                    string stationPropertyName = nameof(Properties.Settings.Default.Station1).Replace('1', Convert.ToChar((i + 1).ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));
                    tables[i].DataContext = new DeparturesViewModel(settings: Properties.Settings.Default,
                                                                    stationNumberProperty: stationPropertyName,
                                                                    timerRefreshInterval: interval,
                                                                    timerRefreshStartInterval: currentStartInterval,
                                                                    tableFontFamily: App.GetFontFromLibrary(Properties.Settings.Default.TableFontFamily),
                                                                    headerFontFamily: App.GetFontFromLibrary(Properties.Settings.Default.HeaderFontFamily),
                                                                    mvgApi: App.GetMvgApi());

                    currentStartInterval += startInterval;

                    labels[i].FontFamily = App.GetFontFromLibrary(Properties.Settings.Default.CaptionFontFamily);
                    labels[i].FontSize = Properties.Settings.Default.CaptionFontSize;
                    labels[i].Tag = Properties.Settings.Default[stationPropertyName];
                    labels[i].Margin = new Thickness(Properties.Settings.Default.TableFontSize * 0.5f, Properties.Settings.Default.TableFontSize * 0.1f, 0, 0);
                    labels[i].Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Properties.Settings.Default.CaptionForegroundColor));

                    if (labels[i].Parent != null && labels[i].Parent is Panel)
                    {
                        (labels[i].Parent as Panel).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Properties.Settings.Default.CaptionBackgroundColor));
                    }
                }
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Setups time label
        /// </summary>
        /// <param name="label">Label to setup</param>
        public static void SetupTimeLabel(Label label)
        {
            if (label != null)
            {
                label.FontFamily = App.GetFontFromLibrary(Properties.Settings.Default.ClockFontFamily);
                label.FontSize = Properties.Settings.Default.ClockFontSize;
                label.Margin = new Thickness(Properties.Settings.Default.ClockFontSize * 0.5f);
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Updates clock label
        /// </summary>
        /// <param name="label">Label to update</param>
        public static void UpdateClockLabel(Label label)
        {
            DateTime now = DateTime.Now;
            string time = now.Hour.ToString("D2", CultureInfo.InvariantCulture) + ":" + now.Minute.ToString("D2", CultureInfo.InvariantCulture) + ":" + now.Second.ToString("D2", CultureInfo.InvariantCulture);
            if (label != null)
            {
                label.Content = time;
            }
        }
    }
}

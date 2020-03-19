// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MVGTimeTable;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="labels"></param>
        /// <param name="tables"></param>
        /// <param name="settings"></param>
        public static void SetupTables(Label[] labels, MVGTimeTable.MVGTimeTable[] tables, MVGTimeTableSettings[] settings)
        {
            if (labels != null && labels.Length > 0 &&
                settings != null && settings.Length > 0 &&
                tables != null && tables.Length > 0)
            {
                int length = Math.Min(labels.Length, Math.Min(tables.Length, settings.Length));

                int interval = Properties.Settings.Default.TimerRefreshInterval;
                int startInterval = interval / length;
                int currentStartInterval = 0;

                for (int i = 0; i < length; ++i)
                {
                    tables[i].DataContext = new DeparturesViewModel(settings[i], interval, currentStartInterval);
                    currentStartInterval += startInterval;

                    labels[i].FontFamily = settings[i].TableFontFamily;
                    labels[i].FontSize = settings[i].TableFontSize * Properties.Settings.Default.CaptionFontSizeCoeff;
                    labels[i].Tag = settings[i].StationName;
                    labels[i].Margin = new Thickness(settings[i].TableFontSize * 0.5f, settings[i].TableFontSize * 0.1f, 0, 0);
                    labels[i].Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Properties.Settings.Default.CaptionForegroundColor));
                    if (labels[i].Parent != null && labels[i].Parent is Panel)
                    {
                        (labels[i].Parent as Panel).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Properties.Settings.Default.HeaderBackgroundColor));
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="fontFamily"></param>
        /// <param name="fontSize"></param>
        public static void SetupTimeLabel(Label label, FontFamily fontFamily, double fontSize)
        {
            if (label != null)
            {
                label.FontFamily = fontFamily;
                label.FontSize = fontSize;
                label.Margin = new Thickness(fontSize * 0.5f);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
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

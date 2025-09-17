// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MVGLive.MainWindows;
using System;
using System.Windows;
using System.Windows.Threading;

namespace MVGLive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow2 : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public string BorderColor { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string ClockBackgroundColor { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string ClockForegroundColor { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string CaptionForegroundColor { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string CaptionBackgroundColor { get; private set; }

        /// ************************************************************************************************
        /// <summary>
        ///
        /// </summary>
        public MainWindow2()
        {
            DataContext = this;
            BorderColor = Properties.Settings.Default.BorderColor;
            ClockBackgroundColor = Properties.Settings.Default.ClockBackgroundColor;
            ClockForegroundColor = Properties.Settings.Default.ClockForegroundColor;
            CaptionForegroundColor = Properties.Settings.Default.CaptionForegroundColor;
            CaptionBackgroundColor = Properties.Settings.Default.CaptionBackgroundColor;

            InitializeComponent();
            this.PreviewKeyUp += MainWindow2_PreviewKeyUp;
            MainCommon.SetupTables(new System.Windows.Controls.Label[] { Text1, Text2 }, new MVGTimeTable.MVGTimeTable[] { Table1, Table2 });
            MainCommon.SetupTimeLabel(LabelTime);
            // Clock Refresh Timer
            DispatcherTimer timerClock = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timerClock.Tick += TimerClock_Tick;
            timerClock.Start();

        }

        private void MainWindow2_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                App.Current.Shutdown();
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Clock Timer Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerClock_Tick(object sender, EventArgs e)
        {
            MainCommon.UpdateClockLabel(LabelTime);
        }

        /// ************************************************************************************************
        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MVGLive.MainWindows;
using System;
using System.Windows;
using System.Windows.Threading;

namespace MVGLive
{
    /// <summary>
    /// Interaction logic for MainWindow1.xaml
    /// </summary>
    public partial class MainWindow1 : Window
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
        public MainWindow1()
        {
            DataContext = this;
            BorderColor = Properties.Settings.Default.BorderColor;
            ClockBackgroundColor = Properties.Settings.Default.ClockBackgroundColor;
            ClockForegroundColor = Properties.Settings.Default.ClockForegroundColor;
            CaptionForegroundColor = Properties.Settings.Default.CaptionForegroundColor;
            CaptionBackgroundColor = Properties.Settings.Default.CaptionBackgroundColor;

            InitializeComponent();
            this.PreviewKeyUp += MainWindow1_PreviewKeyUp;

            MainCommon.SetupTables(new System.Windows.Controls.Label[] { Text1 }, new MVGTimeTable.MVGTimeTable[] { Table1 });
            MainCommon.SetupTimeLabel(LabelTime);
            DispatcherTimer timerClock = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timerClock.Tick += TimerClock_Tick;
            timerClock.Start();
        }

        private void MainWindow1_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
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
// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MVGLive.MainWindows;
using MVGTimeTable;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace MVGLive
{
    /// <summary>
    /// Interaction logic for MainWindow4.xaml
    /// </summary>
    public partial class MainWindow4 : Window
    {

        /// <summary>
        ///
        /// </summary>
        public MainWindow4(MVGTimeTableSettings[] settings)
        {

            if (settings != null && settings.Length >= 4)
            {
                InitializeComponent();

                MainCommon.SetupTables(new System.Windows.Controls.Label[] { Text1, Text2, Text3, Text4 },
                                        new MVGTimeTable.MVGTimeTable[] { Table1, Table2, Table3, Table4 },
                                        settings);

                MainCommon.SetupTimeLabel(LabelTime, settings[0].TableFontFamily, settings[0].TableFontSize);
                // Clock Refresh Timer
                DispatcherTimer timerClock = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                timerClock.Tick += TimerClock_Tick;
                timerClock.Start();
            }
            else
            {
                Debug.Assert(false, "Wrong MainWindow1 constructor's parameter");
            }
        }

        /// <summary>
        /// Clock Timer Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerClock_Tick(object sender, EventArgs e)
        {
            MainCommon.UpdateClockLabel(LabelTime);
        }
    }
}
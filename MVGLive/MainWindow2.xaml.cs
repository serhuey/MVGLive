using System;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;

namespace MVGLive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow2 : Window
    {
        private string[] Directions { get; set; }
        private double UserFontSize { get; set; }

        public MainWindow2()
        {
            InitializeComponent();

            UserFontSize = App.UserFontSize;

            Directions = new string[] { App.DefaultDirection1, App.DefaultDirection2, App.DefaultDirection3 };

            int i = 0;
            foreach (string s in App.Arguments)
            {
                if (i >= Directions.Length) break;
                Directions[i++] = s;
            }

            DispatcherTimer timerClock;
            Thickness labelMargin = new Thickness(UserFontSize, UserFontSize * 0.8f, 0, UserFontSize);

            LabelTime.FontFamily = App.FontFamily;
            LabelTime.FontSize = UserFontSize;
            LabelTime.Margin = new Thickness(UserFontSize * 0.5f);

            Table1.SetProperties(App.FontFamily, Directions[0], 15, 2, (int)UserFontSize);
            Table1.Start();

            Table2.SetProperties(App.FontFamily, Directions[1], 15, 7, (int)UserFontSize);
            Table2.Start();

            Table1.ConnectionStatusChangedEvent += Table1_ConnectionStatusChangedEvent;
            Table2.ConnectionStatusChangedEvent += Table2_ConnectionStatusChangedEvent;

            Text1.FontFamily = App.FontFamily;
            Text1.FontSize = UserFontSize * 1.3f;
            Text1.Tag = Directions[0];
            Text1.Margin = labelMargin;

            Text2.FontFamily = App.FontFamily;
            Text2.FontSize = UserFontSize * 1.3f;
            Text2.Tag = Directions[1];
            Text2.Margin = labelMargin;

            // Clock Refresh Timer
            timerClock = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timerClock.Tick += TimerClock_Tick;
            timerClock.Start();
        }

        private void Table1_ConnectionStatusChangedEvent(object sender, EventArgs e)
        {
        }

        private void Table2_ConnectionStatusChangedEvent(object sender, EventArgs e)
        {
        }


        /// <summary>
        /// Clock Timer Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimerClock_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            string time = now.Hour.ToString("D2", CultureInfo.InvariantCulture) + ":" + now.Minute.ToString("D2", CultureInfo.InvariantCulture) + ":" + now.Second.ToString("D2", CultureInfo.InvariantCulture);
            LabelTime.Content = time;
        }

    }
}

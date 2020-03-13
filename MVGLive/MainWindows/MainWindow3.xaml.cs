using MVGTimeTable.ViewModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;

namespace MVGLive
{
    /// <summary>
    /// Interaction logic for MainWindow3.xaml
    /// </summary>
    public partial class MainWindow3 : Window
    {
        private string[] Directions { get; set; }
        private double UserFontSize { get; set; }

        /// <summary>
        ///
        /// </summary>
        public MainWindow3()
        {
            InitializeComponent();

            UserFontSize = App.TableFontSize;

            Directions = new string[] { App.DefaultDirection1, App.DefaultDirection2, App.DefaultDirection3 };

            int i = 0;
            foreach (string s in App.Arguments)
            {
                if (i >= Directions.Length) break;
                Directions[i++] = s;
            }

            DispatcherTimer timerClock;
            Thickness labelMargin = new Thickness(UserFontSize, UserFontSize * 0.8f, 0, UserFontSize);

            LabelTime.FontFamily = App.TableFontFamily;
            LabelTime.FontSize = UserFontSize;
            LabelTime.Margin = new Thickness(UserFontSize * 0.5f);

            Table1.DataContext = new DeparturesViewModel(Directions[0], App.TableFontFamily, (int)UserFontSize, 15, 2);
            Table2.DataContext = new DeparturesViewModel(Directions[1], App.TableFontFamily, (int)UserFontSize, 15, 7);
            Table3.DataContext = new DeparturesViewModel(Directions[2], App.TableFontFamily, (int)UserFontSize, 15, 12);

            Text1.FontFamily = App.TableFontFamily;
            Text1.FontSize = UserFontSize * 1.3f;
            Text1.Tag = Directions[0];
            Text1.Margin = labelMargin;

            Text2.FontFamily = App.TableFontFamily;
            Text2.FontSize = UserFontSize * 1.3f;
            Text2.Tag = Directions[1];
            Text2.Margin = labelMargin;

            Text3.FontFamily = App.TableFontFamily;
            Text3.FontSize = UserFontSize * 1.3f;
            Text3.Tag = Directions[2];
            Text3.Margin = labelMargin;

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

        private void Table3_ConnectionStatusChangedEvent(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Clock Timer Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerClock_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            string time = now.Hour.ToString("D2", CultureInfo.InvariantCulture) + ":" + now.Minute.ToString("D2", CultureInfo.InvariantCulture) + ":" + now.Second.ToString("D2", CultureInfo.InvariantCulture);
            LabelTime.Content = time;
        }
    }
}
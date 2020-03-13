using MVGTimeTable.ViewModel;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;

namespace MVGLive
{
    /// <summary>
    /// Interaction logic for MainWindow4.xaml
    /// </summary>
    public partial class MainWindow4 : Window
    {
        private string[] Directions { get; set; }
        private double TableFontSize { get; set; }
        private double HeaderFontSize { get; set; }

        /// <summary>
        ///
        /// </summary>
        public MainWindow4()
        {
            InitializeComponent();

            TableFontSize = App.TableFontSize;
            HeaderFontSize = App.HeaderFontSize;

            Directions = new string[] { App.DefaultDirection1, App.DefaultDirection2, App.DefaultDirection3, App.DefaultDirection4 };

            int i = 0;
            foreach (string s in App.Arguments)
            {
                if (i >= Directions.Length) break;
                Directions[i++] = s;
            }

            DispatcherTimer timerClock;
            Thickness labelMargin = new Thickness(TableFontSize * 0.5f, TableFontSize * 0.1f, 0, 0);

            LabelTime.FontFamily = App.TableFontFamily;
            LabelTime.FontSize = TableFontSize;
            LabelTime.Margin = new Thickness(TableFontSize * 0.5f);

            Table1.DataContext = new DeparturesViewModel(Directions[0], App.TableFontFamily, (int)TableFontSize, 16, 2, App.HeaderFontFamily, (int)HeaderFontSize);
            Table2.DataContext = new DeparturesViewModel(Directions[1], App.TableFontFamily, (int)TableFontSize, 16, 6, App.HeaderFontFamily, (int)HeaderFontSize);
            Table3.DataContext = new DeparturesViewModel(Directions[2], App.TableFontFamily, (int)TableFontSize, 16, 10, App.HeaderFontFamily, (int)HeaderFontSize);
            Table4.DataContext = new DeparturesViewModel(Directions[3], App.TableFontFamily, (int)TableFontSize, 16, 14, App.HeaderFontFamily, (int)HeaderFontSize);

            Text1.FontFamily = App.TableFontFamily;
            Text1.FontSize = TableFontSize * 1.3f;
            Text1.Tag = Directions[0];
            Text1.Margin = labelMargin;

            Text2.FontFamily = App.TableFontFamily;
            Text2.FontSize = TableFontSize * 1.3f;
            Text2.Tag = Directions[1];
            Text2.Margin = labelMargin;

            Text3.FontFamily = App.TableFontFamily;
            Text3.FontSize = TableFontSize * 1.3f;
            Text3.Tag = Directions[2];
            Text3.Margin = labelMargin;

            Text4.FontFamily = App.TableFontFamily;
            Text4.FontSize = TableFontSize * 1.3f;
            Text4.Tag = Directions[3];
            Text4.Margin = labelMargin;

            // Clock Refresh Timer
            timerClock = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timerClock.Tick += TimerClock_Tick;
            timerClock.Start();
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
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace MVGLive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List<string> arguments = new List<string>();
            string[] args = Environment.GetCommandLineArgs();

            for (int index = 1; index < args.Length; ++index)
            {
                string arg = args[index].Replace("-", "");
                arguments.Add(arg);
            }

            // ToDo: Move all this parameters from code to the settings form or settings xml-file
            double fontSize = 30;
            string fontFamily = "PT Sans";
            string defaultDirection1 = "Hirschgarten";
            string defaultDirection2 = "Briefzentrum";
            string defaultDirection3 = "Steubenplatz";

            string[] directions = { defaultDirection1, defaultDirection2, defaultDirection3 };
            int i = 0;
            foreach (string s in arguments)
            {
                if (i >= directions.Length) break;
                directions[i++] = s;
            }

            DispatcherTimer timerClock;
            Thickness labelMargin = new Thickness(fontSize, fontSize * 0.8f, 0, fontSize);

            LabelTime.FontFamily = new FontFamily(fontFamily);
            LabelTime.FontSize = fontSize;
            LabelTime.Margin = new Thickness(fontSize * 0.5f);

            Table1.SetProperties(directions[0], 15, 2, (int)fontSize, fontFamily);
            Table1.Start();

            Table2.SetProperties(directions[1], 15, 7, (int)fontSize, fontFamily);
            Table2.Start();

            Table3.SetProperties(directions[2], 15, 12, (int)fontSize, fontFamily);
            Table3.Start();


            Text1.FontFamily = new FontFamily(fontFamily);
            Text1.FontSize = fontSize * 1.3f;
            Text1.Tag = directions[0];
            Text1.Margin = labelMargin;

            Text2.FontFamily = new FontFamily(fontFamily);
            Text2.FontSize = fontSize * 1.3f;
            Text2.Tag = directions[1];
            Text2.Margin = labelMargin;

            Text3.FontFamily = new FontFamily(fontFamily);
            Text3.FontSize = fontSize * 1.3f;
            Text3.Tag = directions[2];
            Text3.Margin = labelMargin;

            // Clock Refresh Timer
            timerClock = new DispatcherTimer();
            timerClock.Interval = TimeSpan.FromSeconds(1);
            timerClock.Tick += timerClock_Tick;
            timerClock.Start();
        }

        /// <summary>
        /// Clock Timer Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timerClock_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            string time = now.Hour.ToString("D2") + ":" + now.Minute.ToString("D2") + ":" + now.Second.ToString("D2");
            LabelTime.Content = time;
        }
    }
}

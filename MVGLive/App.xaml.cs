using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MVGLive
{
    public partial class App : Application
    {
        public static int UserFontSize { get; set; }
        public static FontFamily FontFamily { get; set; }
        public static string DefaultDirection1 { get; set; } = "Hirschgarten";
        public static string DefaultDirection2 { get; set; } = "Briefzentrum";
        public static string DefaultDirection3 { get; set; } = "Steubenplatz";
        public static string DefaultDirection4 { get; set; } = "Hauptbahnhof";
        public static List<string> Arguments { get; } = new List<string>();

        private static bool IsScreenSaverEnabled { get; } = ScreenSaver.GetScreenSaverActive();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Arguments.AddRange(GetCommandLineArguments());
            FontFamily = LoadEmbeddedFont("PT Sans");
            UserFontSize = 38;

            DisableScreenSaver(IsScreenSaverEnabled);

            Window mainWindow = new MainWindow4();
            mainWindow.Show();
        }

        /// ************************************************************************************************
        /// <summary>
        /// Load embedded fonts and out the font with fontFamilyName
        /// </summary>
        /// <param name="fontFamilyName">Desired FontFamily name</param>
        /// <returns>Desired FontFamily if exists, Default FontFamily if not</returns>
        /// ************************************************************************************************
        private FontFamily LoadEmbeddedFont(string fontFamilyName)
        {
            Dictionary<string, FontFamily> fontFamilies = new Dictionary<string, FontFamily>();
            foreach (FontFamily family in Fonts.GetFontFamilies(new Uri("pack://application:,,,/"), "./Fonts/"))
            {
                fontFamilies.Add(family.FamilyNames[System.Windows.Markup.XmlLanguage.GetLanguage("en-US")], family);
            }
            return fontFamilies.ContainsKey(fontFamilyName) ? fontFamilies[fontFamilyName] : new FontFamily("Segoe UI");

        }

        /// ************************************************************************************************
        /// <summary>
        /// Get List with parsed command line arguments
        /// </summary>
        /// <returns></returns>
        /// ************************************************************************************************
        private List<string> GetCommandLineArguments()
        {
            string[] args = Environment.GetCommandLineArgs();

            List<string> outputList = new List<string>();

            for (int index = 1; index < args.Length; ++index)
            {
                string arg = args[index].Replace("-", "");
                outputList.Add(arg);
            }
            return outputList;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            EnableScreenSaver(IsScreenSaverEnabled);
        }

        /// ************************************************************************************************
        /// <summary>
        /// Disable screensaver and sleep
        /// </summary>
        /// <param name="isScreenSaverEnabled">System screensaver status</param>
        /// ************************************************************************************************

        private void DisableScreenSaver(bool isScreenSaverEnabled)
        {
            if (isScreenSaverEnabled)
            {
                ScreenSaver.SetScreenSaverActive(false);
            }
            ScreenSaver.DisableSleep();
        }

        /// ************************************************************************************************
        /// <summary>
        /// Enable screensaver and sleep
        /// </summary>
        /// <param name="isScreenSaverEnabled">System screensaver status</param>
        /// ************************************************************************************************

        private void EnableScreenSaver(bool isScreenSaverEnabled)
        {
            // if screen saver was enabled before app was started turn it on back
            if (isScreenSaverEnabled)
            {
                ScreenSaver.SetScreenSaverActive(true);
            }
            ScreenSaver.EnableSleep();
        }
    }

}

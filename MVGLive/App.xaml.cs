using MVGTimeTable;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using MVGLive.Properties;

namespace MVGLive
{
    public partial class App : Application
    {
        private static int tableType;

        /// <summary>
        /// 
        /// </summary>
        private static bool IsScreenSaverEnabled { get; } = ScreenSaver.GetScreenSaverActive();

        /// <summary>
        /// 
        /// </summary>
        public static List<string> Arguments { get; } = GetCommandLineArguments();

        /// <summary>
        /// 
        /// </summary>
        private static readonly MVGTimeTableSettings[] timeTableSettings = new MVGTimeTableSettings[4];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            LoadSettings();
            DisableScreenSaver(IsScreenSaverEnabled);

            Window mainWindow;
            switch (tableType)
            {
                case (int)MainWindowType.OneDestination: mainWindow = new MainWindow1(timeTableSettings); break;
                case (int)MainWindowType.TwoDestinationsVertical: mainWindow = new MainWindow2(timeTableSettings); break;
                case (int)MainWindowType.TwoDestinationsHorizontal: mainWindow = new MainWindow2a(timeTableSettings); break;
                case (int)MainWindowType.ThreeDestinations: mainWindow = new MainWindow3(timeTableSettings); break;
                case (int)MainWindowType.FourDestinations: mainWindow = new MainWindow4(timeTableSettings); break;
                default: mainWindow = new MainWindow1(timeTableSettings); break;
            }
            mainWindow.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadSettings()
        {
            string[] destinations = new string[] {  Settings.Default.Destination_1,
                                                    Settings.Default.Destination_2,
                                                    Settings.Default.Destination_3,
                                                    Settings.Default.Destination_4 };

            for (int i = 0; i < timeTableSettings.Length && i < destinations.Length; ++i)
            {
                timeTableSettings[i] = new MVGTimeTableSettings(stationName: destinations[i],
                                                         tableBackgroundColor1: Settings.Default.TableBackgroundColor1,
                                                         tableBackgroundColor2: Settings.Default.TableBackgroundColor2,
                                                         tableForegroundColor1: Settings.Default.TableForegroundColor1,
                                                         tableForegroundColor2: Settings.Default.TableForegroundColor2,
                                                         headerBackgroundColor: Settings.Default.HeaderBackgroundColor,
                                                         headerForegroundColor: Settings.Default.HeaderForegroundColor,
                                                         noConnectionForegroundColor: Settings.Default.NoConnectionForegroundColor,
                                                         warningForegroundColor: Settings.Default.WarningForegroundColor,
                                                         tableFontSize: Settings.Default.TableFontSize,
                                                         headerFontSize: Settings.Default.HeaderFontSize,
                                                         tableFontFamily: LoadEmbeddedFont("PT Sans"),
                                                         headerFontFamily: LoadEmbeddedFont("PT Sans"));
            }
            SetTableType((MainWindowType)Enum.ToObject(typeof(MainWindowType), Settings.Default.TableType));
        }

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// ************************************************************************************************
        private static void SetTableType(MainWindowType value)
        {
            tableType = Enum.IsDefined(typeof(MainWindowType), value) ? (int)value : (Enum.GetValues(typeof(MainWindowType)) as int[])[0];
        }

        /// ************************************************************************************************
        /// <summary>
        /// Load embedded fonts and out the font with fontFamilyName
        /// </summary>
        /// <param name="fontFamilyName">Desired FontFamily name</param>
        /// <returns>Desired FontFamily if exists, Default FontFamily if not</returns>
        /// ************************************************************************************************
        private static FontFamily LoadEmbeddedFont(string fontFamilyName)
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
        private static List<string> GetCommandLineArguments()
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
// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using MVGLive.Properties;
using System.Drawing.Text;
using System.Linq;

namespace MVGLive
{
    public partial class App : Application
    {
        /// <summary>
        /// Initial screen saver state
        /// </summary>
        private static bool IsScreenSaverEnabled { get; } = ScreenSaver.GetScreenSaverActive();

        /// <summary>
        /// Command line arguments
        /// </summary>
        public static List<string> Arguments { get; } = GetCommandLineArguments();

        /// <summary>
        /// Font library with all system and embedded fonts
        /// </summary>
        public static Dictionary<string, FontFamily> FontLibrary { get; private set; } = new Dictionary<string, FontFamily>();

        private Window mainWindow;
        private Window settingsWindow;

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool noSettings = Arguments.Contains("ns");

            FillFontLibrary();

            if (!noSettings)
            {
                settingsWindow = new SettingsWindow
                {
                    DataContext = new SettingsViewModel(FontLibrary.Keys.ToList())
                };
                settingsWindow.ShowDialog();
            }

            if (noSettings || settingsWindow.DialogResult == true)
            {
                Settings.Default.Save();
                switch (Settings.Default.TableType)
                {
                    case (int)MainWindowType.OneDestination: mainWindow = new MainWindow1(); break;
                    case (int)MainWindowType.TwoDestinationsVertical: mainWindow = new MainWindow2(); break;
                    case (int)MainWindowType.TwoDestinationsHorizontal: mainWindow = new MainWindow2a(); break;
                    case (int)MainWindowType.ThreeDestinations: mainWindow = new MainWindow3(); break;
                    case (int)MainWindowType.FourDestinations: mainWindow = new MainWindow4(); break;
                    default: mainWindow = new MainWindow1(); break;
                }

                DisableScreenSaver(IsScreenSaverEnabled);
                this.MainWindow = mainWindow;
                mainWindow.Show();
            }
            else
            {
                Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                Current.Shutdown();
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Fill FontLibrary with all system and all embedded fonts
        /// </summary>
        private static void FillFontLibrary()
        {
            List<string> fontFamiliesNames = new List<string>();
            using (InstalledFontCollection installedFontCollection = new InstalledFontCollection())
            {
                foreach (FontFamily family in Fonts.GetFontFamilies(new Uri("pack://application:,,,/"), "./Fonts/"))
                {
                    string fontFamilyName = family.FamilyNames[System.Windows.Markup.XmlLanguage.GetLanguage("en-US")];
                    if (!FontLibrary.ContainsKey(fontFamilyName))
                    {
                        FontLibrary.Add(fontFamilyName, family);
                    }
                }

                foreach (System.Drawing.FontFamily drawingFontFamily in installedFontCollection.Families)
                {
                    fontFamiliesNames.Add(drawingFontFamily.Name);
                }
                foreach (string fontFamilyName in fontFamiliesNames)
                {
                    if (!FontLibrary.ContainsKey(fontFamilyName))
                    {
                        FontLibrary.Add(fontFamilyName, new FontFamily(fontFamilyName));
                    }
                }
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// Load embedded fonts and out the font with fontFamilyName
        /// </summary>
        /// <param name="fontFamilyName">Desired FontFamily name</param>
        /// <returns>Desired FontFamily if exists, Default FontFamily if not</returns>
        public static FontFamily GetFontFromLibrary(string fontFamilyName)
        {
            return FontLibrary.ContainsKey(fontFamilyName) ? FontLibrary[fontFamilyName] : new FontFamily("Segoe UI");
        }

        /// ************************************************************************************************
        /// <summary>
        /// Get List with parsed command line arguments
        /// </summary>
        /// <returns></returns>
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

        /// ************************************************************************************************
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            EnableScreenSaver(IsScreenSaverEnabled);
        }

        /// ************************************************************************************************
        /// <summary>
        /// Disable screensaver and sleep
        /// </summary>
        /// <param name="isScreenSaverEnabled">System screensaver status</param>
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
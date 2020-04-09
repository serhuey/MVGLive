// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace MVGLive
{
    /// <summary>
    /// 
    /// </summary>
    public class SettingsViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly Dictionary<string, string> ColorPickerNames = new Dictionary<string, string> { { "ClockBackgroundColor", "Hintergrundfarbe der Uhr" },
                                                                                                            { "ClockForegroundColor", "Vordergrundfarbe der Uhr" },

                                                                                                            { "CaptionBackgroundColor", "Hintergrundfarbe der HSt" },
                                                                                                            { "CaptionForegroundColor", "Vordergrundfarbe der HSt" },

                                                                                                            { "HeaderBackgroundColor", "Hintergrundfarbe der Kopfzeile" },
                                                                                                            { "HeaderForegroundColor", "Vordergrundfarbe der Kopfzeile" },

                                                                                                            { "TableBackgroundColor1", "1. Hintergrundfarbe der Tabelle" },
                                                                                                            { "TableBackgroundColor2", "2. Hintergrundfarbe der Tabelle" },

                                                                                                            { "TableForegroundColor1", "1. Vordergrundfarbe der Tabelle" },
                                                                                                            { "TableForegroundColor2", "2. Vordergrundfarbe der Tabelle" },
                                                                                                            { "TableForegroundColor3", "3. Vordergrundfarbe der Tabelle" },
                                                                                                            { "WarningForegroundColor", "Vordergrundfarbe der Warnung" },
                                                                                                            { "NoConnectionForegroundColor", "Vordergrundfarbe des Fehlers" },

                                                                                                            { "BorderColor", "Farbe des Rahmens" } };
        private MainWindowType _mainWindowTypeSetting;
        private string _tableBackgroundColor2Setting;
        private string _headerBackgroundColorSetting;
        private string _tableBackgroundColor1Setting;
        private string _tableForegroundColor1Setting;
        private string _tableForegroundColor2Setting;
        private string _tableForegroundColor3Setting;
        private string _captionForegroundColorSetting;
        private string _captionBackgroundColorSetting;
        private string _warningForegroundColorSetting;
        private string _noConnectionForegroundColorSetting;
        private string _clockForegroundColorSetting;
        private string _clockBackgroundColorSetting;
        private string _headerForegroundColorSetting;
        private string _borderColorSetting;
        private string _station1Setting;
        private string _station2Setting;
        private string _station3Setting;
        private string _station4Setting;
        private string _clockFontSizeSetting;
        private string _tableFontSizeSetting;
        private string _headerFontSizeSetting;
        private string _captionFontSizeSetting;
        private string _tableFontFamilySetting;
        private string _headerFontFamilySetting;
        private string _captionFontFamilySetting;
        private string _clockFontFamilySetting;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public MainWindowType MainWindowTypeSetting
        {
            get => _mainWindowTypeSetting;
            set
            {
                if (value != _mainWindowTypeSetting)
                {
                    _mainWindowTypeSetting = value;
                    OnPropertyChange(nameof(MainWindowTypeSetting));
                }
            }
        }

        public string HeaderBackgroundColorSetting
        {
            get => _headerBackgroundColorSetting;
            set => SetStringProperty(newValue: value, backingField: ref _headerBackgroundColorSetting, propertyName: nameof(HeaderBackgroundColorSetting));
        }

        public string TableBackgroundColor1Setting
        {
            get => _tableBackgroundColor1Setting;
            set => SetStringProperty(newValue: value, backingField: ref _tableBackgroundColor1Setting, propertyName: nameof(TableBackgroundColor1Setting));
        }

        public string TableBackgroundColor2Setting
        {
            get => _tableBackgroundColor2Setting;
            set => SetStringProperty(newValue: value, backingField: ref _tableBackgroundColor2Setting, propertyName: nameof(TableBackgroundColor2Setting));
        }

        public string TableForegroundColor1Setting
        {
            get => _tableForegroundColor1Setting;
            set => SetStringProperty(newValue: value, backingField: ref _tableForegroundColor1Setting, propertyName: nameof(TableForegroundColor1Setting));
        }

        public string TableForegroundColor2Setting
        {
            get => _tableForegroundColor2Setting;
            set => SetStringProperty(newValue: value, backingField: ref _tableForegroundColor2Setting, propertyName: nameof(TableForegroundColor2Setting));
        }

        public string TableForegroundColor3Setting
        {
            get => _tableForegroundColor3Setting;
            set => SetStringProperty(newValue: value, backingField: ref _tableForegroundColor3Setting, propertyName: nameof(TableForegroundColor3Setting));
        }

        public string CaptionForegroundColorSetting
        {
            get => _captionForegroundColorSetting;
            set => SetStringProperty(newValue: value, backingField: ref _captionForegroundColorSetting, propertyName: nameof(CaptionForegroundColorSetting));
        }

        public string CaptionBackgroundColorSetting
        {
            get => _captionBackgroundColorSetting;
            set => SetStringProperty(newValue: value, backingField: ref _captionBackgroundColorSetting, propertyName: nameof(CaptionBackgroundColorSetting));
        }

        public string WarningForegroundColorSetting
        {
            get => _warningForegroundColorSetting;
            set => SetStringProperty(newValue: value, backingField: ref _warningForegroundColorSetting, propertyName: nameof(WarningForegroundColorSetting));
        }

        public string NoConnectionForegroundColorSetting
        {
            get => _noConnectionForegroundColorSetting;
            set => SetStringProperty(newValue: value, backingField: ref _noConnectionForegroundColorSetting, propertyName: nameof(NoConnectionForegroundColorSetting));
        }

        public string ClockForegroundColorSetting
        {
            get => _clockForegroundColorSetting;
            set => SetStringProperty(newValue: value, backingField: ref _clockForegroundColorSetting, propertyName: nameof(ClockForegroundColorSetting));
        }

        public string ClockBackgroundColorSetting
        {
            get => _clockBackgroundColorSetting;
            set => SetStringProperty(newValue: value, backingField: ref _clockBackgroundColorSetting, propertyName: nameof(ClockBackgroundColorSetting));
        }

        public string HeaderForegroundColorSetting
        {
            get => _headerForegroundColorSetting;
            set => SetStringProperty(newValue: value, backingField: ref _headerForegroundColorSetting, propertyName: nameof(HeaderForegroundColorSetting));
        }

        public string BorderColorSetting
        {
            get => _borderColorSetting;
            set => SetStringProperty(newValue: value, backingField: ref _borderColorSetting, propertyName: nameof(BorderColorSetting));
        }

        public string Station1Setting
        {
            get => _station1Setting;
            set => SetStringProperty(newValue: value, backingField: ref _station1Setting, propertyName: nameof(Station1Setting));
        }

        public string Station2Setting
        {
            get => _station2Setting;
            set => SetStringProperty(newValue: value, backingField: ref _station2Setting, propertyName: nameof(Station2Setting));
        }

        public string Station3Setting
        {
            get => _station3Setting;
            set => SetStringProperty(newValue: value, backingField: ref _station3Setting, propertyName: nameof(Station3Setting));
        }

        public string Station4Setting
        {
            get => _station4Setting;
            set => SetStringProperty(newValue: value, backingField: ref _station4Setting, propertyName: nameof(Station4Setting));
        }

        public string TableFontSizeSetting
        {
            get => _tableFontSizeSetting;
            set => SetStringProperty(newValue: value, backingField: ref _tableFontSizeSetting, propertyName: nameof(TableFontSizeSetting));
        }

        public string HeaderFontSizeSetting
        {
            get => _headerFontSizeSetting;
            set => SetStringProperty(newValue: value, backingField: ref _headerFontSizeSetting, propertyName: nameof(HeaderFontSizeSetting));
        }

        public string CaptionFontSizeSetting
        {
            get => _captionFontSizeSetting;
            set => SetStringProperty(newValue: value, backingField: ref _captionFontSizeSetting, propertyName: nameof(CaptionFontSizeSetting));
        }

        public string ClockFontSizeSetting
        {
            get => _clockFontSizeSetting;
            set => SetStringProperty(newValue: value, backingField: ref _clockFontSizeSetting, propertyName: nameof(ClockFontSizeSetting));
        }

        public string TableFontFamilySetting
        {
            get => _tableFontFamilySetting;
            set => SetStringProperty(newValue: value, backingField: ref _tableFontFamilySetting, propertyName: nameof(TableFontFamilySetting));
        }

        public string HeaderFontFamilySetting
        {
            get => _headerFontFamilySetting;
            set => SetStringProperty(newValue: value, backingField: ref _headerFontFamilySetting, propertyName: nameof(HeaderFontFamilySetting));
        }

        public string CaptionFontFamilySetting
        {
            get => _captionFontFamilySetting;
            set => SetStringProperty(newValue: value, backingField: ref _captionFontFamilySetting, propertyName: nameof(CaptionFontFamilySetting));
        }

        public string ClockFontFamilySetting
        {
            get => _clockFontFamilySetting;
            set => SetStringProperty(newValue: value, backingField: ref _clockFontFamilySetting, propertyName: nameof(ClockFontFamilySetting));
        }

        public StationsListProvider StationsList { get; private set; }

        public List<string> FontLibrary { get; private set; }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="backingField"></param>
        /// <param name="propertyName"></param>
        private void SetStringProperty(string newValue, ref string backingField, string propertyName)
        {
            if (!string.IsNullOrEmpty(newValue))
            {
                if (string.Compare(newValue, backingField, true, culture: CultureInfo.InvariantCulture) != 0)
                {
                    backingField = newValue;
                    OnPropertyChange(propertyName);
                }
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        public SettingsViewModel(List<string> fontLibrary)
        {
            FontLibrary = fontLibrary;
            LoadProperties();
            ColorPickCommand = new SettingsCommand(ExecuteColorPickMethod, CanExecuteColorPickMethod);
            OkCommand = new SettingsCommand(ExecuteOkMethod, CanExecuteOkMethod);
            DefaultCommand = new SettingsCommand(ExecuteDefaultMethod, CanExecuteDefaultMethod);
            FontSizeChangedCommand = new SettingsCommand(ExecuteFontSizeChangedMethod, CanExecuteFontSizeChangedMethod);
            FontSizePreviewTextInputCommand = new SettingsCommand(ExecuteFontSizePreviewTextMethod, CanExecuteFontSizePreviewTextMethod);
            StationsList = new StationsListProvider(MVGAPI.MVGAPI.StationsList);
        }

        /// ************************************************************************************************
        private void LoadProperties(bool callFromRestoreDefault = false)
        {
            if (!callFromRestoreDefault)
            {
                int type = Properties.Settings.Default.TableType;
                MainWindowTypeSetting = Enum.IsDefined(typeof(MainWindowType), type) ? (MainWindowType)Enum.ToObject(typeof(MainWindowType), type) : MainWindowType.OneDestination;

                Station1Setting = Properties.Settings.Default.Station1;
                Station2Setting = Properties.Settings.Default.Station2;
                Station3Setting = Properties.Settings.Default.Station3;
                Station4Setting = Properties.Settings.Default.Station4;
            }

            HeaderBackgroundColorSetting = Properties.Settings.Default.HeaderBackgroundColor;
            TableBackgroundColor1Setting = Properties.Settings.Default.TableBackgroundColor1;
            TableBackgroundColor2Setting = Properties.Settings.Default.TableBackgroundColor2;
            TableForegroundColor1Setting = Properties.Settings.Default.TableForegroundColor1;
            TableForegroundColor2Setting = Properties.Settings.Default.TableForegroundColor2;
            TableForegroundColor3Setting = Properties.Settings.Default.TableForegroundColor3;
            CaptionForegroundColorSetting = Properties.Settings.Default.CaptionForegroundColor;
            CaptionBackgroundColorSetting = Properties.Settings.Default.CaptionBackgroundColor;
            WarningForegroundColorSetting = Properties.Settings.Default.WarningForegroundColor;
            NoConnectionForegroundColorSetting = Properties.Settings.Default.NoConnectionForegroundColor;
            ClockForegroundColorSetting = Properties.Settings.Default.ClockForegroundColor;
            ClockBackgroundColorSetting = Properties.Settings.Default.ClockBackgroundColor;
            HeaderForegroundColorSetting = Properties.Settings.Default.HeaderForegroundColor;
            BorderColorSetting = Properties.Settings.Default.BorderColor;

            TableFontSizeSetting = Properties.Settings.Default.TableFontSize.ToString(CultureInfo.InvariantCulture);
            ClockFontSizeSetting = Properties.Settings.Default.ClockFontSize.ToString(CultureInfo.InvariantCulture);
            CaptionFontSizeSetting = Properties.Settings.Default.CaptionFontSize.ToString(CultureInfo.InvariantCulture);
            HeaderFontSizeSetting = Properties.Settings.Default.HeaderFontSize.ToString(CultureInfo.InvariantCulture);

            TableFontFamilySetting = Properties.Settings.Default.TableFontFamily;
            ClockFontFamilySetting = Properties.Settings.Default.ClockFontFamily;
            CaptionFontFamilySetting = Properties.Settings.Default.CaptionFontFamily;
            HeaderFontFamilySetting = Properties.Settings.Default.HeaderFontFamily;
        }

        /// ************************************************************************************************
        private void SaveProperties()
        {
            Properties.Settings.Default.TableType = (int)MainWindowTypeSetting;
            Properties.Settings.Default.HeaderBackgroundColor = HeaderBackgroundColorSetting;
            Properties.Settings.Default.TableBackgroundColor1 = TableBackgroundColor1Setting;
            Properties.Settings.Default.TableBackgroundColor2 = TableBackgroundColor2Setting;
            Properties.Settings.Default.TableForegroundColor1 = TableForegroundColor1Setting;
            Properties.Settings.Default.TableForegroundColor2 = TableForegroundColor2Setting;
            Properties.Settings.Default.TableForegroundColor3 = TableForegroundColor3Setting;
            Properties.Settings.Default.CaptionForegroundColor = CaptionForegroundColorSetting;
            Properties.Settings.Default.CaptionBackgroundColor = CaptionBackgroundColorSetting;
            Properties.Settings.Default.WarningForegroundColor = WarningForegroundColorSetting;
            Properties.Settings.Default.NoConnectionForegroundColor = NoConnectionForegroundColorSetting;
            Properties.Settings.Default.ClockForegroundColor = ClockForegroundColorSetting;
            Properties.Settings.Default.ClockBackgroundColor = ClockBackgroundColorSetting;
            Properties.Settings.Default.HeaderForegroundColor = HeaderForegroundColorSetting;
            Properties.Settings.Default.BorderColor = BorderColorSetting;

            Properties.Settings.Default.Station1 = Station1Setting;
            Properties.Settings.Default.Station2 = Station2Setting;
            Properties.Settings.Default.Station3 = Station3Setting;
            Properties.Settings.Default.Station4 = Station4Setting;

            Properties.Settings.Default.TableFontSize = int.Parse(TableFontSizeSetting, CultureInfo.InvariantCulture) >= Properties.Settings.Default.FontSizeMin ?
                int.Parse(TableFontSizeSetting, CultureInfo.InvariantCulture) : Properties.Settings.Default.FontSizeMin;
            Properties.Settings.Default.ClockFontSize = int.Parse(ClockFontSizeSetting, CultureInfo.InvariantCulture) >= Properties.Settings.Default.FontSizeMin ?
                int.Parse(ClockFontSizeSetting, CultureInfo.InvariantCulture) : Properties.Settings.Default.FontSizeMin;
            Properties.Settings.Default.CaptionFontSize = int.Parse(CaptionFontSizeSetting, CultureInfo.InvariantCulture) >= Properties.Settings.Default.FontSizeMin ?
                int.Parse(CaptionFontSizeSetting, CultureInfo.InvariantCulture) : Properties.Settings.Default.FontSizeMin;
            Properties.Settings.Default.HeaderFontSize = int.Parse(HeaderFontSizeSetting, CultureInfo.InvariantCulture) >= Properties.Settings.Default.FontSizeMin ?
                int.Parse(HeaderFontSizeSetting, CultureInfo.InvariantCulture) : Properties.Settings.Default.FontSizeMin;

            Properties.Settings.Default.TableFontFamily = TableFontFamilySetting;
            Properties.Settings.Default.ClockFontFamily = ClockFontFamilySetting;
            Properties.Settings.Default.CaptionFontFamily = CaptionFontFamilySetting;
            Properties.Settings.Default.HeaderFontFamily = HeaderFontFamilySetting;
        }

        /// ************************************************************************************************
        private void RestoreDefaultProperties()
        {
            Properties.Settings.Default.Reset();
            LoadProperties(callFromRestoreDefault: true);
        }

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        public ICommand OkCommand { get; set; }

        /// ************************************************************************************************
        private void ExecuteOkMethod(object obj)
        {
            SaveProperties();
        }

        /// ************************************************************************************************
        private bool CanExecuteOkMethod(object arg)
        {
            return true;
        }

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        public ICommand DefaultCommand { get; set; }
        private void ExecuteDefaultMethod(object obj)
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            if (MessageBox.Show("Möchten Sie Standardeinstellungen laden?", "Vorsicht!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            {
                RestoreDefaultProperties();
            }
        }

        /// ************************************************************************************************
        private bool CanExecuteDefaultMethod(object arg)
        {
            return true;
        }

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        public ICommand FontSizeChangedCommand { get; set; }
        /// ************************************************************************************************
        private void ExecuteFontSizeChangedMethod(object obj)
        {
        }
        /// ************************************************************************************************
        private bool CanExecuteFontSizeChangedMethod(object arg)
        {
            return true;
        }

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        public ICommand FontSizePreviewTextInputCommand { get; set; }

        /// ************************************************************************************************
        private void ExecuteFontSizePreviewTextMethod(object obj)
        {
            if (obj is TextCompositionEventArgs)
            {
                (obj as TextCompositionEventArgs).Handled = IsFontSizeTextAllowed((obj as TextCompositionEventArgs).Text);
            }
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+");

        /// ************************************************************************************************
        private static bool IsFontSizeTextAllowed(string text)
        {
            return _regex.IsMatch(text);
        }

        /// ************************************************************************************************
        private bool CanExecuteFontSizePreviewTextMethod(object arg)
        {
            return true;
        }

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        public ICommand ColorPickCommand
        { get; set; }
        private bool CanExecuteColorPickMethod(object parameter)
        {
            return true;

        }

        /// ************************************************************************************************
        private void ExecuteColorPickMethod(object parameter)
        {
            string value = GetPropertyByName(parameter);
            if (!string.IsNullOrEmpty(value))
            {
                Color color = (Color)ColorConverter.ConvertFromString(GetPropertyByName(parameter));
                string parameterName = parameter.ToString().Replace("Setting", "");
                string title = ColorPickerNames.ContainsKey(parameterName) ? ColorPickerNames[parameterName] : "";
                ColorPickerDialog.ShowDialog(ref color, title);
                string hexColor = "#" + color.A.ToString("X2", CultureInfo.InvariantCulture)
                                      + color.R.ToString("X2", CultureInfo.InvariantCulture)
                                      + color.G.ToString("X2", CultureInfo.InvariantCulture)
                                      + color.B.ToString("X2", CultureInfo.InvariantCulture);
                SetPropertyByName(parameter, hexColor);
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyname"></param>
        private void OnPropertyChange(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private string GetPropertyByName(object propertyName)
        {
            if (propertyName is string)
            {
                try
                {
                    return (string)GetType().GetProperty(propertyName as string).GetValue(this, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return "";
                }
            }
            return "";
        }


        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private bool SetPropertyByName(object propertyName, string propertyValue)
        {
            bool result = false;
            if (propertyName is string)
            {
                try
                {
                    var property = typeof(SettingsViewModel).GetProperty(propertyName as string);
                    property.SetValue(this, propertyValue, null);
                    result = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return result;
        }
    }
}

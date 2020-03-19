using System;
using System.Reflection;
using System.Windows.Media;

namespace MVGTimeTable
{
    public struct MVGTimeTableSettings
    {
        public string StationName { get; }
        public string TableForegroundColor1 { get; }
        public string TableForegroundColor2 { get; }
        public string TableBackgroundColor1 { get; }
        public string TableBackgroundColor2 { get; }

        public string HeaderForegroundColor { get; }
        public string HeaderBackgroundColor { get; }

        public string NoConnectionForegroundColor { get; }
        public string WarningForegroundColor { get; }

        public int TableFontSize { get; }
        public int HeaderFontSize { get; }

        public FontFamily TableFontFamily { get; }
        public FontFamily HeaderFontFamily { get; }

        public MVGTimeTableSettings(string  stationName,
                                    string  tableForegroundColor1,
                                    string  tableForegroundColor2,
                                    string  tableBackgroundColor1,
                                    string  tableBackgroundColor2,
                                    string  headerForegroundColor,
                                    string  headerBackgroundColor,
                                    string  noConnectionForegroundColor,
                                    string  warningForegroundColor,
                                    int     tableFontSize,
                                    int     headerFontSize,
                                    FontFamily tableFontFamily,
                                    FontFamily headerFontFamily
                                   )
        {
            StationName = stationName;
            TableForegroundColor1 = tableForegroundColor1;
            TableForegroundColor2 = tableForegroundColor2;
            TableBackgroundColor1 = tableBackgroundColor1;
            TableBackgroundColor2 = tableBackgroundColor2;
            HeaderForegroundColor = headerForegroundColor;
            HeaderBackgroundColor = headerBackgroundColor;
            NoConnectionForegroundColor = noConnectionForegroundColor;
            WarningForegroundColor = warningForegroundColor;
            TableFontSize = tableFontSize;
            HeaderFontSize = headerFontSize;
            TableFontFamily = tableFontFamily;
            HeaderFontFamily = headerFontFamily;
        }

        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is MVGTimeTableSettings mttsObj)
            {
                result = 
                            string.Equals(StationName, mttsObj.StationName) &&
                            string.Equals(TableForegroundColor1, mttsObj.TableForegroundColor1) &&
                            string.Equals(TableForegroundColor2, mttsObj.TableForegroundColor2) &&
                            string.Equals(TableBackgroundColor1, mttsObj.TableBackgroundColor1) &&
                            string.Equals(TableBackgroundColor2, mttsObj.TableBackgroundColor2) &&
                            string.Equals(HeaderForegroundColor, mttsObj.HeaderForegroundColor) &&
                            string.Equals(HeaderBackgroundColor, mttsObj.HeaderBackgroundColor) &&
                            TableFontSize == mttsObj.TableFontSize &&
                            HeaderFontSize == mttsObj.HeaderFontSize &&
                            TableFontFamily.Equals(mttsObj.TableFontFamily) && HeaderFontFamily.Equals(mttsObj.HeaderFontFamily);
            }
            return result;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();

            foreach (var field in typeof(MVGTimeTableSettings).GetFields(BindingFlags.Instance |
                                                 BindingFlags.NonPublic |
                                                 BindingFlags.Public))
            {
                hash.Add(field.GetHashCode());
            }
            return hash.ToHashCode();
        }

        public static bool operator ==(MVGTimeTableSettings left, MVGTimeTableSettings right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MVGTimeTableSettings left, MVGTimeTableSettings right)
        {
            return !(left == right);
        }
    }
}

// Copyright (c) Sergei Grigorev. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Media;


namespace MVGLive
{
    /// <summary>
    /// Interaction logic for ColorPickerWindow.xaml
    /// </summary>
    public partial class ColorPickerDialog : Window
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly int WidthMax = 574;
        /// <summary>
        /// 
        /// </summary>
        private readonly int WidthMin = 342;
        /// <summary>
        /// 
        /// </summary>
        private bool SimpleMode { get; set; }

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        public ColorPickerDialog()
        {
            InitializeComponent();
        }

        /// ************************************************************************************************
        /// <summary>
        /// Shows color picker dialog
        /// </summary>
        /// <param name="color">Displayed color</param>
        /// <param name="caption">Displayed caption</param>
        /// <returns></returns>
        public static bool ShowDialog(ref Color color, string caption = "Farbwähler")
        {
            
            ColorPickerDialog instance = new ColorPickerDialog();

            instance.ColorPicker.SetColor(color);
            instance.Title = caption;
            color = instance.ColorPicker.Color;

            var result = instance.ShowDialog();
            if (result.HasValue && result.Value)
            {
                color = instance.ColorPicker.Color;
                return true;
            }

            return false;
        }

        /// ************************************************************************************************
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Hide();
        }

        /// ************************************************************************************************
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Hide();
        }

        /// ************************************************************************************************
        private void MinMaxViewButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (SimpleMode)
            {
                SimpleMode = false;
                MinMaxViewButton.Content = "<< Einfach";
                Width = WidthMax;
            }
            else
            {
                SimpleMode = true;
                MinMaxViewButton.Content = "Vorgerückt >>";
                Width = WidthMin;
            }
        }

        /// ************************************************************************************************
        /// <summary>
        /// 
        /// </summary>
        public void ToggleSimpleAdvancedView()
        {
            if (SimpleMode)
            {
                SimpleMode = false;
                MinMaxViewButton.Content = "<< Einfach";
                Width = WidthMax;
            }
            else
            {
                SimpleMode = true;
                MinMaxViewButton.Content = "Vorgerückt >>";
                Width = WidthMin;
            }
        }
    }
}
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace RArcher.UAP.Toolkit.Converter
{
    /// <summary>XAML value converter. Converts bools to/from System.Windows.Visibility</summary>
    public sealed class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>Convert</summary>
        /// <param name="value">Value</param>
        /// <param name="targetType">Target type</param>
        /// <param name="parameter">Object to convert</param>
        /// <param name="language">Culture</param>
        /// <returns>Returns converted value</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>Convert</summary>
        /// <param name="value">Value</param>
        /// <param name="targetType">Target type</param>
        /// <param name="parameter">Object to convert</param>
        /// <param name="language">Culture</param>
        /// <returns>Returns converted value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}
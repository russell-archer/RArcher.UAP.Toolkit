using System;
using Windows.UI.Xaml.Data;

namespace RArcher.UAP.Toolkit.Converter
{
    /// <summary>XAML value converter. Converts and inverts booleans</summary>
    public sealed class BooleanInverterConverter : IValueConverter
    {
        /// <summary>Convert</summary>
        /// <param name="value">Value</param>
        /// <param name="targetType">Target type</param>
        /// <param name="parameter">Object to convert</param>
        /// <param name="language">Culture</param>
        /// <returns>Returns converted value</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is bool) return !(bool) value;
            return false;
        }

        /// <summary>Convert back</summary>
        /// <param name="value">Value</param>
        /// <param name="targetType">Target type</param>
        /// <param name="parameter">Object to convert</param>
        /// <param name="language">Culture</param>
        /// <returns>Returns converted value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool) return !(bool)value;
            return true;
        }
    }
}
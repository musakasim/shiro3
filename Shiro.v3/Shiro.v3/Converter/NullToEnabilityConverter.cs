using System;
using System.Globalization;
using System.Windows.Data;

namespace Shiro.Converter
{
    [ValueConversion(typeof(object), typeof(bool))]
    public sealed class NullToEnabilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
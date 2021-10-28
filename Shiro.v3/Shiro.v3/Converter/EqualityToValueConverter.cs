using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Shiro.Converter
{
    public class EqualityToValueConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return FalseValue;
            return value.ToString().Equals(parameter) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.Equals(TrueValue) ? parameter : null;
        }
    }

    /// <summary>
    ///     TruValue and False Value will be given in xaml, where converter resource added
    /// </summary>
    public class EqualityToVisibilityConverter : EqualityToValueConverter<Visibility>
    {
    }

    /// <summary>
    ///     TruValue and False Value will be given in xaml, where converter resource added
    /// </summary>
    public class EqualityToBooleanConverter : EqualityToValueConverter<bool>
    {
    }
}
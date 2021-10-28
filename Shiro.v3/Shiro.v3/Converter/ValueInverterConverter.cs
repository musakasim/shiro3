using System;
using System.Globalization;
using System.Windows.Data;

namespace Shiro.Converter
{
    /// <summary>
    /// inverts a integer value according to parameter value
    /// ex: parameter= 2400 value=1000
    ///     conversion = 2400 - 1400
    ///     converted value = 1400
    /// if result is lesser than 0, then returns max value
    /// </summary>
    [ValueConversion(typeof(int), typeof(int), ParameterType = typeof(int))]
    public class ValueInverterConverter : IValueConverter
    {
        public int BaseValue { get; set; }

        /// <summary>
        /// If fallback value is set, ignores this value and returns as it is
        /// </summary>
        public int? FallBackValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {  
            var intValue = System.Convert.ToInt32(value);
            if (FallBackValue.HasValue && intValue == FallBackValue.Value)
            {
                return intValue;
            }
            var difference = BaseValue - intValue;

            return difference < 0 ? BaseValue : difference;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
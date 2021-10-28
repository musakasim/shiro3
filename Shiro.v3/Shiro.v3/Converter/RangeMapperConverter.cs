using System;
using System.Globalization;
using System.Windows.Data;

namespace Shiro.Converter
{

    /// <summary>
    ///   X falls between A and B,   Y to fall between C and D, applies the following linear transform:
    ///     Y = (X-A)/(B-A) * (D-C) + C
    /// </summary> 
    public class RangeMapperConverter : IValueConverter
    {
        public double BaseMinValue { get; set; }
        public double BaseMaxValue { get; set; }
        public double TargetMinValue { get; set; }
        public double TargetMaxValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var x = System.Convert.ToDouble(value);

            var y = ( (x - BaseMinValue) / (BaseMaxValue - BaseMinValue) ) * (TargetMaxValue - TargetMinValue) + TargetMinValue;

            return (int)y;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
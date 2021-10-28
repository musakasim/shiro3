using System;
using System.Globalization;
using System.Windows.Data;

namespace Shiro.Converter
{
    [ValueConversion(typeof(int), typeof(int), ParameterType = typeof(decimal))]
    public class MultiplierConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine(targetType);
            //if (targetType != typeof(decimal))
            //{
            //    throw new InvalidDataException("Target type must be integer");
            //}
            return  System.Convert.ToInt32(value)  *  System.Convert.ToInt32(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    } 
}
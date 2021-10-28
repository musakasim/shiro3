using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Shiro.Converter
{
    [ValueConversion(typeof(List<string>), typeof(string))]
    public class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string separator = parameter == null ? ", " : (string)parameter;
            return value != null ? String.Join(separator, (List<string>)value) : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(List<string>), typeof(string))]
    public class ListToStringWithEnclosuresConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //three parameters expected, all seperated with # char
            //first: seperator, second:opening string, third:closing string
            var sparameter = parameter == null ? "###" : (string)parameter;
            var paramss = sparameter.Split('#');
            string separator = string.IsNullOrEmpty(paramss[0]) ? ", " : paramss[0];
            string openingStr = string.IsNullOrEmpty(paramss[1]) ? "( " : paramss[1];
            string closingStr = string.IsNullOrEmpty(paramss[2]) ? " )" : paramss[2];
            return value != null && ((List<string>)value).Any()
                ? string.Format("{1}{0}{2}", String.Join(separator, (List<string>)value), openingStr, closingStr)
                : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(List<string>), typeof(string))]
    public class ListToStringWithBracketsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string separator = parameter == null ? ", " : (string)parameter;
            return value != null && ((List<string>)value).Any()
                ? string.Format("【 {0} 】", String.Join(separator, (List<string>)value))
                : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
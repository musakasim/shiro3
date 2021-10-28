using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using Shiro.Library.FuriganaView;

namespace Shiro.Converter
{
    [ValueConversion(typeof (string), typeof (List<MeCabData>))]
    public class TextTokenizeWithFuriganaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof (IEnumerable))
            {
                throw new InvalidDataException("Target type must be IEnumerable<string>");
            }
            List<MeCabData> tokens = JapaneseTextTokenizer.TokenizeAsMeCabData((string) value);
            return tokens;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
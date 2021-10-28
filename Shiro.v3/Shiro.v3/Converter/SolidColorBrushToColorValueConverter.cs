using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Shiro.Converter
{
    public class SolidColorBrushToColorValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            //todo: bu 2 satır comment'lenecek, ama commentlenince program patlıyor
            if (value is Color)
                return (Color)value;

            if (value is SolidColorBrush)
                return ((SolidColorBrush)value).Color;

            throw new InvalidOperationException("Unsupported type [" + value.GetType().Name + "], SolidColorBrushToColorValueConverter.Convert()");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            //todo: bu 2 satır comment'lenecek, ama commentlenince program patlıyor
            if (value is SolidColorBrush)
                return value;

            if (value is Color)
                return new SolidColorBrush((Color)value);

            throw new InvalidOperationException("Unsupported type [" + value.GetType().Name + "], SolidColorBrushToColorValueConverter.ConvertBack()");
        }

    }
}
﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Shiro.Converter
{
    [ValueConversion(typeof (object), typeof (Visibility))]
    public sealed class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
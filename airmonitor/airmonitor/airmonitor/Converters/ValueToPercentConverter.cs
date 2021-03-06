﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace airmonitor.Converters
{
    internal class ValueToPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!double.TryParse(value?.ToString(), out var result)) return value;

            return result * 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
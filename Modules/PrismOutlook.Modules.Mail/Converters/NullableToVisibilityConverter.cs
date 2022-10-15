using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PrismOutlook.Modules.Mail.Converters;

public class NullableToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value == null ? Visibility.Collapsed : (object)Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

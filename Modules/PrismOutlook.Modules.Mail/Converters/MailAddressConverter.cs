using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace PrismOutlook.Modules.Mail.Converters;

public class MailAddressConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is ObservableCollection<string> emails ? string.Join("; ", emails) : (object)string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var emailCollection = new ObservableCollection<string>();

        if (string.IsNullOrWhiteSpace(value?.ToString()))
            return emailCollection;

        var emails = value.ToString().Replace(" ", "");
        var emailItems = emails.Split(';', StringSplitOptions.RemoveEmptyEntries);
        return emailCollection.AddRange(emailItems);
    }
}

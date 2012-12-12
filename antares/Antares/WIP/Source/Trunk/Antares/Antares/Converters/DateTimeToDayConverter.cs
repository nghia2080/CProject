using System;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
    public class DateTimeToDayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is DateTime) ? ((DateTime)value).Day + string.Empty : "--";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

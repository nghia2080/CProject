using System;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
    public class StatusToDateSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var active = (int)value;

            return active == 0 ? "14" : "20";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

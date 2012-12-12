using System;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
    public class StatusToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var status = (int?)value;
            return (status == null || status == 0) ? "Inactive" : "Active";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var status = (string)value;
            return status == "Inactive" ? 0 : 1;
        }
    }
}

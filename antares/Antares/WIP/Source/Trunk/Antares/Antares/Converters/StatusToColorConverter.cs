using System;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var active = (int) value;

            return active == 0 ? "#878787" : "#046380";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

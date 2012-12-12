using System;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
     public class StatusToFontSizeConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var active = (int)value;

            return active == 0 ? "20" : "35";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

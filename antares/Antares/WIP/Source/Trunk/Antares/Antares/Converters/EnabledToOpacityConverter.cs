using System;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
    public class EnabledToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var enabled = (bool) value;
            if(enabled)
            {
                return 1;
            }
            else
            {
                return 0.3;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

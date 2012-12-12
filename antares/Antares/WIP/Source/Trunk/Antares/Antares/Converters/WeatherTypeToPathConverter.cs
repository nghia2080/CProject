using System;
using Repository.MODELs;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
    public class WeatherTypeToPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var weathertype = (WeatherType) value;

            return "/Assets/WeatherIcon/[IMAGE].png".Replace("[IMAGE]", weathertype.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

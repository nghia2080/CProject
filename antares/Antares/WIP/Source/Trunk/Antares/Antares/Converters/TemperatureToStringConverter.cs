using System;
using Repository.MODELs;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
    public class TemperatureToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var weather = value as DayWeatherModel;
            if(weather == null)
            {
                return "N/A";
            }

            return weather.MinmaxTempuratureC == null ? "N/A" : string.Format("{0}°", weather.MinmaxTempuratureC);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

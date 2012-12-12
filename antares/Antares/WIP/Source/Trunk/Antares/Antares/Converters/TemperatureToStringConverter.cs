using System;
using AntaresShell.Localization;
using Repository.MODELs;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
    public class TemperatureToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var weather = value as DayWeatherModel;
            if(weather == null || weather.MinmaxTempuratureC == null)
            {
                return "N/A";
            }

            var temp = weather.MinmaxTempuratureC.Split(new[] {"° /"}, StringSplitOptions.None);
            try
            {
                var minC = temp[0];
                var maxC = temp[1];

                if (LanguageProvider.CurrentLanguage == "vi" || LanguageProvider.CurrentLanguage == "ja")
                {
                    return string.Format("{0}° / {1}°", minC, maxC);
                }
                else
                {
                    return string.Format("{0}° / {1}°", ToFahrenheit(minC), ToFahrenheit(maxC));
                }
            }
            catch (Exception)
            {
                return "N/A";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private int ToFahrenheit(string temp)
        {
            var cTemp = System.Convert.ToInt32(temp);

            return System.Convert.ToInt32(cTemp*1.8 + 32);
        }
    }
}

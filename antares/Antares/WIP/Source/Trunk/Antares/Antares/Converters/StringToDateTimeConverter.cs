using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
    public class StringToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value + string.Empty == string.Empty)
            {
                return null;
            }

            return System.Convert.ToDateTime(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var date = value.ToString();
            var sp = date.Split(new[] { '/', ' ', '-' });
            switch (CultureInfo.CurrentCulture.Name.ToLower())
            {
                case "vi":
                    date = System.Convert.ToDateTime(value).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                    break;
                default:
                    date = System.Convert.ToDateTime(value).ToString();
                    break;
            }
            return date;
        }
    }
}

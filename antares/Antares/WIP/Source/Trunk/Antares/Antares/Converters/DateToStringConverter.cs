using System;
using System.Globalization;
using AntaresShell.Localization;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
    public class DateToStringConverter : IValueConverter
    {
        private StringToDateTimeConverter std = new StringToDateTimeConverter();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var date = "";
            if(value == null)
            {
                return LanguageProvider.Resource["Present"];
            }
            switch (CultureInfo.CurrentCulture.Name.ToLower())
            {
                case "vi":
                    date = System.Convert.ToDateTime(value).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    break;
                default:
                    date = System.Convert.ToDateTime(value).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);;
                    break;
            }

            
            return date;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

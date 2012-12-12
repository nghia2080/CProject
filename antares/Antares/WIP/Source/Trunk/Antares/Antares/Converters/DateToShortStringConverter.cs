using AntaresShell.Localization;
using System;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
    public class DateToShortStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var date = (DateTime) value;

            if ((date.Day == DateTime.Now.Day) 
                && (date.Month == DateTime.Now.Month) 
                && (date.Year == DateTime.Now.Year)){
                
                return LanguageProvider.Resource["Today"];} 
            
            else
            {
                if(LanguageProvider.CurrentLanguage == "ja")
                {
                    return date.ToString("dddd") + " " + date.Day;
                }
                else
                {
                   return date.ToString("ddd") + " " + date.Day; 
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

using Repository.Sync;
using System;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
    public class DateToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var date = (DateTime) value;

            return date.Month == GlobalData.SelectedMonthIndex ? "1" : "0.3";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

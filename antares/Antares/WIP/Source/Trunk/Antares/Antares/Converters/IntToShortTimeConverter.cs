using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
    public class IntToShortTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var iDate = System.Convert.ToInt32(value);
            if (iDate == 0)
            {
                return null;
            }

            return (iDate / 60).ToString("00") + ":" + (iDate % 60).ToString("00");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var datetime = System.Convert.ToDateTime(value);

            if (datetime == default(DateTime))
            {
                return null;
            }

            return datetime.Hour * 60 + datetime.Minute;
        }
    }
}

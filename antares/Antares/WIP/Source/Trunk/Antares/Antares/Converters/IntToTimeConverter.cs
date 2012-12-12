using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
    public class IntToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var iDate = System.Convert.ToInt32(value);
            if (iDate == 0)
            {
                return null;
            }

            try
            {
                var date = new DateTime(DateTime.Now.Year,
                            DateTime.Now.Month,
                            DateTime.Now.Day,
                            iDate / 60,
                            iDate % 60,
                            0);

                return date;
            }
            catch (Exception)
            {
                return null;
            }

            
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var datetime = System.Convert.ToDateTime(value);

            if(datetime == default(DateTime))
            {
                return null;
            }

            return datetime.Hour*60 + datetime.Minute;
        }
    }
}

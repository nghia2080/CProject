using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.MODELs;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
    public class DataContextToDateRangeConverter : IValueConverter
    {
        IntToShortTimeConverter intToTimeConverter = new IntToShortTimeConverter();
                       
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var task = value as TaskModel;

            if(task == null)
            {
                return null;
            }

            var result =  System.Convert.ToDateTime(task.StartDate).ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            result += " - ";
            result += System.Convert.ToDateTime(task.EndDate).ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.MODELs;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
    public class TaskToShortInfoConverter : IValueConverter
    {
        readonly IntToShortTimeConverter _its = new IntToShortTimeConverter();
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }

            if (parameter == null)
            {
                return "??? - " + value;
            }
            else
            {
                return _its.Convert((int)parameter, null, null, null) + " - " + value;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.MODELs;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
    public class TimeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var model = value as TaskModel;

            var sdt = DateTime.Parse(model.StartDate);
            var edt = DateTime.Parse(model.EndDate);

            if (model.Status == 0)
            {
                if (DateTime.Now < sdt)
                {
                    return "#FFF380";
                }
                else if (DateTime.Now > sdt && DateTime.Now < edt)
                {
                    return "#33FF66";
                }
                else
                {
                    return "Red";
                }
            }
            else
            {
                return "#E0E0E0";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

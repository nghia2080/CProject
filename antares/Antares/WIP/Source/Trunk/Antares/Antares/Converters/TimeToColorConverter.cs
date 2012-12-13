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

         
            var edt = DateTime.Parse(model.EndDate);

            if (model.Status == 0)
            {
                if (DateTime.Now < edt)
                {
                    return "../Assets/TaskTemplate/ondue.png";
                }
                else
                {
                    return "../Assets/TaskTemplate/overdue.png";
                }
            }
            else
            {
                return "../Assets/TaskTemplate/done.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

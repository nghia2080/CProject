using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
    public class CategoryToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cate = (int)value;
            switch (cate)
            {
                case 0:
                    return "../Assets/TaskIcon/Business.png";
                case 1:
                    return "../Assets/TaskIcon/Meeting.png";
                case 2:
                    return "../Assets/TaskIcon/Entertainment.png";
                case 3:
                    return "../Assets/TaskIcon/Study.png";
                case 4:
                    return "../Assets/TaskIcon/Other.png";

                case 10:
                    return "../Assets/TaskIcon/Requirement.png";
                case 11:
                    return "../Assets/TaskIcon/Design.png";
                case 12:
                    return "../Assets/TaskIcon/Implementation.png";
                case 13:
                    return "../Assets/TaskIcon/Verification.png";
                case 14:
                    return "../Assets/TaskIcon/Maintenance.png";

                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

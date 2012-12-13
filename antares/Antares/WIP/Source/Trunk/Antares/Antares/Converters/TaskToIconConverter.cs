using Repository.Sync;
using System;
using Windows.UI.Xaml.Data;
using Repository.MODELs;
namespace Antares.Converters
{
    public class TaskToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int cateID = 0;
            if (value is TaskModel)
            {
                var model = value as TaskModel;
                cateID = model.Category;

            }
            else if (value is GroupCollection)
            {
                var model = value as GroupCollection;
                cateID = model.Id;
            }

            switch (cateID)
            {
                case 10:
                    return "../Assets/TaskTemplate/requirement_icon.png";
                case 11:
                    return "../Assets/TaskTemplate/design_icon.png";
                case 12:
                    return "../Assets/TaskTemplate/implementation_icon.png";
                case 13:
                    return "../Assets/TaskTemplate/verification_icon.png";
                case 14:
                    return "../Assets/TaskTemplate/maintenance_icon.png";
                default:
                    return "../Assets/TaskTemplate/task_icon.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

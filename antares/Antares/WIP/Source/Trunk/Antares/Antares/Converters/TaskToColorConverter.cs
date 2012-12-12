using Repository.Sync;
using System;
using Windows.UI.Xaml.Data;
using Repository.MODELs;
namespace Antares.Converters
{
    public class TaskToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var group = new GroupCollection();

            switch (group.GroupName)
            {
                case "Requirements": return "#e44eff";
                case "Design": return "#ff4eae";
                case "Implementation": return "#4eff91";
                case "Verification": return "#c7ff4e";
                case "Maintanence": return "#524eff";
                default: return "#e44eff";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

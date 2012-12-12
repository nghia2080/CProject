using AntaresShell.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Antares.Converters
{
    public class StringToRoleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var role = (string) value;
            return string.IsNullOrEmpty(role) 
                ? LanguageProvider.Resource["Prj_Mber"] 
                : LanguageProvider.Resource["Prj_PM"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntaresShell.Common;
using AntaresShell.Logger;
using Windows.ApplicationModel.Resources.Core;

namespace AntaresShell.Localization
{
    public static class LanguageProvider
    {
        public static ObservableDictionary<string, string> Resource { get; set; }

        public static string CurrentLanguage { get; private set; }

        private static ResourceContext GetCurrentCulture()
        {
            var rc = new ResourceContext { Languages = new List<string> { Windows.System.UserProfile.GlobalizationPreferences.Languages[0] } };
            CurrentLanguage = rc.Languages[0];
            return rc;
        }

        /// <summary>
        /// Get display resource for current project.
        /// </summary>
        /// <returns>Resource of project as a dictionary.</returns>
        public static void InitDisplayResources()
        {
            var culture = GetCurrentCulture();

            if (Resource != null)
            {
                return;
            }

            Resource = new ObservableDictionary<string, string>();
            try
            {
                var resourceStringMap = ResourceManager.Current.MainResourceMap.GetSubtree("Resources");
                foreach (var key in resourceStringMap.Keys)
                {
                    // get resources base on context language
                    Resource[key] = resourceStringMap.GetValue(key, culture).ValueAsString;
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance.LogException(exception.ToString());
            }


            Resource["DOW_1"] = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[0];
            Resource["DOW_2"] = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[1];
            Resource["DOW_3"] = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[2];
            Resource["DOW_4"] = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[3];
            Resource["DOW_5"] = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[4];
            Resource["DOW_6"] = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[5];
            Resource["DOW_7"] = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[6];
        }
    }
}

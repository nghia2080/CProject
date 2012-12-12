using System;
using System.Globalization;

namespace Repository
{
    public class RepositoryUtils
    {
        /// <summary>
        /// Converts data of date and time in database into C# DateTime object.
        /// </summary>
        /// <param name="date">The Date string in database.</param>
        /// <param name="time">The time (minutes, from 00:00) in database.</param>
        /// <returns>A DateTime object corresponds to the inputs.</returns>
        public static DateTime GetDateTimeFromStrings(string date, int? time)
        {
            DateTime ret;
            IFormatProvider cultureInfo = new CultureInfo("en-US");
            ret = DateTime.Parse(date);

            //switch (CultureInfo.CurrentCulture.Name.ToLower())
            //{
            //    case "vi":
            //        ret = DateTime.ParseExact(date, "MM/dd/yyyy", CultureInfo.CurrentCulture);
            //        break;
            //    default:
            //        ret = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.CurrentCulture);
            //        break;
            //}
            return ret.AddMinutes(time ?? 0);
            
            throw new ArgumentException("Cannot parse string " + date + " to DateTime object.");
        }

        /// <summary>
        /// Gets category name knowing the Id.
        /// </summary>
        /// <param name="categoryId">The category Id.</param>
        /// <returns>A string repesents the name of task's category.</returns>
        public static string GetCategoryNameFromId(int categoryId)
        {
            return categoryId.ToString();
        }
    }
}

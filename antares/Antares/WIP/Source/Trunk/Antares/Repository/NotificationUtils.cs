using System;
using System.Linq;
using Repository.MODELs;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Repository
{
    class NotificationUtils
    {
        public static void CreateNotification(TaskModel data)
        {
            // NghiaTT - Add toast notification
            // TODO: test
            const string xmlFormat = "<toast><visual><binding template='ToastText02'><text id='1'>{0}</text><text id='2'>{1}</text></binding></visual></toast>";
            var xmlString = string.Format(xmlFormat, data.Name, data.Description);
            var xmlDom = new XmlDocument();
            xmlDom.LoadXml(xmlString);
            var dueTime = RepositoryUtils.GetDateTimeFromStrings(data.StartDate, data.StartTime).AddSeconds(15); // 15' before the start time

            var endTime = RepositoryUtils.GetDateTimeFromStrings(data.EndDate, data.EndTime);

            var deviation = (endTime.Year * 365 + endTime.Month * 30 + endTime.Day) - (dueTime.Year * 365 + dueTime.Month * 30 + dueTime.Day);

            if (dueTime < DateTime.Now) return;
            //var dueTime = DateTime.Now.AddSeconds(5);
            ScheduledToastNotification toast;

            switch (data.RepeatType)
            {
                    // NghiaTT: Why 1e9?? Snooze a billion times?
                case 1:
                    toast = new ScheduledToastNotification(xmlDom, dueTime, TimeSpan.FromDays(1), (uint)1e9);
                    break;
                case 2:
                    try
                    {
                        toast = new ScheduledToastNotification(xmlDom, dueTime, TimeSpan.FromDays(7), (uint)1e9 / 7);
                    }
                    catch (Exception)
                    {
                        
                        throw;
                    }
                    
                    break;
                case 3:
                    // TODO: check for day in month
                    toast = new ScheduledToastNotification(xmlDom, dueTime, TimeSpan.FromDays(30), (uint)1e9 / 30);
                    break;
                case 4:
                    // TODO: check for leap year
                    toast = new ScheduledToastNotification(xmlDom, dueTime, TimeSpan.FromDays(365), (uint)1e9 / 365);
                    break;
                default:
                    toast = new ScheduledToastNotification(xmlDom, dueTime);
                    break;
            }
            toast.Id = data.ID.ToString();
            ToastNotificationManager.CreateToastNotifier().AddToSchedule(toast);
        }

        public static void DeleteNotification(int id)
        {
            var itemId = id.ToString();
            var notifier = ToastNotificationManager.CreateToastNotifier();
            var scheduled = notifier.GetScheduledToastNotifications();
            foreach (var t in scheduled.Where(t => t.Id.Equals(itemId)))
            {
                notifier.RemoveFromSchedule(t);
            }
        }
    }
}

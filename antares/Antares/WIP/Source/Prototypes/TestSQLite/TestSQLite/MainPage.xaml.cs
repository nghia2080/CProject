using eSECSync.Clients;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using eSECSync.Helpers;
using eSECSync.Networks;
using Windows.UI.Notifications;
using eSECSync.Helpers.Model;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TestSQLite
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ProgressBarHandler.Instance.ProgressBar = UploadProgressBar;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (await LiveConnection.Instance.CreateConnectionAsync())
            {
                var info = await SayHi();
                var localStorageManager =new LocalStorageManager("UserInfo.xml");
                await localStorageManager.SaveAsync(info);
                
                await UploadFile();

                await DownloadFile();

                var dlocalStorageManager = new LocalStorageManager("DownloadedUserInfo.xml");
                var restoreData = await dlocalStorageManager.RestoreAsync<UserInformationModel>();
            }
        }

        private async Task DownloadFile()
        {
            var destinationFile = await EnsureBaseFolder.Instance.PersonalFolder.CreateFileAsync("DownloadedUserInfo.xml", CreationCollisionOption.ReplaceExisting);
            await LiveConnection.Instance.DownloadDataAsync(destinationFile, "UserInfo.xml");
            CreateNotification("Finished Download");
        }

        private async Task UploadFile()
        {
            var file = await EnsureBaseFolder.Instance.PersonalFolder.GetFileAsync("UserInfo.xml");
            await LiveConnection.Instance.UploadDataAsync(file);
            CreateNotification("Finished Upload");
        }

        private async Task<UserInformationModel> SayHi()
        {
            var info = await LiveConnection.Instance.GetUserInformation();

            if (info == null)
            {
                return null;
            }

            CreateNotification("Hi, " + info.FullName + "!");
           
            return info;
        }

        private void CreateNotification(string content)
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();

            // Make sure notifications are enabled
            if (notifier.Setting != NotificationSetting.Enabled)
            {
                var dialog = new Windows.UI.Popups.MessageDialog("Notifications are currently disabled");
                dialog.ShowAsync();
                return;
            }

            // Get a toast template and insert a text node containing a message
            var template = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            var element = template.GetElementsByTagName("text")[0];
            element.AppendChild(template.CreateTextNode(content));

            // Schedule the toast to appear 5 seconds from now
            var stn = new ScheduledToastNotification(template, DateTime.Now.AddMilliseconds(100));

            notifier.AddToSchedule(stn);
            notifier.GetScheduledToastNotifications();

        }
    }

}

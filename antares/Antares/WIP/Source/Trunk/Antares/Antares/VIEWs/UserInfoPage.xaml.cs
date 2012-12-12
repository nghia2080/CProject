using System;
using System.Net.Http;
using Repository.Sync;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using AntaresShell.IO;
using Repository.ServiceConnection.Controllers;
using Repository.MODELs;
using System.Globalization;
using Repository.LiveConnection;
using AntaresShell.NavigatorProvider;
using Repository.Repositories;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Antares.VIEWs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserInfoPage
    {
        public UserInfoPage()
        {
            InitializeComponent();
        }

        private UserModel _currentUser;

        readonly Uri _unknownUserUri = new Uri("ms-appx:///Assets/defaultUser.png");

        protected async override void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
            Navigator.Instance.MainProgressBar.Visibility = Visibility.Visible;

            MessageArea.Visibility = Visibility.Visible;
            DynamicArea.Visibility = Visibility.Collapsed;

            if (GlobalData.UserInformationModel != null)
            {
                _currentUser = await UserInformationRepository.Instance.GetUser(GlobalData.MyUserID);
                if (_currentUser == null)
                {
                    messageTxt.Text = "eSec can not retrieve live data now.";
                    MessageArea.Visibility = Visibility.Visible;
                    DynamicArea.Visibility = Visibility.Collapsed;
                    Navigator.Instance.MainProgressBar.Visibility = Visibility.Collapsed;
                    return;
                }

                MessageArea.Visibility = Visibility.Collapsed;
                DynamicArea.Visibility = Visibility.Visible;

                FullName.Text = GlobalData.UserInformationModel.FullName;
                Username.Text = _currentUser.Username + "";
                
                var ci = new CultureInfo(GlobalData.UserInformationModel.Locale.Replace("_", "-"));
                Locale.Text = ci.DisplayName;
                UserPic.Source = new BitmapImage(_unknownUserUri);
                UpdateUserPic(LiveConnection.Instance.GetUserAvatarUrl(), GlobalData.UserInformationModel.ID);
                if (_currentUser.DOB != null)
                {
                    Birthday.Value = System.Convert.ToDateTime(_currentUser.DOB);
                }

                PhoneNumber.Text = _currentUser.Phone + "";
                Email.Text = _currentUser.Email + "";
                ShowSaveBtn(false);
                Birthday.ValueChanged += Birthday_ValueChanged;
                PhoneNumber.TextChanged += PhoneNumber_TextChanged;
                Email.TextChanged += Email_TextChanged;
            }
            else
            {
                messageTxt.Text = "eSec can not retrieve live data now.";
                MessageArea.Visibility = Visibility.Visible;
                DynamicArea.Visibility = Visibility.Collapsed;
            }
          
            Navigator.Instance.MainProgressBar.Visibility = Visibility.Collapsed;
        }

        private void ShowSaveBtn(bool isVisible)
        {
            Save.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        void Email_TextChanged(object sender, Windows.UI.Xaml.Controls.TextChangedEventArgs e)
        {
            ShowSaveBtn(true);
        }

        void PhoneNumber_TextChanged(object sender, Windows.UI.Xaml.Controls.TextChangedEventArgs e)
        {
            ShowSaveBtn(true);
        }

        void Birthday_ValueChanged(object sender, EventArgs e)
        {
            ShowSaveBtn(true);
        }

        private async void UpdateUserPic(string url, string userid)
        {
            try
            {
                //try user default file
                var defaultFile = await AntaresBaseFolder.Instance.RoamingFolder.GetFileAsync(userid + ".jpg");
                var defaultUserPic = new BitmapImage();
                defaultUserPic.SetSource(await defaultFile.OpenAsync(FileAccessMode.Read));
                UserPic.Source = defaultUserPic;
            }
            catch
            {
                DownloadUserAvatar(url, userid);
            }
        }

        private async void DownloadUserAvatar(string url, string userid)
        {
            try
            {
                var hc = new HttpClient();
                var response = await hc.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var bitmapByte = await response.Content.ReadAsByteArrayAsync();

                var userFile =
                    await
                    AntaresBaseFolder.Instance.RoamingFolder.CreateFileAsync("Cache\\ProfilePic.jpg",
                                                                             CreationCollisionOption.ReplaceExisting);
                var writeStream = await userFile.OpenAsync(FileAccessMode.ReadWrite);
                var outputStream = writeStream.GetOutputStreamAt(0);
                var dataWriter = new DataWriter(outputStream);

                dataWriter.WriteBytes(bitmapByte);

                await dataWriter.StoreAsync();
                await outputStream.FlushAsync();

                writeStream.Dispose();
                outputStream.Dispose();
                dataWriter.Dispose();

                var userPic = new BitmapImage();
                userPic.SetSource(await userFile.OpenAsync(FileAccessMode.Read));
                UserPic.Source = userPic;
            }
            catch
            {
            }
        }

        private async void Save_Tapped_1(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            // Get latest data
            Navigator.Instance.MainProgressBar.Visibility = Visibility.Visible;
            Save.IsEnabled = false;

            _currentUser.Phone = PhoneNumber.Text;
            _currentUser.DOB = Birthday.Value == null ? null : Birthday.Value.ToString();
            _currentUser.Email = Email.Text;

            var message = await UserInformationRepository.Instance.UpdateUserData(_currentUser);

            Navigator.Instance.ExecuteStatus(message.IsSuccessStatusCode
                                                 ? ConnectionStatus.Done
                                                 : ConnectionStatus.Error);

            Save.Visibility = message.IsSuccessStatusCode ? Visibility.Collapsed : Visibility.Visible;
            Navigator.Instance.MainProgressBar.Visibility = Visibility.Collapsed;
            Save.IsEnabled = true;
            
        }
    }
}

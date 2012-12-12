using System.Runtime.Serialization.Json;
using AntaresShell.Common;
using AntaresShell.NavigatorProvider;
using Repository.LiveConnection;
using Repository.MODELs;
using Repository.Repositories;
using Repository.ServiceConnection.Controllers;
using Repository.Sync;
using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using AntaresShell.Localization;
using AntaresShell.Common.MessageTemplates;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Antares.VIEWs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TimelineWeekPage
    {
        /// <summary>
        /// List of commands of tiles on main page.
        /// </summary>
        private readonly List<SettingsCommand> _commands;

        public TimelineWeekPage()
        {
            NavigationCacheMode = NavigationCacheMode.Enabled;
            InitializeComponent();
            WeekFlip.Loaded += WeekFlipLoaded;

            Messenger.Instance.Register<RebindCbo>(p=>
                                                       {
                                                           var type = (RebindCbo) p;
                                                           if(type==RebindCbo.RebindWeekGrid)
                                                           {
                                                               WeekFlip.SelectedIndex = GlobalData.SelectedWeekIndex;
                                                               WeekFlip.Visibility = Visibility.Visible;  
                                                           }
                                                       });
        }



        void WeekFlipLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                WeekFlip.SelectedIndex = GlobalData.SelectedWeekIndex;
                WeekFlip.Visibility = Visibility.Visible;
            }
            catch
            {

            }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            SignIn();
        }

        private void SignInbtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SignIn();
        }

        private void BasicMonth_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            Navigator.Instance.NavigateTo(typeof(BasicMonthPage));
        }

        private void ProjectButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Navigator.Instance.NavigateTo(typeof(ProjectManagerPage), "#1:");
        }

        private async void SignIn()
        {
            messageTxt.Text = string.Empty;
            messageTxt.Visibility = Visibility.Collapsed;
            prgessBar.Visibility = Visibility.Visible;
            signInbtn.Visibility = Visibility.Collapsed;

            if (await LiveConnection.Instance.CreateConnectionAsync())
            {
                if (GlobalData.UserInformationModel == null)
                {
                    GlobalData.UserInformationModel = await LiveConnection.Instance.GetUserInformation();
                }

                if (GlobalData.UserInformationModel != null)
                {
                    ApplicationData.Current.LocalSettings.Values["UserFirstName"] = GlobalData.UserInformationModel.FirstName;

                    // Check existance of user on server
                    var cloudInfo = await UserInformationRepository.Instance.GetUser(GlobalData.UserInformationModel.Email);

                    // If null profile is returned, create blank one for newcommer.
                    if (cloudInfo == null)
                    {
                        var newBlankInfo = new UserModel
                        {
                            DOB = null,
                            Email = "",
                            Phone = "",
                            Username = GlobalData.UserInformationModel.Email
                        };

                        await UserInformationRepository.Instance.AddUser(newBlankInfo);
                    }
                    else
                    {
                        GlobalData.MyUserID = cloudInfo.UserID;
                    }
                                        
                }
                LoadBuffer();
                //TODO: temporary
                Navigator.Instance.ShowApproveNotificator();

                pageTitle.Text = (ApplicationData.Current.LocalSettings.Values["UserFirstName"] + string.Empty == string.Empty)
                    ? LanguageProvider.Resource["MainTitle_2"]
                    : LanguageProvider.Resource["MainTitle_1"].Replace("[abc]", ApplicationData.Current.LocalSettings.Values["UserFirstName"] + string.Empty);
                yearIndicator.Visibility = Visibility.Visible;
                //signOutbtn.Visibility = Visibility.Visible;
                DynamicContentGrid.Visibility = Visibility.Visible;
                MessageContent.Visibility = Visibility.Collapsed;
            }
            else
            {
                messageTxt.Text = LanguageProvider.Resource["Msg_LoginFailed"];
                messageTxt.Visibility = Visibility.Visible;
                prgessBar.Visibility = Visibility.Collapsed;
                signInbtn.Visibility = Visibility.Visible;
            }
        }

        private async void LoadBuffer()
        {
            CategoryRepository.Instance.GetAllCategories();
            PriorityRepository.Instance.GetAllPriorities();
            RepeatTypeRepository.Instance.GetAllRepeatTypes();
            
            ProjectRepository.Instance.GetAllProjects();
            TaskRepository.Instance.GetAllTasksForUser(GlobalData.MyUserID);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Antares.VIEWs;
using AntaresShell.NavigatorProvider;
using AntaresShell.Utilities;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using AntaresShell.Localization;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Antares
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly List<SettingsCommand> _commands;

        public Frame RootFrame
        {
            get { return _rootFrame; }
            private set { _rootFrame = value; }
        }

        public MainPage()
        {
            InitializeComponent();
            SizeChanged += (sender, e) =>
            {
                PopupMenu.Width = e.NewSize.Width;
                PopupMenu.Height = e.NewSize.Height;
                RootGridOfPopup.Width = e.NewSize.Width;
                RootGridOfPopup.Height = e.NewSize.Height;
            };

            Navigator.Instance.MainPopup = PopupMenu;
            Navigator.Instance.MainProgressBar = PrgBar;
            Navigator.Instance.RootFrameOfPopup = RootFrameOfPopup;
            Navigator.Instance.BottomAppBar = BottomAppBar;
            Navigator.Instance.TopAppBar = TopAppBar;
            Navigator.Instance.SetNotificator(networkStatus);
            Navigator.Instance.SetApproveNotificator(notification);
            SettingsPane.GetForCurrentView().CommandsRequested += CommandsRequestedHandler;
            _commands = new List<SettingsCommand> 
                            { 
                                new SettingsCommand(LanguageProvider.Resource["MainTitle_5"], LanguageProvider.Resource["MainTitle_5"], InformationCommandHandler),
                            };

           // AppButtonManager.Instance.GridAppbar = GridAppbar;

           // AppButtonManager.Instance.AddTaskBtn = AddNewTaskBtn;
           // //AppButtonManager.Instance.ViewTaskBtn = ViewTaskDetail;
           //// AppButtonManager.Instance.DelTaskBtn = DeleteMember;

           // AppButtonManager.Instance.AddProjectBtn = AddNewTaskBtn;
           // //AppButtonManager.Instance.ViewProjectBtn = ViewProjectDetail;
           //// AppButtonManager.Instance.DelProjectBtn = DeleteProject;

           // //AppButtonManager.Instance.GridAppbar.Children.Remove(AppButtonManager.Instance.ViewTaskBtn);
           // AppButtonManager.Instance.GridAppbar.Children.Remove(AppButtonManager.Instance.DelTaskBtn);
           // //AppButtonManager.Instance.GridAppbar.Children.Remove(AppButtonManager.Instance.ViewProjectBtn);
           // AppButtonManager.Instance.GridAppbar.Children.Remove(AppButtonManager.Instance.DelProjectBtn);

        }


        /// <summary>
        /// Event occur request of command handler.
        /// </summary>
        /// <param name="command"> Not param.</param>
        private void InformationCommandHandler(IUICommand command)
        {
            Navigator.Instance.NavigateTo(typeof(UserInfoPage));
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Navigator.Instance.NavigateTo(typeof(TimelineWeekPage));
        }

        private void CommandsRequestedHandler(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            foreach (var command in _commands.Where(command => !args.Request.ApplicationCommands.Contains(command)))
            {
                args.Request.ApplicationCommands.Add(command);
            }
        }

        private void notification_Tapped_1(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Navigator.Instance.NavigateTo(typeof(ApprovePage));
        }

        private void notification_PointerEntered_1(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            notification.Opacity = 0.8;
        }

        private void notification_PointerExited_1(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            notification.Opacity = 0.6;
        }
    }
}

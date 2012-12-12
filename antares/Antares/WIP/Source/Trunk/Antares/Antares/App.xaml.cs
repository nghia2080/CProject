using Antares.VIEWs;
using AntaresShell.Common;
using AntaresShell.IO;
using AntaresShell.Localization;
using AntaresShell.NavigatorProvider;
using System.Threading.Tasks;
using Repository;
using Repository.Repositories;
using Telerik.UI.Xaml.Controls.Input;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.Search;
using Windows.UI.Xaml;
using System.Globalization;

// The Grid App template is documented at http://go.microsoft.com/fwlink/?LinkId=234226

namespace Antares
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App
    {
        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            await EnsureRootFrame(args.PreviousExecutionState);
        }

        private void OverwriteCalendarForVietNam()
        {
            if (CultureInfo.CurrentCulture.Name.ToLowerInvariant() != "vi") return;
            CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames = new[] { "CN", "Hai", "Ba", "Tư", "Năm", "Sáu", "Bảy" };
            CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames = new[] 
                                                                                { "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", 
                                                                                  "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", 
                                                                                  "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12", "" };
        }

        private async Task EnsureRootFrame(ApplicationExecutionState state)
        {
            if (Window.Current.Content == null)
            {
                OverwriteCalendarForVietNam();

                LiveTile.LiveTileManager.Instance.Start();

                await AntaresBaseFolder.Instance.InitializeBaseFolder();
                LanguageProvider.InitDisplayResources();
                InputLocalizationManager.Instance.UserResourceMap =
                    ResourceManager.Current.MainResourceMap.GetSubtree("Resources");


                // Create a Frame to act as the navigation context and navigate to the first page
                // Place the frame in the current Window
                var mainPage = new MainPage();
                Window.Current.Content = mainPage;
                Window.Current.Activate();
                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(mainPage.RootFrame, "AppFrame");
                WeatherRepository.Instance.GetWeatherInfoAsync(); //TODO: What is the purpose of this call?
                Navigator.Instance.SetRootFrame(mainPage.RootFrame);

                Navigator.Instance.NavigateTo(typeof(TimelineWeekPage));

                SearchPane.GetForCurrentView().SearchHistoryEnabled = true;

                if (state == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await SuspensionManager.RestoreAsync();
                    }
                    catch (SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }
            }
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            SearchPane.GetForCurrentView().QuerySubmitted +=
               (sender, queryArgs) => Navigator.Instance.NavigateTo(typeof(SearchContractPage), queryArgs.QueryText);

            base.OnWindowCreated(args);
        }

        /// <summary>
        /// Invoked when the application is activated to display search results.
        /// </summary>
        /// <param name="args">Details about the activation request.</param>
        protected async override void OnSearchActivated(SearchActivatedEventArgs args)
        {
            // TODO: Register the Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().QuerySubmitted
            // event in OnWindowCreated to speed up searches once the application is already running

            // If the Window isn't already using Frame navigation, insert our own Frame

            // If the app does not contain a top-level frame, it is possible that this 
            // is the initial launch of the app. Typically this method and OnLaunched 
            // in App.xaml.cs can call a common method.

            await EnsureRootFrame(args.PreviousExecutionState);
            base.OnSearchActivated(args);

            Navigator.Instance.NavigateTo(typeof(SearchContractPage), args.QueryText);
        }
    }
}

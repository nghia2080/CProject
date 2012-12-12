using AntaresShell.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AntaresShell.NavigatorProvider;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Antares.VIEWMODELs;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Antares.VIEWs
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ApprovePage 
    {
        public ApprovePage()
        {
            this.InitializeComponent();
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
            Navigator.Instance.HideApproveNotificator();

        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            Navigator.Instance.ShowApproveNotificator();
        }

        private void FlipView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if(FlipView == null)
            {
                return;
            }

            if(FlipView.SelectedIndex == 0)
            {
                TaskTxt.Opacity = 1;
                ProjectTxt.Opacity = 0.4;
            }
            else
            {
                TaskTxt.Opacity = 0.4;
                ProjectTxt.Opacity = 1;
            }
        }

        private void TaskTxt_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            FlipView.SelectedIndex = 0;
        }

        private void ProjectTxt_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            FlipView.SelectedIndex = 1;
        }

        private void TaskGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ((ApproveViewModel) DataContext).SelectedTasks = TaskGrid.SelectedItems;
        }

        private void ProjectGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ((ApproveViewModel)DataContext).SelectedProjects = ProjectGrid.SelectedItems;
        }
    }
}

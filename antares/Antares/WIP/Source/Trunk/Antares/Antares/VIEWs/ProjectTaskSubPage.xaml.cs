using System.Collections.Generic;
using Antares.VIEWMODELs;
using System;
using Repository.MODELs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Antares.VIEWs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProjectTaskSubPage
    {
        public ProjectTaskSubPage()
        {
            NavigationCacheMode = NavigationCacheMode.Enabled;
            InitializeComponent();
            SemanticControl.ViewChangeCompleted += SemanticControl_ViewChangeCompleted;
            TaskGridView.Loaded += TaskGridView_Loaded;
        }

        private ScrollViewer scroll = null;
        void TaskGridView_Loaded(object sender, RoutedEventArgs e)
        {
            scroll = FindVisualChildren<ScrollViewer>(TaskGridView);
        }

        void SemanticControl_ViewChangeCompleted(object sender, Windows.UI.Xaml.Controls.SemanticZoomViewChangedEventArgs e)
        {
             if(!e.IsSourceZoomedInView)
             {
                 if(scroll!=null)
                 {
                     var group = e.SourceItem.Item as GroupCollection;
                     if(@group != null && (@group.GroupTasks!=null && @group.GroupTasks.Count!=0))
                     {
                         var itemIndex = TaskGridView.Items.IndexOf(group.GroupTasks[0]);
                         scroll.ScrollToHorizontalOffset(itemIndex);
                     }
                     
                 }
             }
        }

        private T FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
            {
                return default(T);
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T)
                {
                    return (T)child;
                }

                return FindVisualChildren<T>(child);
            }

            return default(T);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataContext = new ProjectSubtaskViewModel(Convert.ToInt32(e.Parameter));
        }
    }
}

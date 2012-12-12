using System;
using Antares.VIEWMODELs;
using AntaresShell.Common;
using AntaresShell.Common.MessageTemplates;
using AntaresShell.Logger;
using AntaresShell.NavigatorProvider;
using Repository.MODELs;
using Repository.Sync;
using Telerik.UI.Xaml.Controls.Input;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Antares.VIEWs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddTask
    {
        public AddTask()
        {

            InitializeComponent();
            cancelButton.Tapped += CancelButtonOnTapped;

            CboCategory.LayoutUpdated += CboCategory_LayoutUpdated;
            CboProject.LayoutUpdated += CboProject_LayoutUpdated;
            CboAssignee.LayoutUpdated += CboAssignee_LayoutUpdated;

            CboCategory.DropDownOpened += CboCategory_DropDownOpened;
            CboProject.DropDownOpened += CboProjectOnDropDownOpened;
            CboAssignee.DropDownOpened += CboAssigneeOnDropDownOpened;
        }

        private void CboAssigneeOnDropDownOpened(object sender, object o)
        {
            _cacheAssignee = null;
        }

        private void CboProjectOnDropDownOpened(object sender, object o)
        {
            _cacheProject = null;
        }

        void CboCategory_DropDownOpened(object sender, object e)
        {
            _cacheCategory = null;
        }

        private void CancelButtonOnTapped(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            Navigator.Instance.HideTimelinePopup();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Allow to focus on the hold page a.k.a not show focus border on any other control.
            IsTabStop = true;

            var stTime = DateTime.Now.TimeOfDay;
            DataContext = e.Parameter == null ? new AddTaskViewModel
            {
                Information = new TaskModel()
                                  {
                    ID = -1,
                    StartDate = DateTime.Now.Date.ToString(),
                    ProjectID = -1,
                    StartTime = (int)stTime.TotalMinutes,
                    EndDate = DateTime.Now.Date.ToString(),
                    EndTime = (int)stTime.TotalMinutes + 60,
                }
            }
            : new AddTaskViewModel((TaskModel)e.Parameter);

        }

        private ItemCollection _cacheAssignee;
        private ItemCollection _cacheProject;
        private ItemCollection _cacheCategory;

        void CboAssignee_LayoutUpdated(object sender, object e)
        {
            if (CboAssignee.Items != null && CboAssignee.Items.Count > 0 && CboAssignee.Items != _cacheAssignee)
            {
                _cacheAssignee = CboAssignee.Items;
                Messenger.Instance.Notify(RebindCbo.RebindMember);
            }
        }

        void CboProject_LayoutUpdated(object sender, object e)
        {
            if (CboProject.Items != null && CboProject.Items.Count > 0 && CboProject.Items != _cacheProject)
            {
                _cacheProject = CboProject.Items;
                Messenger.Instance.Notify(RebindCbo.RebindProject);
            }
        }

        void CboCategory_LayoutUpdated(object sender, object e)
        {
            if (CboCategory.Items != null && CboCategory.Items.Count > 0 && CboCategory.Items != _cacheCategory)
            {
                _cacheCategory = CboCategory.Items;
                Messenger.Instance.Notify(RebindCbo.RebindCategory);
            }
        }

        private void backButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Navigator.Instance.HideTimelinePopup();
            GlobalData.TemporaryTask = null;
        }

        private void PageKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Tab)
            {
                IsTabStop = false;
                // backButton.IsTabStop = true;
            }
            else if (e.Key == VirtualKey.Escape)
            {
                Navigator.Instance.HideTimelinePopup();
            }
        }

        private void Period_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var splitTime = ((AddTaskViewModel)DataContext).Information.StartTime;
            UpdateEndtime();
        }

        private void StTime_OnValueChanged(object sender, EventArgs e)
        {
            UpdateEndtime();
        }

        private void UpdateEndtime()
        {
            var splitTime = ((AddTaskViewModel) DataContext).Information.StartTime;

            if (splitTime != null)
            {
                if (!string.IsNullOrEmpty(Period.Text))
                {
                    try
                    {
                        splitTime += (int) ((Convert.ToDouble(Period.Text))*60);
                    }
                    catch (Exception ex)
                    {
                        LogManager.Instance.LogInfo("Cannot edit task end time " + ex);
                    }
                }

                try
                {
                    ((AddTaskViewModel) DataContext).Information.EndTime = (int) splitTime;
                }
                catch (Exception ex)
                {
                    LogManager.Instance.LogInfo("Cannot edit task end time " + ex);
                }
            }
        }

        private void Period_OnGotFocus(object sender, RoutedEventArgs e)
        {
            Period.Text = "";
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Repository.MODELs;
using Repository.Repositories;
using Repository.Sync;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Globalization;
using AntaresShell.Localization;
using AntaresShell.NavigatorProvider;
using AntaresShell.Common;
using AntaresShell.Common.MessageTemplates;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Antares.VIEWs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserInfoQuickView : Page
    {
        public UserInfoQuickView()
        {
            this.InitializeComponent();
            this.Width = Window.Current.Bounds.Width;
            this.Height = Window.Current.Bounds.Height;
        }

        private UserModel _model;
        private ProjectMemberContrainModel _contrain;

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var temp = e.Parameter as ProjectMemberContrainModel;
            if (temp != null)
            {
                _contrain = new ProjectMemberContrainModel(temp);

                removeButton.IsEnabled = ProjectMemberRepository.Instance.IsManager(_contrain.ProjectID);
                favorButton.IsEnabled = removeButton.IsEnabled;
                unfavorButton.IsEnabled = removeButton.IsEnabled;

                if (string.IsNullOrEmpty(_contrain.Role))   // Validate PM
                {
                    // Validate number tasks.
                    var taskAll = await TaskRepository.Instance.GetAllTasksForProject(_contrain.ProjectID);

                    var query = from task in taskAll
                                where task.UserID == _contrain.UserID
                                select task;

                    if (query.Any())
                    {
                        if (_contrain.IsActive)
                        {
                            unfavorButton.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            favorButton.Visibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        removeButton.Visibility = Visibility.Visible;
                    }
                }

                _model = await UserInformationRepository.Instance.GetUser(_contrain.UserID);

                if (_model != null)
                {
                    Username.Text = _model.Username;

                    Birthday.Text = string.IsNullOrEmpty(_model.DOB)
                                        ? LanguageProvider.Resource["NotAvailable"]
                                        : Convert.ToDateTime(_model.DOB).ToString(
                                            CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);

                    Telephone.Text = string.IsNullOrEmpty(_model.Phone)
                                         ? LanguageProvider.Resource["NotAvailable"]
                                         : _model.Phone;

                    Email.Text = string.IsNullOrEmpty(_model.Email)
                                     ? LanguageProvider.Resource["NotAvailable"]
                                     : _model.Email;
                }
            }
        }

        private async void removeButton_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            if (_model == null)
            {
                return;
            }

            // Create the message dialog and set its content and title
            var messageDialog = new MessageDialog(LanguageProvider.Resource["Msg_DeleteProjectMember"]);

            // Add commands and set their callbacks
            messageDialog.Commands.Add(new UICommand(LanguageProvider.Resource["Msg_Yes"], command => DeleteCommand()));

            messageDialog.Commands.Add(new UICommand(LanguageProvider.Resource["Msg_No"], command => { }));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();
        }

        private async void DeleteCommand()
        {
            Navigator.Instance.MainProgressBar.Visibility = Visibility.Visible;

            var memberList = await ProjectMemberRepository.Instance.GetAllProjectsMember(GlobalData.SelectedProjects);
            if (memberList != null)
            {
                var member = memberList.FirstOrDefault(p => p.UserID == _model.UserID);
                if (member != null)
                {
                    var res = await ProjectMemberRepository.Instance.DeleteMember(member.ID);
                    Navigator.Instance.DisplayStatus(res.IsSuccessStatusCode
                                                         ? ConnectionStatus.Done
                                                         : ConnectionStatus.Error);
                    if (res.IsSuccessStatusCode)
                    {
                        Messenger.Instance.Notify(RefreshMember.Refresh);
                        Navigator.Instance.HideTimelinePopup();
                    }

                }
            }

            Navigator.Instance.MainProgressBar.Visibility = Visibility.Collapsed;
        }

        private void unfavorButton_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            _contrain.IsActive = false;
            UpdateData(_contrain);
        }

        private void favorButton_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            _contrain.IsActive = true;
            UpdateData(_contrain);
        }

        private async void UpdateData(ProjectMemberContrainModel data)
        {
            Navigator.Instance.MainProgressBar.Visibility = Visibility.Visible;
            if (data == null)
            {
                return;
            }

            var res = await ProjectMemberRepository.Instance.EditContrain(data);
            Navigator.Instance.DisplayStatus(res.IsSuccessStatusCode
                                                         ? ConnectionStatus.Done
                                                         : ConnectionStatus.Error);

            if (res.IsSuccessStatusCode)
            {
                Messenger.Instance.Notify(RefreshMember.Refresh);
                Navigator.Instance.HideTimelinePopup();
            }

            Navigator.Instance.MainProgressBar.Visibility = Visibility.Collapsed;
        }

        private void cancelButton_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            Navigator.Instance.HideTimelinePopup();
        }

    }
}

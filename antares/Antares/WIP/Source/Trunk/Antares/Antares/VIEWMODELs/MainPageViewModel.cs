using Antares.VIEWs;
using AntaresShell.BaseClasses;
using System;
using System.Windows.Input;
using AntaresShell.Common;
using AntaresShell.Common.MessageTemplates;
using AntaresShell.NavigatorProvider;
using Windows.UI.Xaml;
using Repository.MODELs;
using Windows.UI.Popups;

namespace Antares.VIEWMODELs
{
    public class MainPageViewModel : ViewModelBase
    {
        private ICommand _addNewTaskCommand;
        public ICommand AddNewTaskCommand
        {
            get { return _addNewTaskCommand ?? (_addNewTaskCommand = new RelayCommand(AddNewTaskCmd)); }
        }

        private ICommand _addNewProjectCommand;
        public ICommand AddNewProjectCommand
        {
            get { return _addNewProjectCommand ?? (_addNewProjectCommand = new RelayCommand(AddNewProjectCmd)); }
        }

        //private ICommand _viewDetailTaskCommand;
        //public ICommand ViewDetailTaskCommand
        //{
        //    get { return _viewDetailTaskCommand ?? (_viewDetailTaskCommand = new RelayCommand(ViewDetailTaskCmd)); }
        //}

        //private ICommand _viewDetailProjectCommand;
        //public ICommand ViewDetailProjectCommand
        //{
        //    get { return _viewDetailProjectCommand ?? (_viewDetailProjectCommand = new RelayCommand(ViewDetailProjectCmd)); }
        //}

        private ICommand _deleteTaskCommand;
        public ICommand DeleteTaskCommand
        {
            get { return _deleteTaskCommand ?? (_deleteTaskCommand = new RelayCommand(DeleteTaskCmd)); }
        }

        private ICommand _deleteProjectCommand;
        public ICommand DeleteProjectCommand
        {
            get { return _deleteProjectCommand ?? (_deleteProjectCommand = new RelayCommand(DeleteProjectCmd)); }
        }

        private ICommand _refreshCommand;
        public ICommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new RelayCommand(RefreshCmd)); }
        }

        private ICommand _gohomeCommand;
        public ICommand GoHomeCommand
        {
            get { return _gohomeCommand ?? (_gohomeCommand = new RelayCommand(GoHomeCmd)); }
        }
        private ICommand _basicmonthCommand;
        public ICommand BasicMonthCommand
        {
            get { return _basicmonthCommand ?? (_basicmonthCommand = new RelayCommand(BasicMonthCmd)); }
        }

        private ICommand _infoCommand;
        public ICommand InfoCommand
        {
            get { return _infoCommand ?? (_infoCommand = new RelayCommand(InfoCmd)); }
        }


        private ICommand _projectCommand;
        public ICommand ProjectCommand
        {
            get { return _projectCommand ?? (_projectCommand = new RelayCommand(ProjectCmd)); }
        }
        private void AddNewTaskCmd(object obj)
        {
            Navigator.Instance.ShowTimelinePopup(typeof(AddTask));
            Navigator.Instance.RootFrameOfPopup.Focus(FocusState.Programmatic);
            Navigator.Instance.BottomAppBar.IsOpen = false;
        }

        private void AddNewProjectCmd(object obj)
        {
            Navigator.Instance.NavigateTo(typeof (ProjectManagerPage), "#2:-1");
        }

        private void RefreshCmd(object obj)
        {

        }

        private void GoHomeCmd(object obj)
        {
            Navigator.Instance.NavigateTo(typeof(TimelineWeekPage));
        }

        private void BasicMonthCmd(object obj)
        {
            Navigator.Instance.NavigateTo(typeof(BasicMonthPage));
        }
        private void InfoCmd(object obj)
        {
            Navigator.Instance.NavigateTo(typeof(UserInfoPage));
        }

        private void ProjectCmd(object obj)
        {
            Navigator.Instance.NavigateTo(typeof(ProjectManagerPage), "#1:");
        }

        //private void ViewDetailTaskCmd(object obj)
        //{

        //}

        //private void ViewDetailProjectCmd(object obj)
        //{

        //}

        private void DeleteTaskCmd(object obj)
        {

        }

        private async void DeleteProjectCmd(object obj)
        {
            // Create the message dialog and set its content and title
            var messageDialog = new MessageDialog("Do you want to delete this project ?", "Metro Calendar");

            // Add commands and set their callbacks
            messageDialog.Commands.Add(new UICommand("Yes", command => Messenger.Instance.Notify(DeleteProjectMsg.Yes)));

            messageDialog.Commands.Add(new UICommand("Cancel", command => Messenger.Instance.Notify(DeleteProjectMsg.Cancel)));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();
        }
    }
}

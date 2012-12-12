using System;
using AntaresShell.Localization;
using AntaresShell.NavigatorProvider;
using System.Collections.Generic;
using Repository.Sync;
using Windows.Storage;
using Windows.UI;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using AntaresShell.Common;
using AntaresShell.Common.MessageTemplates;
using Repository.Repositories;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Antares.VIEWs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProjectManagerPage
    {
        public ProjectManagerPage()
        {
            InitializeComponent();

            Messenger.Instance.Register<EnableProject>(EnableProject);
        }

        private async void EnableProject(object obj)
        {
            var id = GlobalData.SelectedProjects;

            if (id != -1)
            {
                var projects = await ProjectRepository.Instance.GetAllProjects();

                var pro = projects.FirstOrDefault(p => p.ID == id);

                ProjectBtn.Content = pro.Name.ToUpper();

                ProjectBtn.Visibility = Visibility.Visible;
                TaskBtn.Visibility = Visibility.Visible;
                indi1.Visibility = Visibility.Visible;

            }
        }

        protected override void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
            pageTitle.Text = (ApplicationData.Current.LocalSettings.Values["UserFirstName"] + string.Empty == string.Empty)
            ? LanguageProvider.Resource["MainTitle_4"]
            : LanguageProvider.Resource["MainTitle_3"].Replace("[abc]", ApplicationData.Current.LocalSettings.Values["UserFirstName"] + string.Empty);

            EnableProject(null);

            var param = navigationParameter as string;
            if (param == null)
            {
                ProjectFrame.Navigate(typeof(ProjectOverViewSubPage));
                SetHighLight(OverviewBtn);
            }
            else if (param.Contains("#2:"))
            {
                ProjectFrame.Navigate(typeof(ProjectInformationSubPage), param.Split(new[] { "#2:" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                SetHighLight(ProjectBtn);
            }
            else if (navigationParameter.ToString().Contains("#3:"))
            {
                ProjectFrame.Navigate(typeof(ProjectTaskSubPage), param.Split(new[] { "#3:" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                SetHighLight(TaskBtn);
            }
            else
            {
                if (GlobalData.SelectedProjects != -1)
                {
                    ProjectFrame.Navigate(typeof(ProjectTaskSubPage), GlobalData.SelectedProjects);
                    SetHighLight(TaskBtn);
                }
                else
                {
                    ProjectFrame.Navigate(typeof(ProjectOverViewSubPage));
                    SetHighLight(OverviewBtn);
                } 
            }
        }

        private void OverviewBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Navigator.Instance.NavigateTo(typeof(ProjectManagerPage));
        }

        private void ProjectBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Navigator.Instance.NavigateTo(typeof(ProjectManagerPage), "#2:" + GlobalData.SelectedProjects);
        }

        private void Taskbtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Navigator.Instance.NavigateTo(typeof(ProjectManagerPage), "#3:" + GlobalData.SelectedProjects);
        }

        private void ViewBtn_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Navigator.Instance.NavigateTo(typeof(TimelineWeekPage));
        }

        private void SetHighLight(HyperlinkButton highlighted)
        {
            OverviewBtn.Foreground = new SolidColorBrush(Colors.Black);
            ProjectBtn.Foreground = new SolidColorBrush(Colors.Black);
            TaskBtn.Foreground = new SolidColorBrush(Colors.Black);


            highlighted.Foreground = new SolidColorBrush(Colors.Purple);
        }
    }
}

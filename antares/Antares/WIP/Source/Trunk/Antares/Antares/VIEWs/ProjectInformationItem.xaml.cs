using AntaresShell.Common;
using AntaresShell.Common.MessageTemplates;
using AntaresShell.Localization;
using AntaresShell.NavigatorProvider;
using Repository.MODELs;
using Repository.Sync;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Antares.VIEWs
{
    public sealed partial class ProjectInformationItem
    {
        public ProjectInformationItem()
        {
            InitializeComponent();
            Task.Text = LanguageProvider.Resource["Tasks"];
        }

        private void Grid_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            var info = DataContext as ProjectInformationModel;
            Navigator.Instance.NavigateTo(typeof(ProjectManagerPage), "#2:" + info.ID);
            GlobalData.SelectedProjects = info.ID;
            Messenger.Instance.Notify(EnableProject.Enabled);
        }
    }
}

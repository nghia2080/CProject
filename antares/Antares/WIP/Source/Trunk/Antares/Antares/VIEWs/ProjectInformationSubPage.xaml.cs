using System;
using Antares.VIEWMODELs;
using AntaresShell.Common;
using AntaresShell.Common.MessageTemplates;
using AntaresShell.Localization;
using AntaresShell.NavigatorProvider;
using Repository.MODELs;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Antares.VIEWs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProjectInformationSubPage
    {
        public ProjectInformationSubPage()
        {
            NavigationCacheMode = NavigationCacheMode.Enabled;
            InitializeComponent();
            defaultText = LanguageProvider.Resource["Prj_EnterUsername"];
            //TODO: dangerous
            Messenger.Instance.Register<HideSaveBtn>(o => { Save.Visibility = Visibility.Collapsed; });
            Messenger.Instance.Register<MemberProgressRing>(o =>
                                                                {
                                                                    var type = (MemberProgressRing)o;
                                                                    pgrRing.Visibility = type == MemberProgressRing.Hide ? Visibility.Collapsed : Visibility.Visible;
                                                                });

            Messenger.Instance.Register<HighlightTextBox>(o =>
                                                              {
                                                                  var type = (HighlightTextBox)o;
                                                                  Candidate.Foreground = type == HighlightTextBox.Fail ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);
                                                              });

            Messenger.Instance.Register<UpdateProject>(o =>
                                                               {
                                                                   HidememberArea.Visibility = Visibility.Collapsed;
                                                                   Delete.Visibility = Visibility.Visible;
                                                               });
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataContext = e.Parameter == null ? new ProjectInformationViewModel(-1) : new ProjectInformationViewModel(Convert.ToInt32(e.Parameter));

            HidememberArea.Visibility = (e.Parameter == null || Convert.ToInt32(e.Parameter) == -1) ? Visibility.Visible : Visibility.Collapsed;
            Delete.Visibility = (e.Parameter == null || Convert.ToInt32(e.Parameter) == -1) ? Visibility.Collapsed : Visibility.Visible;

            Save.Visibility = Visibility.Collapsed;
        }

        private void TextBox_TextChanged_1(object sender, Windows.UI.Xaml.Controls.TextChangedEventArgs e)
        {
            Save.Visibility = Visibility.Visible;
        }

        private void ComboBox_SelectionChanged_1(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            Save.Visibility = Visibility.Visible;
        }

        private void RadDatePicker_ValueChanged_1(object sender, System.EventArgs e)
        {
            Save.Visibility = Visibility.Visible;
        }

        private void Candidate_TextChanged_1(object sender, Windows.UI.Xaml.Controls.TextChangedEventArgs e)
        {
            Candidate.Foreground = new SolidColorBrush(Color.FromArgb(100, 34, 34, 34));

        }
        private string defaultText;
        private void Candidate_GotFocus(object sender, RoutedEventArgs e)
        {
            Candidate.Text = Candidate.Text == defaultText ? string.Empty : Candidate.Text;
        }

        private void Candidate_LostFocus(object sender, RoutedEventArgs e)
        {
            Candidate.Text = Candidate.Text == string.Empty ? defaultText : Candidate.Text;
        }

        private void StackPanel_DoubleTapped_1(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            var stack = sender as StackPanel;
            var model = stack.DataContext as ProjectMemberContrainModel;
            Navigator.Instance.ShowTimelinePopup(typeof(UserInfoQuickView), model);
        }


    }
}

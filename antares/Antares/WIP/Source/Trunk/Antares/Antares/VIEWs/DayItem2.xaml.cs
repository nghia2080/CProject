using AntaresShell.Localization;
using AntaresShell.NavigatorProvider;
using Repository.MODELs;
using Repository.Sync;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Antares.VIEWs
{
    public sealed partial class DayItem2
    {
        public DayItem2()
        {
            InitializeComponent();
            Width = ((Window.Current.Bounds.Width - 40) / 7) - 10;
            Height = ((Window.Current.Bounds.Height - 140 - 50 - 100) / GlobalData.NumberOfRows) - 15;
            tasktxt.Text = LanguageProvider.Resource["Tasks"];
            //Opacity = (((DayItemModel) DataContext).Today.Month == GlobalData.SelectedMonthIndex) ? 0.8 : 0.3;
        }

        private void DayGrid_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            Navigator.Instance.NavigateTo(typeof(TimelineDayPage), ((DayItemModel)DataContext).Today );
        }
    }
}

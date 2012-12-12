using AntaresShell.NavigatorProvider;
using Repository.MODELs;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Antares.VIEWs
{
    public sealed partial class DayItem 
    {
        public DayItem()
        {
            InitializeComponent();
        }

        private void DayGrid_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            Navigator.Instance.NavigateTo(typeof(TimelineDayPage), ((DayItemModel)DataContext).Today);
        }
    }
}

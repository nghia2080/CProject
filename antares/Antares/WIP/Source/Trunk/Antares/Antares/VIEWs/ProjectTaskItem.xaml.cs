using AntaresShell.NavigatorProvider;
using Repository.MODELs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Antares.VIEWs
{
    public sealed partial class ProjectTaskItem
    {
        public ProjectTaskItem()
        {
            InitializeComponent();
            //PointerEntered += ProjectTaskItem_PointerEntered;
            //PointerExited += ProjectTaskItem_PointerExited;
        }

        //void ProjectTaskItem_PointerExited(object sender, PointerRoutedEventArgs e)
        //{
        //    RowDefinitions[1].Height = new GridLength(5.5, GridUnitType.Star);
        //    RowDefinitions[2].Height = new GridLength(1.5, GridUnitType.Star);
        //}

        //void ProjectTaskItem_PointerEntered(object sender, PointerRoutedEventArgs e)
        //{
        //    RowDefinitions[1].Height = new GridLength(2.5, GridUnitType.Star);
        //    RowDefinitions[2].Height = new GridLength(4.5, GridUnitType.Star);
        //}

        private void Grid_DoubleTapped_1(object sender, DoubleTappedRoutedEventArgs e)
        {
            Navigator.Instance.ShowTimelinePopup(typeof(AddTask), this.DataContext);
        }
    }
}

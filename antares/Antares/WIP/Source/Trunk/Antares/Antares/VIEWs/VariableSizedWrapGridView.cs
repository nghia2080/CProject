using Repository.MODELs;
using Windows.UI.Xaml.Controls;

namespace Antares.VIEWs
{
    public class VariableSizedWrapGridView : GridView
    {
        protected override void PrepareContainerForItemOverride(Windows.UI.Xaml.DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (item != null)
            {
                var span = 1;

                var projectInformationModel = item as ProjectInformationModel;
                if (projectInformationModel != null)
                {
                    span = projectInformationModel.Status == 1 ? 2 : 1;
                }

                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, span);
                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, span);
            }
        }
    }
}

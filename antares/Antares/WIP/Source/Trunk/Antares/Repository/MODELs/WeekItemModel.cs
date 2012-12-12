using System.Collections.ObjectModel;
using AntaresShell.BaseClasses;
using System.Collections.Generic;

namespace Repository.MODELs
{
    public class WeekItemModel : BindableBase
    {
        private ObservableCollection<DayItemModel> _days;
        public ObservableCollection<DayItemModel> Days
        {
            get { return _days; }
            set { SetProperty(ref _days, value); }
        }

        private DayItemModel _selectedItem;
        public DayItemModel SelectedItem
        {
            get { return _selectedItem ?? (_selectedItem = _days[0]); }
            set { SetProperty(ref _selectedItem, value); }
        }
    }
}

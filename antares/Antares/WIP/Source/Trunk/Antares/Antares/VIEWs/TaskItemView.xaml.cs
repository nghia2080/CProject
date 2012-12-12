using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AntaresShell.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Repository.MODELs;
using AntaresShell.BaseClasses;
using Repository.Repositories;
using AntaresShell.NavigatorProvider;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Antares.VIEWs
{
    public sealed partial class TaskItemView
    {
        private TaskModel _dataModel;
        public TaskModel DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }

        public TaskItemView() : this(null)
        {
        }

        public TaskItemView(TaskModel dataModel)
        {
            InitializeComponent();
            Tapped += TaskItemView_Tapped;
            DoubleTapped += OnDoubleTapped;
            if(dataModel!=null)
            {
                DataModel = dataModel;
            }

        }

        private void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs doubleTappedRoutedEventArgs)
        {
            Navigator.Instance.ShowTimelinePopup(typeof(AddTask), new TaskModel(DataModel));
        }

        void TaskItemView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Messenger.Instance.Notify(TaskItemView);
        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {
            DataContext = this;
        }

        private void StackPanel_PointerEntered_1(object sender, PointerRoutedEventArgs e)
        {
        }


        private void ItemImage_PointerPressed_1(object sender, PointerRoutedEventArgs e)
        {
            ItemImage.Margin = new Thickness(2);
        }

        private void ItemImage_OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ItemImage.Margin = new Thickness(0);
        }

        private void UserControl_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            Messenger.Instance.Notify(this);
        }

        internal event PropertyChangedEventHandler PropertyChanged;

        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            if (propertyName != null) OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

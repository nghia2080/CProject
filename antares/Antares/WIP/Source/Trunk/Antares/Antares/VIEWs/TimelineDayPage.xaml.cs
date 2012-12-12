using Antares.Converters;
using AntaresShell.Common;
using Repository.MODELs;
using Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Repository.Sync;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Antares.VIEWs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TimelineDayPage
    {
        private List<TaskItemView> _currentTaskList;
        private List<TaskItemView> _newTaskList;
        private double _width = 70.375;
        private DateTime _currentDate;

        public TimelineDayPage()
        {
            InitializeComponent();
            Messenger.Instance.Register<TaskItemView>(UpdateDescription);
            TaskRepository.Instance.Tasks.CollectionChanged += Tasks_CollectionChanged;
        }

        void Tasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // e.NewItems = item moi add. Cast ve item model
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                if (_newTaskList == null)
                {
                    _newTaskList = new List<TaskItemView>();
                }

                var taskModel = (TaskModel)e.NewItems[0];

                if (taskModel.UserID != GlobalData.MyUserID)
                {
                    return;
                }

                switch (taskModel.RepeatType)
                {
                    case 0:
                        if (Convert.ToDateTime(taskModel.StartDate).Day == _currentDate.Day
                       && Convert.ToDateTime(taskModel.StartDate).Month == _currentDate.Month
                       && Convert.ToDateTime(taskModel.StartDate).Year == _currentDate.Year)
                        {
                            _newTaskList.Add(new TaskItemView(taskModel));
                        }
                        break;

                    //daily
                    case 1:
                        _newTaskList.Add(new TaskItemView(taskModel));
                        break;

                    //weekly
                    case 2:
                        if (_currentDate.DayOfWeek == Convert.ToDateTime(taskModel.StartDate).DayOfWeek)
                        {
                            _newTaskList.Add(new TaskItemView(taskModel));
                        }
                        break;

                    // monthly
                    case 3:
                        if (_currentDate.Day == Convert.ToDateTime(taskModel.StartDate).Day)
                        {
                            _newTaskList.Add(new TaskItemView(taskModel));
                        }
                        break;

                    // yearly
                    case 4:
                        if (_currentDate.Day == Convert.ToDateTime(taskModel.StartDate).Day && _currentDate.Month == Convert.ToDateTime(taskModel.StartDate).Month)
                        {
                            _newTaskList.Add(new TaskItemView(taskModel));
                        }
                        break;
                }
            }

            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (var item in e.OldItems)
                {
                    if (_currentTaskList != null)
                    {
                        var view = _currentTaskList.FirstOrDefault(p => p.DataModel.ID == ((TaskModel)item).ID);
                        if (view != null)
                        {
                            _currentTaskList.Remove(view);
                            TimeGrid.Children.Remove(view);
                        }
                    }
                }
            }

            UpdateTimeline();
        }

        private void UpdateTimeline()
        {
            try
            {
                foreach (var task in _newTaskList)
                {
                    task.Margin = new Thickness(GetLocationBaseTime(task.DataModel.StartTime) + 30, 0, 0, 0);
                    task.Width = _width;
                    task.HorizontalAlignment = HorizontalAlignment.Left;
                    task.VerticalAlignment = VerticalAlignment.Bottom;
                    task.SetValue(Grid.RowProperty, 0);
                    task.SetValue(Grid.RowSpanProperty, 2);
                    task.Height = GetItemHeight(task.DataModel.Priority);
                    task.Transitions = new TransitionCollection {new AddDeleteThemeTransition()};
                    if (_currentTaskList == null)
                    {
                        _currentTaskList = new List<TaskItemView>();
                    }

                    if (!_currentTaskList.Contains(task))
                    {
                        _currentTaskList.Add(task);
                        TimeGrid.Children.Add(task);
                    }
                }

                _newTaskList.Clear();
            }
            catch
            {
            }
        }

        IntToShortTimeConverter intToTimeConverter = new IntToShortTimeConverter();
        private void UpdateDescription(object taskItem)
        {   GridInfo.Visibility =  Visibility.Visible;
            var task = taskItem as TaskItemView;
            if (task != null)
            {
                TaskDetail.Text = task.DataModel.Description;
                TaskTitle.Text = task.DataModel.Name;
                TaskStart.Text = intToTimeConverter.Convert(task.DataModel.StartTime, null, null, null) + " " + Convert.ToDateTime(task.DataModel.StartDate).ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
                TaskEnd.Text = intToTimeConverter.Convert(task.DataModel.EndTime, null, null, null) + " " + Convert.ToDateTime(task.DataModel.EndDate).ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            }
        }

        private double GetItemHeight(int dPriority)
        {
            switch (dPriority)
            {
                case 2:
                    return 240;
                case 1:
                    return 210;
                case 0:
                    return 180;
            }

            return 195;
        }


        private int GetLocationBaseTime(int? dTimeO)
        {
            var oneH = (TimeGrid.ActualWidth - 60) / 24;
            var location = (dTimeO - 30) * oneH / 60;
            return (int)location;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if (navigationParameter != null)
            {
                _currentDate = ((DateTime)navigationParameter);

                pageTitle.Text = _currentDate.ToString("dddd") + " " + _currentDate.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            }
            BindWeatherBackground(_currentDate);
        }

        private async void BindWeatherBackground(DateTime dt)
        {
            var imagePath = "../Assets/WeatherBackground/";
            var weather = await WeatherRepository.Instance.GetWeather(dt);
            if (weather == null)
            {
                imagePath += "unavailable.jpg";
            }
            else
            {
                switch (weather.WeatherType)
                {
                    case WeatherType.barelycloudy:
                    case WeatherType.morecloudy:
                        imagePath += "sunnyCloud.jpg";
                        break;
                    case WeatherType.mostcloudy:
                    case WeatherType.morecloudyday:
                        imagePath += "cloud.jpg";
                        break;
                    case WeatherType.hail:
                        imagePath += "hail.jpg";
                        break;
                    case WeatherType.lightrain:
                    case WeatherType.heavyrain:
                        imagePath += "rain.jpg";
                        break;
                    case WeatherType.heavysnow:
                    case WeatherType.lightsnow:
                        imagePath += "snow.jpg";
                        break;
                    case WeatherType.storm:
                        imagePath += "storm.jpg";
                        break;
                    case WeatherType.sunny:
                        imagePath += "sunny.jpg";
                        break;
                    case WeatherType.windy:
                        imagePath += "windy.jpg";
                        break;
                    default:
                        imagePath += "unavailable.jpg";
                        break;
                }
            }
            WeatherBackground.Source = new BitmapImage(new Uri(BaseUri, imagePath));
        }

        private async void BindingData(DateTime dt)
        {
            _newTaskList = new List<TaskItemView>();
            var listModels = await TaskRepository.Instance.GetTaskListFor(dt);
            foreach (var viewItem in listModels.Select(model => new TaskItemView(model)))
            {

                _newTaskList.Add(viewItem);
            }
            UpdateTimeline();
        }

        private void TimeGrid_Loaded(object sender, RoutedEventArgs e)
        {
            _width = (TimeGrid.ActualWidth - 60) / 24;
            BindingData(_currentDate);
        }
    }
}
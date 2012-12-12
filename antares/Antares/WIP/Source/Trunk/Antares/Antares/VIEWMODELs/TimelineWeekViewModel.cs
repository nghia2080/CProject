using System.Collections.ObjectModel;
using AntaresShell.BaseClasses;
using AntaresShell.Common;
using AntaresShell.Common.MessageTemplates;
using AntaresShell.Localization;
using Repository.MODELs;
using Repository.Repositories;
using Repository.Sync;
using System;
using System.Collections.Generic;

namespace Antares.VIEWMODELs
{
    public class TimelineWeekViewModel : ViewModelBase
    {
        private string _currentYear;
        public string CurrentYear
        {
            get { return _currentYear; }
            set { SetProperty(ref _currentYear, value); }
        }

        private WeekItemModel _selectedWeek;
        public WeekItemModel SelectedWeek
        {
            get { return _selectedWeek; }
            set
            {
                SetProperty(ref _selectedWeek, value);
                if (SelectedWeek != null)
                {
                    if (LanguageProvider.CurrentLanguage == "ja")
                    {
                        CurrentYear = SelectedWeek.Days[0].Today.ToString("MMMM") + " " + SelectedWeek.Days[0].Today.Year;
                    }
                    else
                    {
                        CurrentYear = SelectedWeek.Days[0].Today.ToString("MMM") + " " + SelectedWeek.Days[0].Today.Year;
                    }
                }
            }
        }

        private ObservableCollection<WeekItemModel> _sampleContent;
        public ObservableCollection<WeekItemModel> SampleContent
        {
            get { return _sampleContent; }
            set { SetProperty(ref _sampleContent, value); }
        }

        public TimelineWeekViewModel()
        {
            Messenger.Instance.Register<Refresh>(RefreshAll);
            BindWeek();

            Messenger.Instance.Notify(RebindCbo.RebindWeekGrid);

            Messenger.Instance.Register<UpdateTaskList>(Tasks_CollectionChanged);
        }             

        private void RefreshAll(object obj)
        {
            BindWeek();
            Messenger.Instance.Notify(RebindCbo.RebindWeekGrid);
        }

        async void Tasks_CollectionChanged(object sender)
        {
            foreach (var weekItemModel in SampleContent)
            {
                foreach (var dayModel in weekItemModel.Days)
                {
                    dayModel.TaskList = await TaskRepository.Instance.GetTaskListFor(dayModel.Today);
                }
            }
        }

        private void BindWeek()
        {
            DateTime dt;

            dt = DateTime.Now.Month > 2 ? new DateTime(DateTime.Now.Year, DateTime.Now.Month - 2, 1) : new DateTime(DateTime.Now.Year - 1, 12 - (3 - DateTime.Now.Month), 1);


            var firstMonday = GetFirstMondaySince(dt);
            var wims = new ObservableCollection<WeekItemModel>();
            var rd = new Random();
            for (int index = 0; index < 17; index++)
            {
                var dumb = new ObservableCollection<DayItemModel>();
                for (int j = 1; j < 8; j++)
                {
                    if (firstMonday == DateTime.Today)
                    {
                        GlobalData.SelectedWeekIndex = wims.Count;
                    }


                    dumb.Add(new DayItemModel { Today = firstMonday });
                    firstMonday = firstMonday.AddDays(1);
                }

                var weekItem = new WeekItemModel { Days = dumb };

                wims.Add(weekItem);
            }

            SampleContent = wims;

            foreach (var weekItemModel in wims)
            {
                foreach (var itemModel in weekItemModel.Days)
                {
                    BindWeather(itemModel);
                }
            }

            //SampleContent = new ObservableCollection<WeekItemModel>(wims);
        }

        private async void BindWeather(DayItemModel itemModel)
        {
            try
            {
                itemModel.WeatherModel = await WeatherRepository.Instance.GetWeather(itemModel.Today) ??
                                         new DayWeatherModel
                                             {
                                                 AverageTempuratureC = null,
                                                 AverageTempuratureF = null,
                                                 WeatherType = WeatherType.notavailable
                                             };
            }
            catch
            {
            }
        }

        private DateTime GetFirstMondaySince(DateTime dt)
        {
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return dt;

                case DayOfWeek.Tuesday:
                    return dt.AddDays(6);

                case DayOfWeek.Wednesday:
                    return dt.AddDays(5);

                case DayOfWeek.Thursday:
                    return dt.AddDays(4);

                case DayOfWeek.Friday:
                    return dt.AddDays(3);

                case DayOfWeek.Saturday:
                    return dt.AddDays(2);

                case DayOfWeek.Sunday:
                    return dt.AddDays(1);

                default:
                    return dt;
            }
        }
    }
}

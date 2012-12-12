using System.Collections.ObjectModel;
using AntaresShell.BaseClasses;
using System;
using System.Collections.Generic;
using AntaresShell.Utilities;
using AntaresShell.Localization;

namespace Repository.MODELs
{
    public class DayItemModel : BindableBase
    {
        private DateTime _today;
        public DateTime Today
        {
            get { return _today; }
            set
            {
                SetProperty(ref _today, value);

                if (LanguageProvider.CurrentLanguage.ToLower() == "vi")
                {
                    int month, year, day;
                    VietnameseCalendar.FromDateTime(value, out year, out month, out day);
                    LunarDay = day.ToString();
                    if (day == 1) LunarDay += "/" + VietnameseCalendar.GetMonthSpeechName(year, month);
                }
            }
        }

        private string _lunarDay;
        public string LunarDay
        {
            get { return _lunarDay; }
            set { SetProperty(ref _lunarDay, value); }
        }

        private DayWeatherModel _weatherModel;
        public DayWeatherModel WeatherModel
        {
            get { return _weatherModel; }
            set { SetProperty(ref _weatherModel, value); }
        }

        private ObservableCollection<TaskModel> _taskList;
        public ObservableCollection<TaskModel> TaskList
        {
            get { return _taskList ?? (_taskList = new ObservableCollection<TaskModel>()); }
            set { SetProperty(ref _taskList, value); }
        }
    }
}

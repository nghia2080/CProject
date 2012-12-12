using AntaresShell.BaseClasses;
using AntaresShell.Common;
using AntaresShell.Common.MessageTemplates;
using Repository.MODELs;
using System;
using System.Collections.Generic;
using Repository.Repositories;
using Repository.Sync;
using Windows.Globalization;
using DayOfWeek = System.DayOfWeek;
using System.Windows.Input;

namespace Antares.VIEWMODELs
{
    public class BasicMonthViewModel : ViewModelBase
    {
        private string _currentMonth;
        private string _currentYear;

        private ICommand _nextCommand;
        private ICommand _previousCommand;

        public ICommand NextCommand
        {
            get { return _nextCommand ?? (_nextCommand = new RelayCommand(ExecuteNext)); }
        }

        public ICommand PreviousCommand
        {
            get { return _previousCommand ?? (_previousCommand = new RelayCommand(ExecutePrevious)); }
        }

        public string CurrentMonth
        {
            get { return _currentMonth; }
            set { SetProperty(ref _currentMonth, value); }
        }

        public string CurrentYear
        {
            get { return _currentYear; }
            set { SetProperty(ref _currentYear, value); }
        }

        private void ExecuteNext(object obj)
        {
            _index++;
            BindData(_index);
        }

        private void ExecutePrevious(object obj)
        {
            _index--;
            BindData(_index);
        }

        private int _index = 0;

        private List<DayItemModel> _singleMonth;
        public List<DayItemModel> SingleMonth
        {
            get { return _singleMonth; }
            set { SetProperty(ref _singleMonth, value); }
        }

        public BasicMonthViewModel()
        {
            Messenger.Instance.Register<GotoMonth>(JumpTo);
            Messenger.Instance.Register<Refresh>(RefreshAll);
            BindData(_index);
            Messenger.Instance.Register<UpdateTaskList>(Tasks_CollectionChanged);
        }

        private void RefreshAll(object obj)
        {
            BindData(_index);
        }

        async void Tasks_CollectionChanged(object sender)
        {
            if(SingleMonth!=null)
            {
                foreach (var dayModel in SingleMonth)
                {
                    dayModel.TaskList = await TaskRepository.Instance.GetTaskListFor(dayModel.Today);
                }
            }
        }

        private void JumpTo(object obj)
        {
            if(obj !=null)
            {
                var dateTime = ((GotoMonth) obj).Target;
                if (dateTime != null)
                {
                    var targetDate = dateTime.Value;

                    var deviation = (targetDate.Month - DateTime.Now.Month) + (targetDate.Year - DateTime.Now.Year)*12;

                    _index = deviation;
                }
                BindData(_index);
            }
        }

        private async void BindData(int index)
        {
            var calendar = new Calendar();
            calendar.AddMonths(index);

            CurrentMonth = calendar.MonthAsString();
            CurrentYear = calendar.YearAsString();

            // Save month.
            GlobalData.SelectedMonthIndex = calendar.Month;

            var fDayOfThisMonth = new DateTime(calendar.Year, calendar.Month, calendar.FirstDayInThisMonth);
            var fbuffer = CalcBuffer(fDayOfThisMonth);

            // Save this value to draw grid of month later
            GlobalData.NumberOfRows = NumberOfRowsForMonth(calendar, fDayOfThisMonth);

            var lDayOfThisMonth = new DateTime(calendar.Year, calendar.Month, calendar.LastDayInThisMonth);
            var lbuffer = CalcBuffer(lDayOfThisMonth, true);

            calendar.AddMonths(-1);

            var temp = new List<DayItemModel>();
            // Add pre buffer
            for (var i = 1; i <= fbuffer; i++)
            {
                var dim = new DayItemModel
                    {
                        Today = new DateTime(calendar.Year, calendar.Month, calendar.LastDayInThisMonth - (fbuffer - i)),
                    };
                dim.TaskList = await TaskRepository.Instance.GetTaskListFor(dim.Today);
                temp.Add(dim);
            }

            calendar.AddMonths(1);
            // Add month
            for (var i = 0; i < lDayOfThisMonth.Day; i++)
            {
                var dim = new DayItemModel
                    {
                        Today = new DateTime(calendar.Year, calendar.Month, calendar.FirstDayInThisMonth + i)
                    };
                dim.TaskList = await TaskRepository.Instance.GetTaskListFor(dim.Today);

                temp.Add(dim);
            }

            calendar.AddMonths(1);
            // Add post buffer
            for (var i = 0; i < lbuffer; i++)
            {
                var dim = new DayItemModel
                    {
                        Today = new DateTime(calendar.Year, calendar.Month, calendar.FirstDayInThisMonth + i)
                    };
                dim.TaskList = await TaskRepository.Instance.GetTaskListFor(dim.Today);

                temp.Add(dim);
            }

            SingleMonth = temp;
        }

        private int CalcBuffer(DateTime fDayOfThisMonth, bool reverse = false)
        {
            var buffer = 0;

            switch (fDayOfThisMonth.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    buffer = 0;
                    break;
                case DayOfWeek.Tuesday:
                    buffer = 1;
                    break;
                case DayOfWeek.Wednesday:
                    buffer = 2;
                    break;
                case DayOfWeek.Thursday:
                    buffer = 3;
                    break;
                case DayOfWeek.Friday:
                    buffer = 4;
                    break;
                case DayOfWeek.Saturday:
                    buffer = 5;
                    break;
                case DayOfWeek.Sunday:
                    buffer = 6;
                    break;
            }

            if (reverse)
            {
                buffer = Math.Abs(buffer - 6);
            }

            return buffer;
        }

        private int NumberOfRowsForMonth(Calendar calendar, DateTime fdayInThisMonth)
        {
            if(calendar.NumberOfDaysInThisMonth == 31)
            {
                if (fdayInThisMonth.DayOfWeek == DayOfWeek.Saturday ||
                   fdayInThisMonth.DayOfWeek == DayOfWeek.Sunday)
                {
                    return 6;
                }

                return 5;
            }

            if (calendar.NumberOfDaysInThisMonth == 30)
            {
                if (fdayInThisMonth.DayOfWeek == DayOfWeek.Sunday)
                {
                    return 6;
                }

                return 5;
            }

            return 5;
        }
    }
}

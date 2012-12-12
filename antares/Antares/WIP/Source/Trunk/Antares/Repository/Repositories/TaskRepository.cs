using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;
using AntaresShell.Common;
using AntaresShell.Common.MessageTemplates;
using AntaresShell.IO;
using AntaresShell.Logger;
using Repository.MODELs;
using Repository.ServiceConnection.Controllers;
using Repository.Sync;
using AntaresShell.NavigatorProvider;
using Windows.UI.Xaml;

namespace Repository.Repositories
{
    public class TaskRepository
    {
        LocalStorageManager _taskLocal = new LocalStorageManager("Cache\\TaskList.esec");

        public ObservableCollection<TaskModel> Tasks = new ObservableCollection<TaskModel>();

        private readonly SemaphoreSlim _slGetAllTasks = new SemaphoreSlim(1);

        public async Task<ObservableCollection<TaskModel>> GetAllTasksForUser(int userID)
        {
            await _slGetAllTasks.WaitAsync();
            try
            {
                if (Tasks.Count != 0)
                {
                    var query = from task in Tasks
                                where task.UserID == userID
                                select task;

                    if (query.Any())
                    {
                        return new ObservableCollection<TaskModel>(query);
                    }

                }

                var dumb = (ObservableCollection<TaskModel>)await TaskController.Instance.GetTasksAsync("uid$$" + GlobalData.MyUserID);

                if (dumb != null)
                {
                    foreach (var VARIABLE in dumb)
                    {
                        var task = Tasks.FirstOrDefault(p => p.ID == VARIABLE.ID);
                        if (task == null)
                        {
                            Tasks.Add(VARIABLE);
                        }
                        else
                        {
                            Tasks[Tasks.IndexOf(task)] = VARIABLE;
                        }
                    }
                }
                else
                {
                    var d = await _taskLocal.RestoreAsync<ObservableCollection<TaskModel>>();
                    if (d != null)
                    {
                        Tasks = d;
                    }

                    if (Tasks.Count != 0)
                    {
                        var query = from task in Tasks
                                    where task.UserID == userID
                                    select task;

                        if (query.Any())
                        {
                            return new ObservableCollection<TaskModel>(query);
                        }
                    }
                }

                return dumb;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogException(ex.ToString());
                return null;
            }
            finally
            {
                Messenger.Instance.Notify(UpdateTaskList.Update);
                _slGetAllTasks.Release();
            }
        }

        public async Task<ObservableCollection<TaskModel>> GetTaskListFor(DateTime dt)
        {
            if (Tasks == null)
            {
                await GetAllTasksForUser(GlobalData.MyUserID);
                if (Tasks == null)
                {
                    return null;
                }
            }

            var list = new ObservableCollection<TaskModel>();

            var myTask = from task in Tasks
                         where task.UserID == GlobalData.MyUserID
                         select task;

            if (!myTask.Any())
            {
                return null;
            }

            foreach (var taskModel in myTask)
            {
                var sdate = Convert.ToDateTime(taskModel.StartDate);
                var edate = Convert.ToDateTime(taskModel.EndDate);

                switch (taskModel.RepeatType)
                {
                    case 0:
                        if (sdate.Day == dt.Day
                        && sdate.Month == dt.Month
                        && sdate.Year == dt.Year)
                        {
                            list.Add(taskModel);
                        }
                        break;

                    //daily
                    case 1:
                        if ((sdate.Day <= dt.Day
                        && sdate.Month <= dt.Month
                        && sdate.Year <= dt.Year)

                        && (edate.Day >= dt.Day
                        && edate.Month >= dt.Month
                        && edate.Year >= dt.Year))
                        {
                            list.Add(taskModel);
                        }
                        break;

                    //weekly
                    case 2:
                        if (dt.DayOfWeek == sdate.DayOfWeek)
                        {
                            if ((sdate.Day <= dt.Day
                       && sdate.Month <= dt.Month
                       && sdate.Year <= dt.Year)

                       && (edate.Day >= dt.Day
                       && edate.Month >= dt.Month
                       && edate.Year >= dt.Year))
                            {
                                list.Add(taskModel);
                            }
                        }
                        break;

                    // monthly
                    case 3:
                        if (dt.Day == sdate.Day)
                        {
                            if ((sdate.Day <= dt.Day
                       && sdate.Month <= dt.Month
                       && sdate.Year <= dt.Year)

                       && (edate.Day >= dt.Day
                       && edate.Month >= dt.Month
                       && edate.Year >= dt.Year))
                            {
                                list.Add(taskModel);
                            }
                        }
                        break;

                    // yearly
                    case 4:
                        if (dt.Day == sdate.Day && dt.Month == sdate.Month)
                        {
                            if ((sdate.Day <= dt.Day
                        && sdate.Month <= dt.Month
                        && sdate.Year <= dt.Year)

                        && (edate.Day >= dt.Day
                        && edate.Month >= dt.Month
                        && edate.Year >= dt.Year))
                            {
                                list.Add(taskModel);
                            }
                        }
                        break;
                }

            }

            var query = from t in list
                        orderby t.StartTime
                        select t as TaskModel;
            return new ObservableCollection<TaskModel>(query);
        }

        public async Task<ObservableCollection<TaskModel>> GetAllTasksForProject(int projectID)
        {
            await _slGetAllTasks.WaitAsync();
            try
            {
                if (Tasks.Count != 0)
                {
                    var query = from task in Tasks
                                where task.ProjectID == projectID
                                select task;

                    if (query.Any())
                    {
                        var notMyTask = from t in query
                                        where t.UserID != GlobalData.MyUserID
                                        select t;

                        if (notMyTask.Any())
                        {
                            return new ObservableCollection<TaskModel>(query);
                        }
                    }
                }

                var dumb = (ObservableCollection<TaskModel>)await TaskController.Instance.GetTasksAsync("pid$$" + projectID);

                if (dumb != null)
                {
                    foreach (var VARIABLE in dumb)
                    {
                        var task = Tasks.FirstOrDefault(p => p.ID == VARIABLE.ID);
                        if (task == null)
                        {
                            Tasks.Add(VARIABLE);
                        }
                        else
                        {
                            Tasks[Tasks.IndexOf(task)] = VARIABLE;
                        }

                    }
                }
                else
                {

                    var d = await _taskLocal.RestoreAsync<ObservableCollection<TaskModel>>();
                    if (d != null)
                    {
                        Tasks = d;
                    }

                    if (Tasks.Count != 0)
                    {
                        var query = from task in Tasks
                                    where task.ProjectID == projectID
                                    select task;

                        if (query.Any())
                        {
                            return new ObservableCollection<TaskModel>(query);
                        }
                    }
                }

                return dumb;
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogException(ex.ToString());
                return null;
            }
            finally
            {
                Messenger.Instance.Notify(UpdateTaskList.Update);
                _slGetAllTasks.Release();
            }
        }

        public async Task<TaskModel> AddNewTask(TaskModel data)
        {
            var response = await TaskController.Instance.PostAsync(data);
            if (response.IsSuccessStatusCode)
            {

                NotificationUtils.CreateNotification(data);
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var djs = new DataContractJsonSerializer(typeof(TaskModel));
                    var taskModel = (TaskModel)djs.ReadObject(stream);
                    Tasks.Add(taskModel);
                    NotificationUtils.CreateNotification(taskModel);
                    Messenger.Instance.Notify(UpdateTaskList.Update);

                    return taskModel;
                }
            }

            return null;
        }

        public async Task<HttpResponseMessage> UpdateTask(TaskModel data)
        {
            var response = await TaskController.Instance.PushAsync(data.ID, data);
            if (response.IsSuccessStatusCode)
            {
                var target = Tasks.FirstOrDefault(p => p.ID == data.ID);
                var index = Tasks.IndexOf(target);
                Tasks.RemoveAt(index);
                NotificationUtils.DeleteNotification(data.ID);
                IFormatProvider cultureInfo = new CultureInfo("en-US");
                data.StartDate = DateTime.Parse(data.StartDate, cultureInfo).ToString();
                data.EndDate = DateTime.Parse(data.EndDate, cultureInfo).ToString();

                Tasks.Insert(index, data);
                NotificationUtils.CreateNotification(data);

                Messenger.Instance.Notify(UpdateTaskList.Update);

                return response;
            }

            return null;
        }

        public async Task<HttpResponseMessage> DeleteTask(int id)
        {
            var response = await TaskController.Instance.DeleteAsync(id);
            if (response.IsSuccessStatusCode)
            {
                var target = Tasks.FirstOrDefault(p => p.ID == id);
                Tasks.Remove(target);
                NotificationUtils.DeleteNotification(id);

                Messenger.Instance.Notify(UpdateTaskList.Update);
            }

            return response;
        }

        private void ClearCache()
        {
            Tasks = new ObservableCollection<TaskModel>();
        }

        #region Singleton
        private TaskRepository()
        {
            Tasks.CollectionChanged += Tasks_CollectionChanged;
            Messenger.Instance.Register<Refresh>(p =>
                                                     {
                                                         var type = (Refresh)p;
                                                         if (type == Refresh.All || type == Refresh.Task)
                                                         {
                                                             ClearCache();
                                                         }
                                                     });
        }

        async void Tasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await _taskLocal.SaveAsync(Tasks);
        }

        public static TaskRepository Instance
        {
            get { return Nested._instance; }
        }

        private class Nested
        {
            /// <summary>
            /// Instance of Repository for Singleton pattern.
            /// </summary>
            internal static readonly TaskRepository _instance = new TaskRepository();

            /// <summary>
            /// Explicit static constructor to tell C# compiler not to mark type as beforefieldinit.
            /// </summary>
            static Nested()
            {
            }
        }
        #endregion
    }
}

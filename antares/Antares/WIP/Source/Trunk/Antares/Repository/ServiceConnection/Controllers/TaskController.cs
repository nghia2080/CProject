using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using AntaresShell.Localization;
using AntaresShell.Logger;
using Repository.MODELs;
using System.Globalization;

namespace Repository.ServiceConnection.Controllers
{
    public class TaskController : BaseConnection
    {
        public TaskController()
        {
            ApiRoot += "Task";
        }

        public async Task<dynamic> GetTasksAsync(string id)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var resp = await http.GetAsync(new Uri(ApiRoot + "?id=" + id));
                    using (var stream = await resp.Content.ReadAsStreamAsync())
                    {
                        var djs = new DataContractJsonSerializer(typeof(List<TaskModel>));
                        return new ObservableCollection<TaskModel>((IEnumerable<TaskModel>)djs.ReadObject(stream));
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogException(ex.ToString());
                return null;
            }
        }

        public override async Task<dynamic> GetAsync(int id)
        {
            try
            {
                var resp = await HttpClient.GetAsync(new Uri(ApiRoot) + @"/" + id);
                using (var stream = await resp.Content.ReadAsStreamAsync())
                {
                    var djs = new DataContractJsonSerializer(typeof(TaskModel));
                    return (TaskModel)djs.ReadObject(stream);
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogException(ex.ToString());
                return null;
            }
        }

        public override async Task<HttpResponseMessage> PostAsync(dynamic targetObject)
        {
            return await EditUserCommit(-1, targetObject);
        }

        public override async Task<HttpResponseMessage> PushAsync(int id, dynamic targetObject)
        {
            return await EditUserCommit(id, targetObject, true);
        }

        private async Task<HttpResponseMessage> EditUserCommit(int id, dynamic targetObject, bool edit = false)
        {
            try
            {
                //if (targetObject.StartDate != null)
                //{
                //    targetObject.StartDate =
                //        Convert.ToDateTime(targetObject.StartDate).ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
                //}

                //if (targetObject.EndDate != null)
                //{
                //    targetObject.EndDate =
                //        Convert.ToDateTime(targetObject.EndDate).ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
                //}

                // TODO: Emi commented out because exception in vietnamese
                //if (!edit)
                //{
                   
                //}

                var person = new TaskModel((TaskModel)targetObject)
                                 {
                                     StartDate = RepositoryUtils.JustifyDateTimeCulture(targetObject.StartDate),
                                     EndDate = RepositoryUtils.DateTimeCulture(targetObject.EndDate)
                                 };

                using (var ms = new MemoryStream())
                {
                    var djs = new DataContractJsonSerializer(typeof(TaskModel));
                    djs.WriteObject(ms, person);
                    ms.Position = 0;
                    var sc = new StreamContent(ms);
                    sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    var resp = id == -1
                                   ? await HttpClient.PostAsync(new Uri(ApiRoot), sc)
                                   : await HttpClient.PutAsync(new Uri(ApiRoot + @"/" + id), sc);

                    resp.EnsureSuccessStatusCode();
                    var tmp = await resp.Content.ReadAsStringAsync();
                    return resp;
                }

            }
            catch (Exception ex)
            {
                LogManager.Instance.LogException(ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.NotModified);
            }
        }

        private static TaskController _instance;
        public static TaskController Instance
        {
            get { return _instance ?? (_instance = new TaskController()); }
        }

        /// <summ
    }
}

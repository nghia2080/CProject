using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using AntaresShell.Logger;
using Repository.MODELs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ServiceConnection.Controllers
{
    public class ProjectMemberController : BaseConnection
    {
        public ProjectMemberController()
        {
            ApiRoot += "ProjectMember";
        }

        public async Task<dynamic> GetAsync(string id)
        {
            try
            {
                var resp = await HttpClient.GetAsync(new Uri(ApiRoot) + @"?id=" + id);
                using (var stream = await resp.Content.ReadAsStreamAsync())
                {
                    var djs = new DataContractJsonSerializer(typeof(List<ProjectMemberContrainModel>));
                    var dta = (List<ProjectMemberContrainModel>) djs.ReadObject(stream);
                    if(dta==null)
                    {
                        return null;
                    }

                    return new ObservableCollection<ProjectMemberContrainModel>(dta);
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
            return await EditUserCommit(id, targetObject);
        }

        private async Task<HttpResponseMessage> EditUserCommit(int id, dynamic targetObject)
        {
            try
            {
                var person = (ProjectMemberContrainModel)targetObject;

                using (var ms = new MemoryStream())
                {
                    var djs = new DataContractJsonSerializer(typeof(ProjectMemberContrainModel));
                    djs.WriteObject(ms, person);
                    ms.Position = 0;
                    var sc = new StreamContent(ms);
                    sc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    var resp = id == -1
                                   ? await HttpClient.PostAsync(new Uri(ApiRoot), sc)
                                   : await HttpClient.PutAsync(new Uri(ApiRoot + @"/" + id), sc);

                    resp.EnsureSuccessStatusCode();
                    return resp;
                }

            }
            catch (Exception ex)
            {
                LogManager.Instance.LogException(ex.ToString());
                return new HttpResponseMessage(HttpStatusCode.NotModified);
            }
        }

        private static ProjectMemberController _instance;

        public static ProjectMemberController Instance
        {
            get { return _instance ?? (_instance = new ProjectMemberController()); }
        }
    }
}

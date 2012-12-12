using System.IO;
using System.Net;
using System.Net.Http;
using System;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using AntaresShell.Logger;
using Repository.MODELs;

namespace Repository.ServiceConnection.Controllers
{
    public class UserInformationController : BaseConnection
    {
        public UserInformationController()
        {
            ApiRoot += "User";
        }

        public override async Task<dynamic> GetAsync(int id)
        {
            try
            {
                var resp = await HttpClient.GetAsync(new Uri(ApiRoot) + @"/" + id);
                using (var stream = await resp.Content.ReadAsStreamAsync())
                {
                    var djs = new DataContractJsonSerializer(typeof(UserModel));
                    return (UserModel)djs.ReadObject(stream);
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogException(ex.ToString());
                return null;
            }
        }

        public async Task<dynamic> GetAsync(string username)
        {
            try
            {
                var resp = await HttpClient.GetAsync(new Uri(ApiRoot) + @"?username=" + username + "");
                using (var stream = await resp.Content.ReadAsStreamAsync())
                {
                    var djs = new DataContractJsonSerializer(typeof(UserModel));
                    return (UserModel)djs.ReadObject(stream);
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
                var person = (UserModel)targetObject;

                using (var ms = new MemoryStream())
                {
                    var djs = new DataContractJsonSerializer(typeof(UserModel));
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

        private static UserInformationController _instance;
        public static UserInformationController Instance
        {
            get { return _instance ?? (_instance = new UserInformationController()); }
        }
    }
}

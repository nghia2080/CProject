using System.Net;
using AntaresShell.Logger;
using Repository.ServiceConnection.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Repository.ServiceConnection.Controllers
{
    public class BaseConnection : IServiceConnection
    {
        protected BaseConnection()
        {
            ApiRoot = "http://antaresservice.cloudapp.net/api/"; // "http://127.0.0.1:81/api/";//
            HttpClientHandler = new HttpClientHandler();
            HttpClient = new HttpClient(HttpClientHandler);
        }

        protected string ApiRoot { get; set; }

        /// <summary>
        /// Add some special handler for HTTP client.
        /// </summary>
        protected HttpClientHandler HttpClientHandler;

        /// <summary>
        /// Object use to send HTTP request to API root.
        /// </summary>
        protected HttpClient HttpClient;

        public virtual async Task<HttpResponseMessage> PushAsync(int id, dynamic targetObject)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<HttpResponseMessage> PostAsync(dynamic targetObject)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<HttpResponseMessage> DeleteAsync(int id)
        {
            try
            {
                var resp = await HttpClient.DeleteAsync(new Uri(ApiRoot + "/" + id));
                //resp.EnsureSuccessStatusCode();
                return resp;
            }
            catch (Exception exception)
            {
                LogManager.Instance.LogException(exception.ToString());
                return new HttpResponseMessage(HttpStatusCode.NotModified);
            }
        }

        public virtual async Task<dynamic> GetAsync()
        {
            //var resp = await HttpClient.GetAsync(new Uri(ApiRoot));
            //using (var stream = await resp.Content.ReadAsStreamAsync())
            //{
            //    var djs = new DataContractJsonSerializer(typeof(List<Person>));
            //    People = new ObservableCollection<Person>((IEnumerable<Person>)djs.ReadObject(stream));
            //}
            throw new NotImplementedException();
        }

        public virtual async Task<dynamic> GetAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}

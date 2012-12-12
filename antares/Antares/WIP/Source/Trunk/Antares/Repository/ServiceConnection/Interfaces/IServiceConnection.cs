using System.Net.Http;
using System.Threading.Tasks;

namespace Repository.ServiceConnection.Interfaces
{
    public interface IServiceConnection
    {
        Task<HttpResponseMessage> PushAsync(int id, dynamic targetObject);
        Task<HttpResponseMessage> PostAsync(dynamic targetObject);
        Task<HttpResponseMessage> DeleteAsync(int id);
        Task<dynamic> GetAsync();
        Task<dynamic> GetAsync(int id);
    }
}

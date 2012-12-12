using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using AntaresShell.Logger;
using Repository.MODELs;

namespace Repository.ServiceConnection.Controllers
{
    class PriorityController : BaseConnection
    {
        public override async Task<dynamic> GetAsync()
        {
            try
            {
                var resp = await HttpClient.GetAsync(new Uri(ApiRoot));
                using (var stream = await resp.Content.ReadAsStreamAsync())
                {
                    var djs = new DataContractJsonSerializer(typeof(List<PriorityModel>));
                    return new ObservableCollection<PriorityModel>((IEnumerable<PriorityModel>)djs.ReadObject(stream));
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogException(ex.ToString());
                return null;
            }
        }

        #region Singleton
        private PriorityController()
        {
            ApiRoot += "Priority";
        }

        public static PriorityController Instance
        {
            get { return Nested._instance; }
        }

        private class Nested
        {
            /// <summary>
            /// Instance of Repository for Singleton pattern.
            /// </summary>
            internal static readonly PriorityController _instance = new PriorityController();

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

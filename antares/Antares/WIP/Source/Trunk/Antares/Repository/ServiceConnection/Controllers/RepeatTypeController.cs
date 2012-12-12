using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using AntaresShell.Logger;
using Repository.MODELs;

namespace Repository.ServiceConnection.Controllers
{
    class RepeatTypeController : BaseConnection
    {
        public override async Task<dynamic> GetAsync()
        {
            try
            {
                var resp = await HttpClient.GetAsync(new Uri(ApiRoot));
                using (var stream = await resp.Content.ReadAsStreamAsync())
                {
                    var djs = new DataContractJsonSerializer(typeof(List<RepeatTypeModel>));
                    return new ObservableCollection<RepeatTypeModel>((IEnumerable<RepeatTypeModel>)djs.ReadObject(stream));
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogException(ex.ToString());
                return null;
            }
        }

        #region Singleton
        private RepeatTypeController()
        {
            ApiRoot += "RepeatType";
        }

        public static RepeatTypeController Instance
        {
            get { return Nested._instance; }
        }

        private class Nested
        {
            /// <summary>
            /// Instance of Repository for Singleton pattern.
            /// </summary>
            internal static readonly RepeatTypeController _instance = new RepeatTypeController();

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

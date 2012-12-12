using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using AntaresShell.Logger;
using Repository.MODELs;

namespace Repository.ServiceConnection.Controllers
{
    public class CategoryController : BaseConnection
    {
        public override async Task<dynamic> GetAsync()
        {
            try
            {
                var resp = await HttpClient.GetAsync(new Uri(ApiRoot));
                using (var stream = await resp.Content.ReadAsStreamAsync())
                {
                    var djs = new DataContractJsonSerializer(typeof(List<CategoryModel>));
                    return new ObservableCollection<CategoryModel>((IEnumerable<CategoryModel>)djs.ReadObject(stream));
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.LogException(ex.ToString());
                return null;
            }
        }

        #region Singleton
        private CategoryController()
        {
            ApiRoot += "Category";
        }

        public static CategoryController Instance
        {
            get { return Nested._instance; }
        }

        private class Nested
        {
            /// <summary>
            /// Instance of Repository for Singleton pattern.
            /// </summary>
            internal static readonly CategoryController _instance = new CategoryController();

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

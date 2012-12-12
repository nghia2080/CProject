using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AntaresShell.IO;
using AntaresShell.Localization;
using Repository.MODELs;
using Repository.ServiceConnection.Controllers;

namespace Repository.Repositories
{
    public class PriorityRepository
    {
        LocalStorageManager _priLocal = new LocalStorageManager("Cache\\Priorities.esec");

        private ObservableCollection<PriorityModel> _priorities;

        private readonly SemaphoreSlim _sl = new SemaphoreSlim(1);

        public async Task<ObservableCollection<PriorityModel>> GetAllPriorities()
        {
            await _sl.WaitAsync();

            try
            {
                if (_priorities != null)
                {
                    return _priorities;
                }

                _priorities = await PriorityController.Instance.GetAsync();

                if (_priorities != null)
                {
                    await _priLocal.SaveAsync(_priorities);
                }
                else
                {
                    _priorities = await _priLocal.RestoreAsync<ObservableCollection<PriorityModel>>();
                }

                //localize
                if (_priorities != null)
                {
                    foreach (var priorityModel in _priorities)
                    {
                        if (priorityModel.Name.ToLowerInvariant().Contains("low"))
                        {
                            priorityModel.Name = LanguageProvider.Resource["Tsk_Priority_Low"];
                        }
                        else if (priorityModel.Name.ToLowerInvariant().Contains("medium"))
                        {
                            priorityModel.Name = LanguageProvider.Resource["Tsk_Priority_Medium"];
                        }
                        else if (priorityModel.Name.ToLowerInvariant().Contains("high"))
                        {
                            priorityModel.Name = LanguageProvider.Resource["Tsk_Priority_High"];
                        }

                    }
                }

                return _priorities;
            }
            finally
            {
                _sl.Release();
            }
        }

        public void Clearcache()
        {
            _priorities = null;
        }

        #region Singleton
        private PriorityRepository()
        {

        }

        public static PriorityRepository Instance
        {
            get { return Nested._instance; }
        }

        private class Nested
        {
            /// <summary>
            /// Instance of Repository for Singleton pattern.
            /// </summary>
            internal static readonly PriorityRepository _instance = new PriorityRepository();

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

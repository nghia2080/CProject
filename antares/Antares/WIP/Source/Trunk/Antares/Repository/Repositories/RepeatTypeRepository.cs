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
    public class RepeatTypeRepository
    {
        LocalStorageManager _reptLocal = new LocalStorageManager("Cache\\RepeateType.esec");

        private ObservableCollection<RepeatTypeModel> _repeatTypes;

        private readonly SemaphoreSlim _sl = new SemaphoreSlim(1);

        public async Task<ObservableCollection<RepeatTypeModel>> GetAllRepeatTypes()
        {
            await _sl.WaitAsync();

            try
            {
                if (_repeatTypes != null)
                {
                    return _repeatTypes;
                }

                _repeatTypes = await RepeatTypeController.Instance.GetAsync();

                if (_repeatTypes != null)
                {
                    await _reptLocal.SaveAsync(_repeatTypes);
                }
                else
                {
                    _repeatTypes = await _reptLocal.RestoreAsync<ObservableCollection<RepeatTypeModel>>();
                }

                // localize
                if (_repeatTypes != null)
                {
                    foreach (var repeatTypeModel in _repeatTypes)
                    {
                        if (repeatTypeModel.Name.ToLowerInvariant().Contains("once"))
                        {
                            repeatTypeModel.Name = LanguageProvider.Resource["Tsk_RepeatType_Once"];
                        }
                        else if (repeatTypeModel.Name.ToLowerInvariant().Contains("daily"))
                        {
                            repeatTypeModel.Name = LanguageProvider.Resource["Tsk_RepeatType_Daily"];
                        }
                        else if (repeatTypeModel.Name.ToLowerInvariant().Contains("weekly"))
                        {
                            repeatTypeModel.Name = LanguageProvider.Resource["Tsk_RepeatType_Weekly"];
                        }
                        else if (repeatTypeModel.Name.ToLowerInvariant().Contains("monthly"))
                        {
                            repeatTypeModel.Name = LanguageProvider.Resource["Tsk_RepeatType_Monthly"];
                        }
                        else if (repeatTypeModel.Name.ToLowerInvariant().Contains("yearly"))
                        {
                            repeatTypeModel.Name = LanguageProvider.Resource["Tsk_RepeatType_Yearly"];
                        }
                    }
                }
                return _repeatTypes;
            }
            finally
            {
                _sl.Release();
            }
        }

        #region Singleton
        private RepeatTypeRepository()
        {

        }

        public static RepeatTypeRepository Instance
        {
            get { return Nested._instance; }
        }

        private class Nested
        {
            /// <summary>
            /// Instance of Repository for Singleton pattern.
            /// </summary>
            internal static readonly RepeatTypeRepository _instance = new RepeatTypeRepository();

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

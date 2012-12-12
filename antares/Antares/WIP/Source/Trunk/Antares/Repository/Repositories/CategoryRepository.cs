using System.Threading;
using AntaresShell.IO;
using AntaresShell.Localization;
using Repository.MODELs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.ServiceConnection.Controllers;

namespace Repository.Repositories
{
    public class CategoryRepository
    {
        LocalStorageManager _cateLocal = new LocalStorageManager("Cache\\Categories.esec");

        private ObservableCollection<CategoryModel> _categories;

        private readonly SemaphoreSlim _sl = new SemaphoreSlim(1);

        public async Task<ObservableCollection<CategoryModel>> GetAllCategories()
        {
            await _sl.WaitAsync();

            try
            {
                if (_categories != null)
                {
                    return _categories;
                }

                _categories = await CategoryController.Instance.GetAsync();

                if (_categories != null)
                {
                    await _cateLocal.SaveAsync(_categories);
                }
                else
                {
                    _categories = await _cateLocal.RestoreAsync<ObservableCollection<CategoryModel>>();
                }

                // localize
                if (_categories != null)
                {
                    foreach (var categoryModel in _categories)
                    {
                        if (categoryModel.Name.ToLowerInvariant().Contains("bussiness"))
                        {
                            categoryModel.Name = LanguageProvider.Resource["Tsk_Categories_Bussiness"];
                        }
                        else if (categoryModel.Name.ToLowerInvariant().Contains("project"))
                        {
                            categoryModel.Name = LanguageProvider.Resource["Tsk_Categories_Project"];
                        }
                        else if (categoryModel.Name.ToLowerInvariant().Contains("meeting"))
                        {
                            categoryModel.Name = LanguageProvider.Resource["Tsk_Categories_Meeting"];
                        }
                        else if (categoryModel.Name.ToLowerInvariant().Contains("entertainment"))
                        {
                            categoryModel.Name = LanguageProvider.Resource["Tsk_Categories_Entertainment"];
                        }
                        else if (categoryModel.Name.ToLowerInvariant().Contains("sport"))
                        {
                            categoryModel.Name = LanguageProvider.Resource["Tsk_Categories_Sport"];
                        }
                        else if (categoryModel.Name.ToLowerInvariant().Contains("study"))
                        {
                            categoryModel.Name = LanguageProvider.Resource["Tsk_Categories_Study"];
                        }
                        else if (categoryModel.Name.ToLowerInvariant().Contains("requirements"))
                        {
                            categoryModel.Name = LanguageProvider.Resource["Tsk_SubCategories_Requirements"];
                        }
                        else if (categoryModel.Name.ToLowerInvariant().Contains("design"))
                        {
                            categoryModel.Name = LanguageProvider.Resource["Tsk_SubCategories_Design"];
                        }
                        else if (categoryModel.Name.ToLowerInvariant().Contains("implementation"))
                        {
                            categoryModel.Name = LanguageProvider.Resource["Tsk_SubCategories_Implementation"];
                        }
                        else if (categoryModel.Name.ToLowerInvariant().Contains("verification"))
                        {
                            categoryModel.Name = LanguageProvider.Resource["Tsk_SubCategories_Verification"];
                        }
                        else if (categoryModel.Name.ToLowerInvariant().Contains("maintenance"))
                        {
                            categoryModel.Name = LanguageProvider.Resource["Tsk_SubCategories_Maintenance"];
                        }
                    }
                }

                return _categories;
            }
            finally
            {
                _sl.Release();
            }
        }

        public async Task<ObservableCollection<CategoryModel>> GetMainCategories()
        {
            if (_categories == null)
            {
                _categories = await CategoryController.Instance.GetAsync();
            }

            if (_categories == null)
            {
                return null;
            }

            var categoryQuery = from category in _categories
                                where category.Type == 0
                                select category;

            return new ObservableCollection<CategoryModel>(categoryQuery.AsEnumerable());
        }

        public async Task<ObservableCollection<CategoryModel>> GetSubCategories()
        {
            if (_categories == null)
            {
                _categories = await CategoryController.Instance.GetAsync();
            }


            if (_categories == null)
            {
                return null;
            }

            var categoryQuery = from category in _categories
                                where category.Type == 1
                                select category;

            return new ObservableCollection<CategoryModel>(categoryQuery.AsEnumerable());
        }

        public void Clearcache()
        {
            _categories = null;
        }

        #region Singleton
        private CategoryRepository()
        {

        }

        public static CategoryRepository Instance
        {
            get { return Nested._instance; }
        }

        private class Nested
        {
            /// <summary>
            /// Instance of Repository for Singleton pattern.
            /// </summary>
            internal static readonly CategoryRepository _instance = new CategoryRepository();

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

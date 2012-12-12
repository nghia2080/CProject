using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AntaresShell.BaseClasses;
using SearchEngine.Interfaces;

namespace SearchEngine
{
    /// <summary>
    /// A class to manage all search providers in VCM.
    /// </summary>
    public class SearchProviderManager
    {
        /// <summary>
        /// A static list of all search providers in VCM.
        /// </summary>
        private static readonly List<ISearchProvider> _searchProviders;

        /// <summary>
        /// A static list of current search item.
        /// </summary>
        private static List<SearchableBaseModel> _currentSeachItem;

        /// <summary>
        /// Initializes static members of the SearchProviderManager class.
        /// </summary>
        static SearchProviderManager()
        {
            var localSearchProvider = new LocalSearchProvider();
            _searchProviders = new List<ISearchProvider> { localSearchProvider };
        }

        /// <summary>
        /// Searchs the VCM to find objects associate with the search string.
        /// </summary>
        /// <param name="query">The search string entered by user.</param>
        /// <param name="documentList">A list contains data to be searched.</param>
        /// <returns>A list of results, grouped by sections.</returns>
        public static ObservableCollection<SearchGroupModel> Search(string query, List<SearchableBaseModel> documentList = null)
        {
            return null;
            //var results = new ObservableCollection<SearchGroupModel>();
            //var messagesSection = new SearchGroupModel { SectionHeader = ContentTitle.MESSAGES_CONTENT, SearchResults = new ObservableCollection<SearchableBaseModel>() };
            //var aboutYourVAIOSection = new SearchGroupModel { SectionHeader = ContentTitle.ABOUT_VAIO_UPPER_TITLE, SearchResults = new ObservableCollection<SearchableBaseModel>() };
            //var contactAndSupportSection = new SearchGroupModel { SectionHeader = ContentTitle.CONTACT_SONY_UPPER_TITLE, SearchResults = new ObservableCollection<SearchableBaseModel>() };
            //var others = new SearchGroupModel { SearchResults = new ObservableCollection<SearchableBaseModel>() };
            //bool needIndexing = false;
            //if (documentList != null)
            //{
            //    _currentSeachItem = documentList;
            //    needIndexing = true;
            //}

            //foreach (var result in _searchProviders.Select(searchProvider => searchProvider.GetResults(query, _currentSeachItem, needIndexing)).SelectMany(items => items))
            //{
            //    // Switch theo category.
            //    // Contact support & Message.
            //    switch (result.Category)
            //    {
            //        case SearchContent.MESSAGE_CATEGORY:
            //            messagesSection.SearchResults.Add(result);
            //            break;
            //        case SearchContent.ABOUT_CATEGORY:
            //            aboutYourVAIOSection.SearchResults.Add(result);
            //            break;
            //        case SearchContent.CONTACT_CATEGORY:
            //            contactAndSupportSection.SearchResults.Add(result);
            //            break;
            //        default:
            //            others.SearchResults.Add(result);
            //            break;
            //    }
            //}

            //if (messagesSection.SearchResults.Count > 0)
            //{
            //    results.Add(messagesSection);
            //}

            //if (aboutYourVAIOSection.SearchResults.Count > 0)
            //{
            //    results.Add(aboutYourVAIOSection);
            //}

            //if (contactAndSupportSection.SearchResults.Count > 0)
            //{
            //    results.Add(contactAndSupportSection);
            //}

            //if (others.SearchResults.Count > 0)
            //{
            //    results.Add(others);
            //}

            //return results;
        }
    }
}
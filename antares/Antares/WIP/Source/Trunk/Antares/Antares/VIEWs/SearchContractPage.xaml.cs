using AntaresShell.BaseClasses;
using AntaresShell.Localization;
using Repository.MODELs;
using Repository.Repositories;
using SearchEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Search Contract item template is documented at http://go.microsoft.com/fwlink/?LinkId=234240



namespace Antares.VIEWs
{
    /// <summary>
   /// This page displays search results when a global search is directed to this application.
 
    /// </summary>
    public sealed partial class SearchContractPage
    {
        public SearchContractPage()
        {
            //NavigationCacheMode = NavigationCacheMode.Enabled;
            InitializeComponent();
            try
            {
                TaskRepository.Instance.Tasks.CollectionChanged += Tasks_CollectionChanged;
            }
            catch
            {

            }
        }

       void Tasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
 
        {
            SearchProvider.IsDirty = true;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
       protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
 
        {
            var query = navigationParameter as String;

            if (SearchProvider.IsDirty)
            {
                RenewIndex();
                SearchProvider.IsDirty = false;
            }

            var resultList = SearchProvider.Searcher.Search(query);

           var filterList = new List<Filter> { new Filter("All", 0, true) };

            resultText.Text = LanguageProvider.Resource["Search_Result"] + " ";
            noResultsTextBlock.Text = LanguageProvider.Resource["Search_NoResult"];
            // Communicate results through the view model
            DefaultViewModel["QueryText"] = '\u201c' + query + '\u201d';
        DefaultViewModel["Results"] = CreateActualResultList(resultList, SearchProvider.MapStringItem, SearchProvider.ItemList);
    
            DefaultViewModel["Filters"] = filterList;
            DefaultViewModel["ShowFilters"] = filterList.Count > 1;
        //DefaultViewModel["Results"] = CreateActualResultList(resultList, mapStringItems, itemList);
    
        }

        async void RenewIndex()
        {
            SearchProvider.ItemList = await Repository.Repository.Instance.GetSeachableBaseModelAsync();
        var normalizedStrings = GetNormalizedStrings(SearchProvider.ItemList);
    
            var dictionary = ToStringList(SearchProvider.ItemList);
            SearchProvider.MapStringItem = MapStringToItems(dictionary, normalizedStrings);
            SearchProvider.CreateIndex(dictionary);
            SearchProvider.CreateSearcher(SearchProvider.Index);
        }

    private List<int>[] MapStringToItems(IList<string> dictionary, IList<string> normalizedStrings)
    
        {
            var returned = new List<int>[dictionary.Count];
            for (var i = 0; i < dictionary.Count; ++i)
            {
                returned[i] = new List<int>();
                // Consider using KMP algorithm to speed up.
                for (var j = 0; j < normalizedStrings.Count; ++j)
                {
                    if (normalizedStrings[j].Contains(dictionary[i]))
                    {
                        returned[i].Add(j);
                    }
                }
            }

            return returned;
        }

        private string[] GetNormalizedStrings(IList<SearchableBaseModel> itemList)
        {
            var returned = new string[itemList.Count];
            for (var i = 0; i < itemList.Count; ++i)
            {
                returned[i] = Normalize(itemList[i].ToSearchableString());
            }

            return returned;
        }

        private char[] _specialChars = new[]
            {
            '.', ',', '|', '-', ' ', '!', '`', '~', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '=', '+', '[',
    
                ']', '{', '}', '\\', ';', ':', '\'', '"', '<', '>', '/', '?' , 
            '。', '、', '・', '【', '】', '〔', '〕', '〈', '〉', '《', '》', '「', '」', '『', '』', '　'
    
            };
        private string[] ToStringList(IEnumerable<SearchableBaseModel> itemList)
        {
            var returned = new HashSet<string>();
        foreach (var word in itemList.Select(item => Normalize(item.ToSearchableString()))
    
                .Select(str => str.Split(_specialChars, StringSplitOptions.RemoveEmptyEntries))
                .SelectMany(strs => strs))
            {
                returned.Add(word);
            }
            return returned.ToArray();
        }

        private string Normalize(string source)
        {
            return source.ToLower();
        }

    private ObservableCollection<SearchResultModel> CreateActualResultList(IEnumerable<int> resultList, IList<List<int>> mapStringItems, IList<SearchableBaseModel> itemList)
    
        {
            var max = 0;
            var freq = new Dictionary<int, int>();
            foreach (var n in resultList.SelectMany(i => mapStringItems[i]))
            {
                freq[n] = freq.ContainsKey(n) ? freq[n] + 1 : 1;
                max = Math.Max(max, freq[n]);
            }
            var returned = new ObservableCollection<SearchResultModel>();
            for (var i = max; i >= 1; --i)
            {
                var i1 = i;
            foreach (var kvp in freq.Where(kvp => kvp.Value == i1))
    
                {
                    returned.Add(new SearchResultModel
                    {
                        Title = itemList[kvp.Key].Name,
                        Description = itemList[kvp.Key].Description,
                    //NavigationTarget = Convert.ToDateTime(((TaskModel)itemList[kvp.Key]).StartDate)
    
                    });
                }
            }
            return returned;
        }

        /// <summary>
        /// Invoked when a filter is selected using the ComboBox in snapped view state.
        /// </summary>
        /// <param name="sender">The ComboBox instance.</param>
    /// <param name="e">Event data describing how the selected filter was changed.</param>
    
        void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Determine what filter was selected
            var selectedFilter = e.AddedItems.FirstOrDefault() as Filter;
            if (selectedFilter != null)
            {
                // Mirror the results into the corresponding Filter object to allow the
            // RadioButton representation used when not snapped to reflect the change
    
                selectedFilter.Active = true;

            // TODO: Respond to the change in active filter by setting DefaultViewModel["Results"]
    
            //       to a collection of items with bindable Image, Name, Subtitle, and Description properties

   

                // Ensure results are found
                object results;
                ICollection resultsCollection;
                if (DefaultViewModel.TryGetValue("Results", out results) &&
                    (resultsCollection = results as ICollection) != null &&
                    resultsCollection.Count != 0)
                {
                    VisualStateManager.GoToState(this, "ResultsFound", true);
                    return;
                }
            }

        // Display informational text when there are no search results.
    
            VisualStateManager.GoToState(this, "NoResultsFound", true);
        }

        /// <summary>
    /// Invoked when a filter is selected using a RadioButton when not snapped.
    
        /// </summary>
        /// <param name="sender">The selected RadioButton instance.</param>
    /// <param name="e">Event data describing how the RadioButton was selected.</param>
    
        void Filter_Checked(object sender, RoutedEventArgs e)
        {
            // Mirror the change into the CollectionViewSource used by the corresponding ComboBox
            // to ensure that the change is reflected when snapped
            if (filtersViewSource.View != null)
            {
                var frameworkElement = sender as FrameworkElement;
                if (frameworkElement != null)
                {
                    var filter = frameworkElement.DataContext;
                    filtersViewSource.View.MoveCurrentTo(filter);
                }
            }
        }

        /// <summary>
    /// View model describing one of the filters available for viewing search results.
    
        /// </summary>
        private sealed class Filter : BindableBase
        {
            private String _name;
            private int _count;
            private bool _active;

            public Filter(String name, int count, bool active = false)
            {
                Name = name;
                Count = count;
                Active = active;
            }

            public override String ToString()
            {
                return Description;
            }

            public String Name
            {
                get { return _name; }
                set { if (SetProperty(ref _name, value)) OnPropertyChanged("Description"); }
            }

            public int Count
            {
                get { return _count; }
                set { if (SetProperty(ref _count, value)) OnPropertyChanged("Description"); }
            }

            public bool Active
            {
                get { return _active; }
                set { SetProperty(ref _active, value); }
            }

            public String Description
            {
                get { return String.Format("{0} ({1})", _name, _count); }
            }
        }
    }
}

using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using AntaresShell.BaseClasses;

namespace SearchEngine
{
    /// <summary>
    /// A class represents a group of search results.
    /// </summary>
    [DataContract]
    public class SearchGroupModel
    {
        /// <summary>
        /// Gets or sets the header of the section.
        /// </summary>
        /// <value>A string represents the section header.</value>
        [DataMember]
        public string SectionHeader { get; set; }

        /// <summary>
        /// Gets or sets the list of Search results items in the section.
        /// </summary>
        /// <value>A collection of search results.</value>
        [DataMember]
        public ObservableCollection<SearchableBaseModel> SearchResults { get; set; }
    }
}
using System.Collections.Generic;
using AntaresShell.BaseClasses;

namespace SearchEngine.Interfaces
{
    /// <summary>
    /// A common interface for every search provider.
    /// </summary>
    public interface ISearchProvider
    {
        /// <summary>
        /// Returns a list of objects corresponse to the query entered by user.
        /// </summary>
        /// <param name="query">The text entered by user.</param>
        /// <param name="documentList">List of pool item.</param>
		/// <param name="needIndexing">Condition to indexing.</param>
        /// <returns>A result set in form of a list of objects.</returns>
        List<SearchableBaseModel> GetResults(string query, List<SearchableBaseModel> documentList, bool needIndexing);
    }
}
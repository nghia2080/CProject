using System.Collections.Generic;

namespace SearchEngine.Interfaces
{
    /// <summary>
    /// An interface for searcher classes, which do the search and return the matches.
    /// </summary>
    public interface ISearcher
    {
        /// <summary>
        /// Searches matches of a specific query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A set contains the indices of each matches of the query.</returns>
        IEnumerable<int> Search(string query);
    }
}

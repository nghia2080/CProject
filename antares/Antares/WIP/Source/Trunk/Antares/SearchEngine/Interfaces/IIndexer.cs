namespace SearchEngine.Interfaces
{
    /// <summary>
    /// An interface for indexer classes.
    /// </summary>
    public interface IIndexer
    {
        /// <summary>
        /// Creates the index table.
        /// </summary>
        /// <param name="dictionary">The list of words to be searched in.</param>
        /// <returns>An instance of an index class with the index table constructed.</returns>
        WordIndexBase CreateIndex(string[] dictionary);
    }
}

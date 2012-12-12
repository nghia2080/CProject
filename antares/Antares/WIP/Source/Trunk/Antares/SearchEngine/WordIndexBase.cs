namespace SearchEngine
{
    /// <summary>
    /// An index class which index words only.
    /// </summary>
    public abstract class WordIndexBase
    {
        #region PRIVATE MEMBERS

        /// <summary>
        /// The dictionary.
        /// </summary>
        private readonly string[] _dictionary;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of the WordIndexBase class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        protected WordIndexBase(string[] dictionary)
        {
            _dictionary = dictionary;
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets the dictionary.
        /// </summary>
        public string[] Dictionary
        {
            get { return _dictionary; }
        }

        #endregion

    }

}

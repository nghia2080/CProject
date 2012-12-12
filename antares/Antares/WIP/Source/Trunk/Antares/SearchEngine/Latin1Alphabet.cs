namespace SearchEngine
{
    /// <summary>
    /// An English alphabet. Does not contain uppercase letters and digits.
    /// </summary>
    public class Latin1Alphabet : SimpleAlphabet
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of the EnglishAlphabet class.
        /// </summary>
        public Latin1Alphabet()
            : base('À', 'ý') { }

        #endregion
    }
}

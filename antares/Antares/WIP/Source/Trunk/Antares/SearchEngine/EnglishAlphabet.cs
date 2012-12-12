namespace SearchEngine
{
    /// <summary>
    /// An English alphabet. Does not contain uppercase letters and digits.
    /// </summary>
    public class EnglishAlphabet : SimpleAlphabet
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of the EnglishAlphabet class.
        /// </summary>
        public EnglishAlphabet()
            : base('a', 'z') { }

        #endregion
    }
}

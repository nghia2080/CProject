namespace SearchEngine
{
    /// <summary>
    /// An alphabet contains only digits.
    /// </summary>
    public class DigitAlphabet : SimpleAlphabet
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of the DigitAlphabet class.
        /// </summary>
        public DigitAlphabet()
            : base('0', '9') { }

        #endregion
    }
}

namespace SearchEngine
{
    public abstract class Metric
    {
        #region PUBLIC METHODS

        /// <summary>
        /// Gets the distance between 2 words (measures the similarity of them).
        /// Unlimited distance.
        /// </summary>
        /// <param name="first">The first word.</param>
        /// <param name="second">The second word.</param>
        /// <returns>An integer denotes the distance between 2 provided words.</returns>
        public int GetDistance(string first, string second)
        {
            return GetDistance(first, second, -1);
        }

        /// <summary>
        /// Gets the distance between 2 words (measures the similarity of them).
        /// </summary>
        /// <param name="first">The first word.</param>
        /// <param name="second">The second word.</param>
        /// <param name="max">The maximum allowed distance.</param>
        /// <returns>An integer denotes the distance between 2 provided words.</returns>
        public abstract int GetDistance(string first, string second, int max);

        /// <summary>
        /// Calculates the distance between the prefix pattern and a string 
        /// to find the distance between the specified prefix and nearest string prefix.
        /// Unlimited distance.
        /// Prefix Damerau-Levenshtein distance is calculated for the asymptotic time 
        /// O ((max + 1) * min (prefix.Length, string.Length))
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns>An integer denotes the distance between the prefix pattern and the string.</returns>
        public int GetPrefixDistance(string str, string prefix)
        {
            return GetPrefixDistance(str, prefix, -1);
        }

        /// <summary>
        /// Calculates the distance between the prefix pattern and a string 
        /// to find the distance between the specified prefix and nearest string prefix.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="max">The maximum allowed distance.</param>
        /// <returns>An integer denotes the distance between the prefix pattern and the string.</returns>
        public abstract int GetPrefixDistance(string str, string prefix, int max);

        /// <summary>
        /// Gets the distance between 2 words (measures the similarity of them).
        /// Unlimited distance.
        /// </summary>
        /// <param name="first">The first word.</param>
        /// <param name="second">The second word.</param>
        /// <param name="prefix">True if the second string is a prefix pattern. Otherwise false.</param>
        /// <returns>An integer denotes the distance between 2 provided words.</returns>
        public int GetDistance(string first, string second, bool prefix)
        {
            return prefix ? GetPrefixDistance(first, second) : GetDistance(first, second);
        }

        /// <summary>
        /// Gets the distance between 2 words (measures the similarity of them).
        /// </summary>
        /// <param name="first">The first word.</param>
        /// <param name="second">The second word.</param>
        /// <param name="max">The maximum allowed distance.</param>
        /// <param name="prefix">True if the second string is a prefix pattern. Otherwise false.</param>
        /// <returns>An integer denotes the distance between 2 provided words.</returns>
        public int GetDistance(string first, string second, int max, bool prefix)
        {
            return prefix ? GetPrefixDistance(first, second, max) : GetDistance(first, second, max);
        }

        #endregion
    }
}

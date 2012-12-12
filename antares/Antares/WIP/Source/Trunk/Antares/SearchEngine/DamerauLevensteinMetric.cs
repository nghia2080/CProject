using System;

namespace SearchEngine
{
    public class DamerauLevenshteinMetric : Metric
    {
        #region PRIVATE MEMBERS

        /// <summary>
        /// Default word length.
        /// </summary>
        private const int DEFAULT_LENGTH = 255;

        /// <summary>
        /// The current, to-be-calculated row.
        /// </summary>
        private int[] _currentRow;

        /// <summary>
        /// The last calculated row.
        /// </summary>
        private int[] _previousRow;

        /// <summary>
        /// The second last calculated row.
        /// </summary>
        private int[] _transpositionRow;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of the DamerauLevensteinMetric class. Default maximum length of all words is 255.
        /// </summary>
        public DamerauLevenshteinMetric()
            : this(DEFAULT_LENGTH) { }

        /// <summary>
        /// Initializes a new instance of the DamerauLevensteinMetric class.
        /// </summary>
        /// <param name="maxLength">The maximum length of all words.</param>
        public DamerauLevenshteinMetric(int maxLength)
        {
            _currentRow = new int[maxLength + 1];
            _previousRow = new int[maxLength + 1];
            _transpositionRow = new int[maxLength + 1];
        }

        #endregion

        #region PUBLIC METHODS

        /// <summary>
        /// Gets the distance between 2 words (measures the similarity of them).
        /// Damerau-Levenshtein distance is calculated for the asymptotic running time 
        /// O ((max + 1) * min (first.Length, second.Length))
        /// </summary>
        /// <param name="first">The first word.</param>
        /// <param name="second">The second word.</param>
        /// <param name="max">The maximum allowed distance.</param>
        /// <returns>An integer denotes the distance between 2 provided words.</returns>
        public override int GetDistance(string first, string second, int max)
        {
            int firstLength = first.Length;
            int secondLength = second.Length;

            if (firstLength == 0) return secondLength;
            if (secondLength == 0) return firstLength;

            // Makes sure the second string is no shorter than the first one.
            if (firstLength > secondLength)
            {
                string tmp = first;
                first = second;
                second = tmp;
                firstLength = secondLength;
                secondLength = second.Length;
            }

            if (max < 0) max = secondLength;
            if (secondLength - firstLength > max) return max + 1;

            if (firstLength > _currentRow.Length)
            {
                _currentRow = new int[firstLength + 1];
                _previousRow = new int[firstLength + 1];
                _transpositionRow = new int[firstLength + 1];
            }

            // Using dynamic programming, calculates the distance of every pair of prefixes of the 2 strings.
            // Xt
            //    Xs  Xd
            //    Xi  Dij
            // Dij = min(Xi + 1, Xd + 1, Xs + Csub, Xt + Ctran)
            // Csub = first[i] == second[j] ? 0 : 1;
            // Ctran = (first[i] == second[j-1]) & (first[i-1] == second[j]) ? 1 : INF;
            for (int i = 0; i <= firstLength; i++)
                _previousRow[i] = i;

            var lastSecondCh = (char)0;
            for (int i = 1; i <= secondLength; i++)
            {
                char secondCh = second[i - 1];
                _currentRow[0] = i;

                // Computes only the diagonal strip of width 2 * (max + 1)
                int from = Math.Max(i - max - 1, 1);
                int to = Math.Min(i + max + 1, firstLength);

                var lastFirstCh = (char)0;
                for (int j = from; j <= to; j++)
                {
                    char firstCh = first[j - 1];

                    // Calculates the minimum price the transition to the current state of the preceding of 
                    // deletion, insertion and replacement, respectively. (Csub)
                    int cost = firstCh == secondCh ? 0 : 1;
                    int value = Math.Min(Math.Min(_currentRow[j - 1] + 1, _previousRow[j] + 1), _previousRow[j - 1] + cost);

                    // If the typist suddenly had a transposition, we must also take into account its cost.
                    if (firstCh == lastSecondCh && secondCh == lastFirstCh)
                        value = Math.Min(value, _transpositionRow[j - 2] + cost);

                    _currentRow[j] = value;
                    lastFirstCh = firstCh;
                }
                lastSecondCh = secondCh;

                int[] tempRow = _transpositionRow;
                _transpositionRow = _previousRow;
                _previousRow = _currentRow;
                _currentRow = tempRow;
            }
            return _previousRow[firstLength];
        }

        /// <summary>
        /// Calculates the distance between the prefix pattern and a string 
        /// to find the distance between the specified prefix and nearest string prefix.
        /// Prefix Damerau-Levenshtein distance is calculated for the asymptotic time 
        /// O ((max + 1) * min (prefix.Length, string.Length))
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="max">The maximum allowed distance.</param>
        /// <returns>An integer denotes the distance between the prefix pattern and the string.</returns>
        public override int GetPrefixDistance(string str, string prefix, int max)
        {
            int prefixLength = prefix.Length;
            if (max < 0) max = prefixLength;
            int stringLength = Math.Min(str.Length, prefix.Length + max);

            if (prefixLength == 0) return 0;
            if (stringLength == 0) return prefixLength;

            if (stringLength < prefixLength - max) return max + 1;

            if (prefixLength > _currentRow.Length)
            {
                _currentRow = new int[prefixLength + 1];
                _previousRow = new int[prefixLength + 1];
                _transpositionRow = new int[prefixLength + 1];
            }

            for (int i = 0; i <= prefixLength; i++)
                _previousRow[i] = i;

            int distance = int.MaxValue;

            var lastStringCh = (char)0;
            for (int i = 1; i <= stringLength; i++)
            {
                char stringCh = str[i - 1];
                _currentRow[0] = i;

                // Compute only the diagonal strip of width 2 * (max + 1).
                int from = Math.Max(i - max - 1, 1);
                int to = Math.Min(i + max + 1, prefixLength);

                var lastPrefixCh = (char)0;
                for (int j = from; j <= to; j++)
                {
                    char prefixCh = prefix[j - 1];

                    // Calculate the minimum price the transition to the current state of the preceding of 
                    // deletion, insertion and replacement, respectively.
                    int cost = prefixCh == stringCh ? 0 : 1;
                    int value = Math.Min(Math.Min(_currentRow[j - 1] + 1, _previousRow[j] + 1), _previousRow[j - 1] + cost);

                    // If the typist suddenly had a transposition, we must also take into account its cost.
                    if (prefixCh == lastStringCh && stringCh == lastPrefixCh)
                        value = Math.Min(value, _transpositionRow[j - 2] + cost);

                    _currentRow[j] = value;
                    lastPrefixCh = prefixCh;
                }
                lastStringCh = stringCh;

                // Computes the minimum distance from the given prefix to all prefix that differ from no more than max
                if (i >= prefixLength - max && i <= prefixLength + max && _currentRow[prefixLength] < distance)
                    distance = _currentRow[prefixLength];

                int[] tempRow = _transpositionRow;
                _transpositionRow = _previousRow;
                _previousRow = _currentRow;
                _currentRow = tempRow;
            }

            return distance;
        }

        #endregion
    }
}

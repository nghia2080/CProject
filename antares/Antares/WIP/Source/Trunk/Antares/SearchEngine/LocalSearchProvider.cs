using System;
using System.Collections.Generic;
using System.Linq;
using AntaresShell.BaseClasses;
using SearchEngine.Interfaces;

namespace SearchEngine
{
    /// <summary>
    /// A class provides methods to search local objects.
    /// </summary>
    public class LocalSearchProvider : ISearchProvider
    {
        /// <summary>
        /// List of tokens used to separated words.
        /// </summary>
        private readonly char[] _separators;

        /// <summary>
        /// List of all to-be-indexed documents.
        /// </summary>
        private volatile List<SearchableBaseModel> _documents;

        /// <summary>
        /// An inverted index hash table to be used in search process.
        /// </summary>
        private Dictionary<string, List<KeyValuePair<int, int>>> _indexTable;

        /// <summary>
        /// List of possible results to be sorted and checked.
        /// </summary>
        private List<int> _lookupList;

        /// <summary>
        /// Initializes a new instance of the LocalSearchProvider class.
        /// </summary>
        public LocalSearchProvider()
        {
            _separators = new[]
                                {
                                    ' ', '!', '"', '#', '$', '%', '&', '\'',
                                    '(', ')', '*', '+', '-', '.', '/', ':',
                                    ';', '<', '=', '>', '?', '@', '[', ']',
                                    '\\', '^', '_', '`', '~', '{', '}', '|',
                                    ',', '\n', '\t', '\r'
                                };
        }

        /// <summary>
        /// Returns the search results for the specified query.
        /// </summary>
        /// <param name="query">The query string.</param>
        /// <param name="documentList">List contains all items for search.</param>
		/// <param name="needIndexing">Condition to indexing.</param>
        /// <returns>Search results as a list of objects.</returns>
        public List<SearchableBaseModel> GetResults(string query, List<SearchableBaseModel> documentList, bool needIndexing)
        {
            _documents = documentList;
            if (needIndexing)
            {
                Indexing();
            }

            query = NormalizeString(query);
            var results = new List<SearchableBaseModel>();
            var tokens = query.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
            _lookupList = new List<int>();

            foreach (var token in tokens)
            {
                if (string.IsNullOrEmpty(token))
                {
                    continue;
                }

                if (token.Length == 1)
                {
                    ProcessLengthOneToken(token);
                    continue;
                }

                ProcessToken(token);
            }

            GetBestResults(results);

            return results;
        }

        /// <summary>
        /// Split token.
        /// </summary>
        /// <param name="token">Parse in token.</param>
        private void ProcessToken(string token)
        {
            int maxSize = 0;
            bool valid = true;
            var postingLists = new List<KeyValuePair<int, int>>[token.Length - 1];
            for (int i = 0; i < token.Length - 1; ++i)
            {
                string bigram = string.Empty + token[i] + token[i + 1];
                if (_indexTable.ContainsKey(bigram))
                {
                    postingLists[i] = new List<KeyValuePair<int, int>>(_indexTable[bigram]);
                    maxSize = Math.Max(maxSize, postingLists[i].Count);
                }
                else
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                ProcessValidToken(postingLists, token);
            }
        }

        /// <summary>
        /// Finds the documents which contains the most of the search query.
        /// </summary>
        /// <param name="results">The result list.</param>
        private void GetBestResults(List<SearchableBaseModel> results)
        {
            _lookupList.Sort();
            int maxMatch = 1;
            int currentDocument = -1;
            int count = 0;
            foreach (var t in _lookupList)
            {
                if (t != currentDocument)
                {
                    currentDocument = t;
                    count = 1;
                }
                else
                {
                    count++;
                }

                if (count == maxMatch)
                {
                    results.Add(_documents[currentDocument]);
                }
                else
                {
                    if (count > maxMatch)
                    {
                        results.Clear();
                        results.Add(_documents[currentDocument]);
                        maxMatch = count;
                    }
                }
            }
        }

        /// <summary>
        /// Processes tokens which all bi-grams are existed in the index table.
        /// </summary>
        /// <param name="postingLists">The posting list.</param>
        /// <param name="token">The token to be processed.</param>
        private void ProcessValidToken(List<KeyValuePair<int, int>>[] postingLists, string token)
        {
            var isChecked = new bool[_documents.Count];
            var currentPos = new int[token.Length - 1];
            if (postingLists.Length > 1)
            {
                Merge(postingLists, 0, currentPos, ref isChecked, new KeyValuePair<int, int>(-1, -1));
            }
            else
            {
                if (postingLists.Length == 1)
                {
                    foreach (var appearance in postingLists[0].Where(appearance => !isChecked[appearance.Key]))
                    {
                        _lookupList.Add(appearance.Key);
                        isChecked[appearance.Key] = true;
                    }
                }
            }
        }

        /// <summary>
        /// Processes tokens which contain only 1 character.
        /// </summary>
        /// <param name="token">The token to be processed.</param>
        private void ProcessLengthOneToken(string token)
        {
            var isChecked = new bool[_documents.Count];
            foreach (KeyValuePair<string, List<KeyValuePair<int, int>>> indexEntry in _indexTable)
            {
                if (indexEntry.Key[0].Equals(token[0]))
                {
                    foreach (KeyValuePair<int, int> appearance in indexEntry.Value)
                    {
                        if (!isChecked[appearance.Key])
                        {
                            _lookupList.Add(appearance.Key);
                            isChecked[appearance.Key] = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Normalizes a string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns>The string after nomarlization.</returns>
        private string NormalizeString(string source)
        {
            return source.Trim().ToLower();
        }

        /// <summary>
        /// Intersects the posting lists of each bigram by recursively checking each list to find an expected item.
        /// </summary>
        /// <param name="postingLists">The array of posting lists of each bigram.</param>
        /// <param name="row">The index of the current row.</param>
        /// <param name="currentPositions">The array which store indexes to be consider on each row.</param>
        /// <param name="isChecked">The array to be used when check whether a document was checked or not.</param>
        /// <param name="expectedPosition">The expected position of the appearance of the current-in-check bigram.</param>
        private void Merge(
                List<KeyValuePair<int, int>>[] postingLists,
                int row,
                int[] currentPositions,
                ref bool[] isChecked,
                KeyValuePair<int, int> expectedPosition)
        {
            // If went passed the end of row, go to the next item on the preceding row, then stop processing the current row.
            if (currentPositions[row] >= postingLists[row].Count)
            {
                if (row > 0)
                {
                    ++currentPositions[row - 1];
                }

                return;
            }

            while (currentPositions[row] < postingLists[row].Count)
            {
                // If the document has been checked, go to the next item.
                if (isChecked[postingLists[row][currentPositions[row]].Key])
                {
                    ++currentPositions[row];
                    continue;
                }

                // If this row is not the first row, then compare the current item with expected result.
                if (expectedPosition.Key != -1)
                {
                    // If the document ID is smaller than the expected document ID, go to the next item.
                    if (postingLists[row][currentPositions[row]].Key < expectedPosition.Key)
                    {
                        ++currentPositions[row];
                        continue;
                    }

                    // If the document ID is bigger than the expected document ID,
                    // that means the expected document does not contain the search phrase.
                    // Mark the expected document as checked, then stop processing the current row.
                    if (postingLists[row][currentPositions[row]].Key > expectedPosition.Key)
                    {
                        isChecked[expectedPosition.Key] = true;
                        return;
                    }

                    // If the position in the expected document is bigger than the expected position,
                    // check the next item on the preceding row, and stop processing the current row.
                    if (postingLists[row][currentPositions[row]].Value > expectedPosition.Value)
                    {
                        ++currentPositions[row - 1];
                        return;
                    }

                    // If the position in the expected document is smaller than the expected position, check the next item.
                    if (postingLists[row][currentPositions[row]].Value < expectedPosition.Value)
                    {
                        ++currentPositions[row];
                        continue;
                    }

                    // If the expected item is found, check if this is the last row.
                    // If yes, then the expected document does contain the token,
                    // so mark the expected document as checked and add it to the result list.
                    // If no, then continue by checking the next row.
                    if (row == postingLists.Length - 1)
                    {
                        isChecked[expectedPosition.Key] = true;
                        _lookupList.Add(expectedPosition.Key);
                        return;
                    }
                }

                // Merge with the next
                Merge(
                    postingLists,
                    row + 1,
                    currentPositions,
                    ref isChecked,
                    new KeyValuePair<int, int>(postingLists[row][currentPositions[row]].Key, postingLists[row][currentPositions[row]].Value + 1));
            }
        }

        /// <summary>
        /// Creates the index table.
        /// </summary>
        private void Indexing()
        {
            _indexTable = new Dictionary<string, List<KeyValuePair<int, int>>>();
            for (int docID = 0; docID < _documents.Count; ++docID)
            {
                string data = NormalizeString(_documents[docID].ToSearchableString());
                for (int i = 0; i < data.Length; ++i)
                {
                    if (_separators.Contains(data[i]))
                    {
                        continue;
                    }

                    string token;
                    if (i < data.Length - 1)
                    {
                        if (_separators.Contains(data[i + 1]))
                        {
                            token = string.Empty + data[i] + "\v";
                            if (_indexTable.ContainsKey(token))
                            {
                                _indexTable[token].Add(new KeyValuePair<int, int>(docID, i));
                            }
                            else
                            {
                                _indexTable.Add(token, new List<KeyValuePair<int, int>> { new KeyValuePair<int, int>(docID, i) });
                            }
                            ++i;
                            continue;
                        }

                        token = string.Empty + data[i] + data[i + 1];
                        if (_indexTable.ContainsKey(token))
                        {
                            _indexTable[token].Add(new KeyValuePair<int, int>(docID, i));
                        }
                        else
                        {
                            _indexTable.Add(token, new List<KeyValuePair<int, int>> { new KeyValuePair<int, int>(docID, i) });
                        }
                    }
                    else
                    {
                        if (!_separators.Contains(data[i]))
                        {
                            token = string.Empty + data[i] + "\v";
                            if (_indexTable.ContainsKey(token))
                            {
                                _indexTable[token].Add(new KeyValuePair<int, int>(docID, i));
                            }
                            else
                            {
                                _indexTable.Add(token, new List<KeyValuePair<int, int>> { new KeyValuePair<int, int>(docID, i) });
                            }
                            ++i;
                        }
                    }
                }
            }
        }
    }
}
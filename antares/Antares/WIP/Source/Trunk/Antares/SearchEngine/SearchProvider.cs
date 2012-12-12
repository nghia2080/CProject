using System.Collections.Generic;
using AntaresShell.BaseClasses;
using SearchEngine.Interfaces;

namespace SearchEngine
{
    public static class SearchProvider
    {
        public static void CreateIndex(string[] dictionary)
        {
            Index = new NGramIndexerM1(new UnionAlphabet(new EnglishAlphabet(), 
                                                        new DigitAlphabet(),
                                                        new Latin1Alphabet(),
                                                        new LatinExtendedAAlphabet(),
                                                        new LatinExtendeBAlphabet(),
                                                        new LatinExtendedAdditionalAlphabet(),
                                                        new HiraganaAlphabet(),
                                                        new KatakanaAlphabet(), 
                                                        new RomajiAlphabet(),
                                                        new CommonKanjiAlphabet())).CreateIndex(dictionary);
        }

        public static void CreateSearcher(WordIndexBase index)
        {
            Searcher = new NGramSearcherM1((NGramIndexM1)index, new DamerauLevenshteinMetric(), 2, false);
        }
        static SearchProvider()
        {
            IsDirty = true;
        }

        public static WordIndexBase Index { get; private set; }
        public static ISearcher Searcher { get; private set; }
        public static SearchableBaseModel[] ItemList { get; set; }
        public static List<int>[] MapStringItem { get; set; }
        public static bool IsDirty { get; set; }
    }
}

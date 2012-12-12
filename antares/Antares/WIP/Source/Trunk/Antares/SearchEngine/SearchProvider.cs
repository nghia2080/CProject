using SearchEngine.Interfaces;

namespace SearchEngine
{
    public class SearchProvider
    {
        public static WordIndexBase CreateIndex(string[] dictionary)
        {
            return new NGramIndexerM1(new UnionAlphabet(new EnglishAlphabet(), 
                                                        new DigitAlphabet(),
                                                        new Latin1Alphabet(),
                                                        new LatinExtendedAAlphabet(),
                                                        new LatinExtendeBAlphabet(),
                                                        new LatinExtendedAdditionalAlphabet(),
                                                        new HiraganaAlphabet(),
                                                        new KatakanaAlphabet(), 
                                                        new RomajiAlphabet(),
                                                        new CommonKanjiAlphabet(),
                                                        new RareKanjiAlphabet())).CreateIndex(dictionary);
        }

        public static ISearcher CreateSearcher(WordIndexBase index)
        {
            return new NGramSearcherM1((NGramIndexM1)index, new DamerauLevenshteinMetric(), 2, false);
        }

    }
}

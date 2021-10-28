using System.Collections.Generic;
using System.Linq;
using Kelebron.Utils.Japanese;

namespace Shiro.Library.FuriganaView
{
    public class JapaneseTextTokenizer
    {
        public static List<string> Tokenize(string text)
        {
            if (text != null)
                return text.Split(' ').ToList();
            return null;
        }

        public static List<MeCabData> TokenizeAsMeCabData(string text)
        {
            if (text != null)
            {
                List<MeCabData> meCabDatas = MecabAnalyzer.AnalyzeModel(text);
                foreach (MeCabData meCabData in meCabDatas)
                {
                    bool containsKanji = JapaneseKanaClassifier.ContainsKanji(meCabData.Token);
                    bool isKatakana = JapaneseKanaClassifier.IsKatakana(meCabData.Pronounciation);
                    if (containsKanji && isKatakana)
                        meCabData.Pronounciation = JapaneseKanaConverter.KatakanaToHiragana(meCabData.Pronounciation);
                    else if (meCabData.Pronounciation != "々")
                        meCabData.Pronounciation = "";
                }
                return meCabDatas;
            }
            return new List<MeCabData>();
        }
    }
}
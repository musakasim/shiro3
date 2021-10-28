//  Author: Paweł Szczepański (kelebron@gmail.com)
//  January 2010
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

//http://kdev.yum.pl/kanjimap/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using log4net;

namespace Kelebron.Utils.Japanese
{
    /// <summary>
    ///     Convert from romaji to kana and between kana syllabaries.
    ///     It allocates dictionary of about 200 strings (max 3 characters).
    /// </summary>
    public class JapaneseKanaConverter
    {
        //  static ILog log = LogManager.GetLogger(typeof(JapaneseKanaConverter));

        private static Dictionary<char, char> _longToShortVovelDictionary;

        /// <summary>
        ///     Dictionary filled on first use from hardcoded translation array.
        ///     It maps romaji syllabe to katakana characters.
        /// </summary>
        private static Dictionary<string, string> _romajiKanaDictionary;

        static JapaneseKanaConverter()
        {
            InitRomajiKanaDictionary();
            InitLongToShortVovelDictionary();
        }

        public static string RomajiToKatakana(string romaji)
        {
            List<string> syllabes = SplitRomajiToSyllabes(romaji);
            var resultBuilder = new StringBuilder(syllabes.Count);
            foreach (string syllabe in syllabes)
            {
                if (!_romajiKanaDictionary.ContainsKey(syllabe))
                    throw new FormatException("Unexpected syllabe " + syllabe);

                resultBuilder.Append(_romajiKanaDictionary[syllabe]);
            }
            return resultBuilder.ToString();
        }

        public static string RomajiToHiragana(string romaji)
        {
            string result = KatakanaToHiragana(RomajiToKatakana(romaji));
            // Some last fixes
            result = result.Replace("くぁ", "くゎ"); // kwa
            result = result.Replace("どぅ", "づ"); // du
            result = result.Replace("ぐぁ", "ぐゎ"); // kwa
            result = result.Replace("すぃ", "し"); // si

            if (!JapaneseKanaClassifier.IsHiragana(result))
                throw new FormatException("The argument is not valid japanese romaji (" + romaji + ")");

            return result;
        }

        public static bool TryParseRomajiToHiragana(string romaji, out string hiragana)
        {
            if (!JapaneseKanaClassifier.IsRomaji(romaji))
            {
                hiragana = romaji;
                return false;
            }

            try
            {
                hiragana = RomajiToHiragana(romaji);
                return true;
            }
            catch
            {
                hiragana = romaji;
                return false;
            }
        }

        public static string KatakanaToHiragana(string katakana)
        {
            if (JapaneseKanaClassifier.IsHalfwidthKatakana(katakana))
                katakana = katakana.Normalize(NormalizationForm.FormKC);

            if (!JapaneseKanaClassifier.IsFullwidthKatakana(katakana))
                throw new FormatException("The given string is not valid katakana.");

            var resultBuilder = new StringBuilder(katakana.Length);
            foreach (char character in katakana)
            {
                resultBuilder.Append((char) (character + (0x3040 - 0x30A0)));
            }
            return resultBuilder.ToString();
        }

        public static string HiraganaToKatakana(string hiragana)
        {
            if (!JapaneseKanaClassifier.IsHiragana(hiragana))
                throw new FormatException("The given string is not valid hiragana.");

            var resultBuilder = new StringBuilder(hiragana.Length);
            foreach (char character in hiragana)
            {
                resultBuilder.Append((char) (character - (0x3040 - 0x30A0)));
            }
            return resultBuilder.ToString();
        }

        private static bool IsVovel(char c)
        {
            return "aiueoāīūēō".Contains(c);
        }

        private static bool IsLongVovel(char c)
        {
            return "āīūēō".Contains(c);
        }

        private static List<string> SplitRomajiToSyllabes(string romaji)
        {
            int start = 0;
            var result = new List<string>();
            for (int i = 0; i < romaji.Length; ++i)
            {
                if (IsVovel(romaji[i]) || romaji[i] == '-')
                {
                    // Preprocessing for long vovels: splt each to two single vovels.
                    // Exceptions: ō becomes ou when syllabe starts with a consonant.
                    if (IsLongVovel(romaji[i]))
                    {
                        char shortVovel = _longToShortVovelDictionary[romaji[i]];
                        result.Add(romaji.Substring(start, i - start) + shortVovel);
                        if (shortVovel == 'o' && !IsVovel(romaji[start]))
                            result.Add("u");
                        else
                            result.Add(shortVovel.ToString());
                    }
                    else
                    {
                        result.Add(romaji.Substring(start, i - start + 1));
                    }
                    start = i + 1;
                }
                else if ((i > 0 && (romaji[i - 1] == 'n' || romaji[i - 1] == 'm') && romaji[i] != 'y') ||
                         (i > 0 && romaji[i - 1] == romaji[i]))
                {
                    result.Add(romaji.Substring(start, i - start));
                    start = i;
                }
                if (char.IsWhiteSpace(romaji[i]))
                {
                    start = i + 1;
                }
            }
            if (start < romaji.Length)
            {
                result.Add(romaji.Substring(start, romaji.Length - start));
            }
            // Handle consonant repetitions.
            for (int i = 1; i < result.Count; ++i)
            {
                if (result[i - 1].Length == 1 &&
                    !IsVovel(result[i - 1][0]) &&
                    result[i - 1][0] != 'n' && result[i - 1][0] == result[i][0])
                {
                    result[i - 1] = "tsuXS";
                }
            }
            return result;
        }

        private static void InitRomajiKanaDictionary()
        {
            #region romajiKanaPairs

            string[] romajiKanaPairs =
            {
                "ア" , "a",
                "イ" , "i",
                "ウ" , "u",
                "エ" , "e",
                "オ" , "o",
                "カ" , "ka",
                "キ" , "ki",
                "ク" , "ku",
                "ケ" , "ke",
                "コ" , "ko",
                "キャ", "kya",
                "キュ", "kyu",
                "キョ", "kyo",
                "サ" , "sa",
                "シ" , "shi",
                "ス" , "su",
                "セ" , "se",
                "ソ" , "so",
                "シャ", "sha",
                "シュ", "shu",
                "ショ", "sho",
                "タ" , "ta",
                "チ" , "chi",
                "ツ" , "tsu",
                "テ" , "te",
                "ト" , "to",
                "チャ", "cha",
                "チュ", "chu",
                "チョ", "cho",
                "ナ" , "na",
                "ニ" , "ni",
                "ヌ" , "nu",
                "ネ" , "ne",
                "ノ" , "no",
                "ニャ", "nya",
                "ニュ", "nyu",
                "ニョ", "nyo",
                "ハ" , "ha",
                "ヒ" , "hi",
                "フ" , "fu",
                "ヘ" , "he",
                "ホ" , "ho",
                "ヒャ", "hya",
                "ヒュ", "hyu",
                "ヒョ", "hyo",
                "マ" , "ma",
                "ミ" , "mi",
                "ム" , "mu",
                "メ" , "me",
                "モ" , "mo",
                "ミャ", "mya",
                "ミュ", "myu",
                "ミョ", "myo",
                "ヤ" , "ya",
                "ユ" , "yu",
                "ヨ" , "yo",
                "ラ" , "ra",
                "リ" , "ri",
                "ル" , "ru",
                "レ" , "re",
                "ロ" , "ro",
                "リャ", "rya",
                "リュ", "ryu",
                "リョ", "ryo",
                "ワ" , "wa",
                "ヲ" , "wo",
                "ン" , "n",
                "ン" , "m",
                "ガ" , "ga",
                "ギ" , "gi",
                "グ" , "gu",
                "ゲ" , "ge",
                "ゴ" , "go",
                "ギャ", "gya",
                "ギュ", "gyu",
                "ギョ", "gyo",
                "ザ" , "za",
                "ジ" , "ji",
                "ズ" , "zu",
                "ゼ" , "ze",
                "ゾ" , "zo",
                "ジャ", "ja",
                "ジュ", "ju",
                "ジョ", "jo",
                "ジャ", "jya",
                "ジュ", "jyu",
                "ジョ", "jyo",
                "ダ" , "da",
                "デ" , "de",
                "ド" , "do",
                "バ" , "ba",
                "ビ" , "bi",
                "ブ" , "bu",
                "ベ" , "be",
                "ボ" , "bo",
                "ビャ", "bya",
                "ビュ", "byu",
                "ビョ", "byo",
                "パ" , "pa",
                "ピ" , "pi",
                "プ" , "pu",
                "ペ" , "pe",
                "ポ" , "po",
                "ピャ", "pya",
                "ピュ", "pyu",
                "ピョ", "pyo",
                "イィ", "yi ",
                "イェ", "ye",
                "ヴァ", "va",
                "ヴィ", "vi",
                "ヴ" , "vu",
                "ヴェ", "ve",
                "ヴォ", "vo",
                "ヴャ", "vya",
                "ヴュ", "vyu",
                "ヴョ", "vyo",
                "シェ", "she",
                "ジェ", "je",
                "チェ", "che",
                "スァ", "swa",
                "スィ", "si",
                "スゥ", "swu",
                "スェ", "swe",
                "スォ", "swo",
                "スャ", "sya",
                "スュ", "syu",
                "スョ", "syo",
                "ズァ", "zwa",
                "ズィ", "zi",
                "ズゥ", "zwu",
                "ズェ", "zwe",
                "ズォ", "zwo",
                "ズャ", "zya",
                "ズュ", "zyu",
                "ズョ", "zyo",
                "ツァ", "tsa",
                "ツィ", "tsi",
                "ツェ", "tse",
                "ツォ", "tso",
                "テァ", "tha",
                "ティ", "ti",
                "テゥ", "thu",
                "テェ", "tye",
                "テォ", "tho",
                "テャ", "tya",
                "テュ", "tyu",
                "テョ", "tyo",
                "デァ", "dha",
                "ディ", "di",
                "デゥ", "dhu",
                "デェ", "dye",
                "デォ", "dho",
                "デャ", "dya",
                "デュ", "dyu",
                "デョ", "dyo",
                "トァ", "twa",
                "トィ", "twi",
                "トゥ", "tu",
                "トェ", "twe",
                "トォ", "two",
                "ドァ", "dwa",
                "ドィ", "dwi",
                "ドゥ", "du",
                "ドェ", "dwe",
                "ドォ", "dwo",
                "ファ", "fa",
                "フィ", "fi",
                "ホゥ", "hu",
                "フェ", "fe",
                "フォ", "fo",
                "フャ", "fya",
                "フュ", "fyu",
                "フョ", "fyo",
                "リィ", "ryi",
                "リェ", "rye",
                "ウィ", "wi",
                "ウゥ", "wu",
                "ウェ", "we",
                "ウャ", "wya",
                "ウュ", "wyu",
                "ウョ", "wyo",
                "クァ", "kwa",
                "クィ", "kwi",
                "クゥ", "kwu",
                "クェ", "kwe",
                "クォ", "kwo",
                "グァ", "gwa",
                "グィ", "gwi",
                "グゥ", "gwu",
                "グェ", "gwe",
                "グォ", "gwo",
                "ムァ", "mwa",
                "ムィ", "mwi",
                "ムゥ", "mwu",
                "ムェ", "mwe",
                "ムォ", "mwo",
                "ッ" , "tsuXS",
                "ー" , "-"
            };

            #endregion

            _romajiKanaDictionary = new Dictionary<string, string>(romajiKanaPairs.Length/2);
            for (int i = 0; i < romajiKanaPairs.Length; i += 2)
            {
                string kana = romajiKanaPairs[i];
                string romaji = romajiKanaPairs[i + 1];
                if (!_romajiKanaDictionary.ContainsKey(romaji))
                    //  log.Warn("Duplicate kana for " + romaji);
                    _romajiKanaDictionary[romaji] = kana;
            }
        }

        private static void InitLongToShortVovelDictionary()
        {
            const string longVovels = "āīūēō";
            const string shortVovels = "aiueo";
            _longToShortVovelDictionary = new Dictionary<char, char>(longVovels.Length);
            for (int i = 0; i < longVovels.Length; ++i)
            {
                _longToShortVovelDictionary[longVovels[i]] = shortVovels[i];
            }
        }
    }
}
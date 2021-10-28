using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FrInterfaces;
using Shiro.Controller;

namespace Shiro.Model
{
    /// <summary>
    /// This is the data model for dictionary entries
    /// Contains detailed or compiled infos about an entry
    /// easy to use with bindings etc.
    /// </summary>
    public class ShiroEntryBzzt : ShiroEntry
    {
        private ObservableCollection<BookmarkCollection> _bookmarks;

        //todo:COOOK ACİİİL: bu 2 prop buradan kalkacak!!!!!
        private static IKanjiInfoController KanjiInfoController { get; set; }
        private static IBookmarkController BookmarkController { get; set; }

        public static void SetControllerProperties(IKanjiInfoController kanjiInfoController,IBookmarkController bookmarkController)
        {
            KanjiInfoController = kanjiInfoController;
            BookmarkController = bookmarkController;
        }

        public ShiroEntryBzzt()
        {
            
        }

        public ShiroEntryBzzt(ShiroEntry shiroEntry)
        {
            Id = shiroEntry.Id;
            KanjiChars = shiroEntry.KanjiChars;
            Meanings = shiroEntry.Meanings;
            Spellings = shiroEntry.Spellings;
            GroupedSpellings = shiroEntry.GroupedSpellings;
        }

        private int? _score { get; set; }
        /// <summary>
        /// //todo:must be actual model, all data should be calculated on dbpopulation
        /// A calculated frequency score 
        /// small values are more frequent
        /// </summary>
        public int Score
        {
            get
            {
                if (_score == null)
                    _score = Kanjis.Where(f => f.Frequency > 0).Select(t => t.Frequency).Min();
                return _score.Value;
            }
        }

        public ObservableCollection<BookmarkCollection> BookmarkCollections
        {
            get
            {
                if (_bookmarks == null)
                {
                    if (BookmarkController != null)
                    {
                        var a = BookmarkController.GetBookmarksOfData(Id, BookmarkType.ShiroEntryBookmark).ToList();
                        _bookmarks = new ObservableCollection<BookmarkCollection>(a);
                    }
                    else
                    {
                        _bookmarks = new ObservableCollection<BookmarkCollection>(new List<BookmarkCollection>());
                    }
                }
                return _bookmarks;
            }
            set { _bookmarks = value; }
        }

        private List<KanjiInfo> _kanjis { get; set; }

        /// <summary>
        ///     Contained Kanji's in any spelling of the entry
        /// </summary>
        public List<KanjiInfo> Kanjis
        {
            get { return _kanjis ?? (_kanjis = KanjiChars.Select(k => KanjiInfoController != null ? KanjiInfoController.Search(k).FirstOrDefault() : null).Where(t => t != null).ToList()); }
        }

        /// <summary>
        ///     ReadingElement(KanjiElement1, KanjiElement2 , KanjiElement3 , KanjiElement4 ) seklinde ifade icin gruplama
        /// </summary>
        public List<Tuple<List<string>, List<string>>> GroupedSpellingsTuple
        {
            get
            {
                return GroupedSpellings.Select(p => new Tuple<List<string>, List<string>>(p[0], p[1])).ToList();
            }
        }

        public void SetGroupedSpellings(List<Tuple<List<string>, List<string>>> tuples)
        {
            GroupedSpellings = tuples.Select(o => new List<List<string>> { o.Item1, o.Item2 }).ToList();
        }
    }
    public class ShiroEntry : IBaseModel
    {
        public List<string> KanjiChars { get; set; }

        //(EntSeq)
        //ReadingElement

        /// <summary>
        ///     orthography
        /// </summary>
        public List<Spelling> Spellings { get; set; } //KanjiElement &   ReadingElement

        /// <summary>
        ///     The sense element will record the translational equivalent of the Japanese word, plus other related information.
        ///     Where there are several distinctly different meanings of the word, multiple sense elements will be employed.
        ///     Meaning by context
        /// </summary>
        public List<Meaning> Meanings { get; set; } //Sense

        public List<List<List<string>>> GroupedSpellings { get; set; }

        /// <summary>
        ///     A unique numeric sequence number for each entry
        /// </summary>
        public int Id { get; set; }

        public class Meaning
        {
            //todo : also consider following fields: SInf, Lsource, Example,Field
            public int Index { get; set; }

            /// <summary>
            ///     For words specifically associated with regional dialects in Japanese, the entity code for that dialect, e.g. ksb
            ///     for Kansaiben.
            /// </summary>
            public string Dial { get; set; }

            /// <summary>
            ///     Within each sense will be one or more "glosses", i.e. target-language words or phrases which are equivalents to the
            ///     Japanese word.
            ///     This element would normally be present, however it may be omitted in entries which are purely for a
            ///     cross-reference.
            /// </summary>
            public List<string> Gloss { get; set; } //Gloss: Dictionary<Lang,Text>    

            /// <summary>
            ///     Part-of-speech information about the entry/sense. Should use appropriate entity codes.
            ///     In general where there are multiple senses in an entry,
            ///     the part-of-speech of an earlier sense will apply to later senses unless there is a new part-of-speech indicated.
            /// </summary>
            public List<string> PartOfSpeech { get; set; } //Pos  //todo:enum olacak

            /// <summary>
            ///     PartOfSpeech'de uzun uzun belirtilen aciklamalarin kisa halde belirtilmis hali
            ///     ornek:    noun (common) (futsuumeishi) ---> n
            ///     //todo:tum durumlar hazirlanacak
            /// </summary>
            public string PartOfSpeechShort
            {
                get
                {
                    string str = String.Empty;
                    if (PartOfSpeech.Any(t => t == "noun (common) (futsuumeishi)"))
                    {
                        str += "n";
                    }
                    return str;
                }
            }

            /// <summary>
            ///     This element is used to indicate a cross-reference to another entry with a similar or related meaning or sense.
            ///     The content of this element is typically a keb or reb element in another entry. In some cases a keb will be
            ///     followed by a reb and/or a sense number to provide
            ///     a precise target for the cross-reference. Where this happens, a JIS "centre-dot" (0x2126) is placed between the
            ///     components of the cross-reference.
            /// </summary>
            public string Xref { get; set; }

            /// <summary>
            ///     This element is used to indicate another entry which is an antonym of the current entry/sense.
            ///     The content of this element must exactly match that of a keb or reb element in another entry.
            /// </summary>
            public string Antonyms { get; set; } //Ant

            /// <summary>
            ///     This element is used for other relevant information about the entry/sense.
            ///     As with part-of-speech, information will usually apply to several senses.
            /// </summary>
            //todo: Lsource bilgisi de buraya islenecek(enum veya string olarak)
            public List<string> Misc { get; set; }
        }

        public class Spelling
        {
            // public JMKotobaSpellingType SpellingType { get; set; }
            public string Value { get; set; } //Keb & Reb: Reb+(Keb,Keb,Keb) seklinde olacak, ReRestr'lar yuzunden
            public string Priority { get; set; }
            // KePri & RePri //todo: bu tek bir (bu programa ozgu bir)siralamaya donusturulecek,enum yapilacak (- news1/2,- ichi1/2,- spec1 and spec2,- gai1/2,- nfxx)
            //todo: Spelling Keb'ler ve Reb'ler olarak ayrilacak galiba, cunku Information bagi kopuyor:
            public List<string> Restrictions { get; set; } //ReRestr
            public List<string> KanjiInformation { get; set; }
            //KeInf   //todo: var olan sekiller belirlenip enum yapilacak
            public List<string> KanaInformation { get; set; }
            //    ReInf //todo: var olan sekiller belirlenip enum yapilacak
        }
    }


    //<!ENTITY MA "martial arts term">
    //<!ENTITY X "rude or X-rated term (not displayed in educational software)">
    //<!ENTITY abbr "abbreviation">
    //<!ENTITY adj-i "adjective (keiyoushi)">
    //<!ENTITY adj-na "adjectival nouns or quasi-adjectives (keiyodoshi)">
    //<!ENTITY adj-no "nouns which may take the genitive case particle `no'">
    //<!ENTITY adj-pn "pre-noun adjectival (rentaishi)">
    //<!ENTITY adj-t "`taru' adjective">
    //<!ENTITY adj-f "noun or verb acting prenominally">
    //<!ENTITY adj "former adjective classification (being removed)">
    //<!ENTITY adv "adverb (fukushi)">
    //<!ENTITY adv-to "adverb taking the `to' particle">
    //<!ENTITY arch "archaism">
    //<!ENTITY ateji "ateji (phonetic) reading">
    //<!ENTITY aux "auxiliary">
    //<!ENTITY aux-v "auxiliary verb">
    //<!ENTITY aux-adj "auxiliary adjective">
    //<!ENTITY Buddh "Buddhist term">
    //<!ENTITY chem "chemistry term">
    //<!ENTITY chn "children's language">
    //<!ENTITY col "colloquialism">
    //<!ENTITY comp "computer terminology">
    //<!ENTITY conj "conjunction">
    //<!ENTITY ctr "counter">
    //<!ENTITY derog "derogatory">
    //<!ENTITY eK "exclusively kanji">
    //<!ENTITY ek "exclusively kana">
    //<!ENTITY exp "Expressions (phrases, clauses, etc.)">    seen in sense.pos
    //<!ENTITY fam "familiar language">
    //<!ENTITY fem "female term or language">
    //<!ENTITY food "food term">
    //<!ENTITY geom "geometry term">
    //<!ENTITY gikun "gikun (meaning as reading)  or jukujikun (special kanji reading)">
    //<!ENTITY hon "honorific or respectful (sonkeigo) language">
    //<!ENTITY hum "humble (kenjougo) language">
    //<!ENTITY iK "word containing irregular kanji usage">    seen in Keb.KeInf
    //<!ENTITY id "idiomatic expression">
    //<!ENTITY ik "word containing irregular kana usage">
    //<!ENTITY int "interjection (kandoushi)">
    //<!ENTITY io "irregular okurigana usage">
    //<!ENTITY iv "irregular verb">
    //<!ENTITY ling "linguistics terminology">
    //<!ENTITY m-sl "manga slang">
    //<!ENTITY male "male term or language">
    //<!ENTITY male-sl "male slang">
    //<!ENTITY math "mathematics">
    //<!ENTITY mil "military">
    //<!ENTITY n "noun (common) (futsuumeishi)">       seen in sense.pos
    //<!ENTITY n-adv "adverbial noun (fukushitekimeishi)">
    //<!ENTITY n-suf "noun, used as a suffix">
    //<!ENTITY n-pref "noun, used as a prefix">
    //<!ENTITY n-t "noun (temporal) (jisoumeishi)">
    //<!ENTITY num "numeric">
    //<!ENTITY oK "word containing out-dated kanji">
    //<!ENTITY obs "obsolete term">
    //<!ENTITY obsc "obscure term">
    //<!ENTITY ok "out-dated or obsolete kana usage">
    //<!ENTITY oik "old or irregular kana form">
    //<!ENTITY on-mim "onomatopoeic or mimetic word">
    //<!ENTITY pn "pronoun">
    //<!ENTITY poet "poetical term">
    //<!ENTITY pol "polite (teineigo) language">
    //<!ENTITY pref "prefix">
    //<!ENTITY proverb "proverb">
    //<!ENTITY prt "particle">
    //<!ENTITY physics "physics terminology">
    //<!ENTITY rare "rare">
    //<!ENTITY sens "sensitive">
    //<!ENTITY sl "slang">
    //<!ENTITY suf "suffix">
    //<!ENTITY uK "word usually written using kanji alone">
    //<!ENTITY uk "word usually written using kana alone">
    //<!ENTITY v1 "Ichidan verb">
    //<!ENTITY v2a-s "Nidan verb with 'u' ending (archaic)">
    //<!ENTITY v4h "Yodan verb with `hu/fu' ending (archaic)">
    //<!ENTITY v4r "Yodan verb with `ru' ending (archaic)">
    //<!ENTITY v5 "Godan verb (not completely classified)">
    //<!ENTITY v5aru "Godan verb - -aru special class">
    //<!ENTITY v5b "Godan verb with `bu' ending">
    //<!ENTITY v5g "Godan verb with `gu' ending">
    //<!ENTITY v5k "Godan verb with `ku' ending">
    //<!ENTITY v5k-s "Godan verb - Iku/Yuku special class">
    //<!ENTITY v5m "Godan verb with `mu' ending">
    //<!ENTITY v5n "Godan verb with `nu' ending">
    //<!ENTITY v5r "Godan verb with `ru' ending">
    //<!ENTITY v5r-i "Godan verb with `ru' ending (irregular verb)">
    //<!ENTITY v5s "Godan verb with `su' ending">
    //<!ENTITY v5t "Godan verb with `tsu' ending">
    //<!ENTITY v5u "Godan verb with `u' ending">
    //<!ENTITY v5u-s "Godan verb with `u' ending (special class)">
    //<!ENTITY v5uru "Godan verb - Uru old class verb (old form of Eru)">
    //<!ENTITY vz "Ichidan verb - zuru verb (alternative form of -jiru verbs)">
    //<!ENTITY vi "intransitive verb">
    //<!ENTITY vk "Kuru verb - special class">
    //<!ENTITY vn "irregular nu verb">
    //<!ENTITY vr "irregular ru verb, plain form ends with -ri">
    //<!ENTITY vs "noun or participle which takes the aux. verb suru">
    //<!ENTITY vs-c "su verb - precursor to the modern suru">
    //<!ENTITY vs-s "suru verb - special class">
    //<!ENTITY vs-i "suru verb - irregular">
    //<!ENTITY kyb "Kyoto-ben">
    //<!ENTITY osb "Osaka-ben">
    //<!ENTITY ksb "Kansai-ben">
    //<!ENTITY ktb "Kantou-ben">
    //<!ENTITY tsb "Tosa-ben">
    //<!ENTITY thb "Touhoku-ben">
    //<!ENTITY tsug "Tsugaru-ben">
    //<!ENTITY kyu "Kyuushuu-ben">
    //<!ENTITY rkb "Ryuukyuu-ben">
    //<!ENTITY nab "Nagano-ben">
    //<!ENTITY hob "Hokkaido-ben">
    //<!ENTITY vt "transitive verb">
    //<!ENTITY vulg "vulgar expression or word">
    //<!ENTITY adj-kari "`kari' adjective (archaic)">
    //<!ENTITY adj-ku "`ku' adjective (archaic)">
    //<!ENTITY adj-shiku "`shiku' adjective (archaic)">
    //<!ENTITY adj-nari "archaic/formal form of na-adjective">
    //<!ENTITY n-pr "proper noun">
    //<!ENTITY v-unspec "verb unspecified">
    //<!ENTITY v4k "Yodan verb with `ku' ending (archaic)">
    //<!ENTITY v4g "Yodan verb with `gu' ending (archaic)">
    //<!ENTITY v4s "Yodan verb with `su' ending (archaic)">
    //<!ENTITY v4t "Yodan verb with `tsu' ending (archaic)">
    //<!ENTITY v4n "Yodan verb with `nu' ending (archaic)">
    //<!ENTITY v4b "Yodan verb with `bu' ending (archaic)">
    //<!ENTITY v4m "Yodan verb with `mu' ending (archaic)">
    //<!ENTITY v2k-k "Nidan verb (upper class) with `ku' ending (archaic)">
    //<!ENTITY v2g-k "Nidan verb (upper class) with `gu' ending (archaic)">
    //<!ENTITY v2t-k "Nidan verb (upper class) with `tsu' ending (archaic)">
    //<!ENTITY v2d-k "Nidan verb (upper class) with `dzu' ending (archaic)">
    //<!ENTITY v2h-k "Nidan verb (upper class) with `hu/fu' ending (archaic)">
    //<!ENTITY v2b-k "Nidan verb (upper class) with `bu' ending (archaic)">
    //<!ENTITY v2m-k "Nidan verb (upper class) with `mu' ending (archaic)">
    //<!ENTITY v2y-k "Nidan verb (upper class) with `yu' ending (archaic)">
    //<!ENTITY v2r-k "Nidan verb (upper class) with `ru' ending (archaic)">
    //<!ENTITY v2k-s "Nidan verb (lower class) with `ku' ending (archaic)">
    //<!ENTITY v2g-s "Nidan verb (lower class) with `gu' ending (archaic)">
    //<!ENTITY v2s-s "Nidan verb (lower class) with `su' ending (archaic)">
    //<!ENTITY v2z-s "Nidan verb (lower class) with `zu' ending (archaic)">
    //<!ENTITY v2t-s "Nidan verb (lower class) with `tsu' ending (archaic)">
    //<!ENTITY v2d-s "Nidan verb (lower class) with `dzu' ending (archaic)">
    //<!ENTITY v2n-s "Nidan verb (lower class) with `nu' ending (archaic)">
    //<!ENTITY v2h-s "Nidan verb (lower class) with `hu/fu' ending (archaic)">
    //<!ENTITY v2b-s "Nidan verb (lower class) with `bu' ending (archaic)">
    //<!ENTITY v2m-s "Nidan verb (lower class) with `mu' ending (archaic)">
    //<!ENTITY v2y-s "Nidan verb (lower class) with `yu' ending (archaic)">
    //<!ENTITY v2r-s "Nidan verb (lower class) with `ru' ending (archaic)">
    //<!ENTITY v2w-s "Nidan verb (lower class) with `u' ending and `we' conjugation (archaic)">
    //<!ENTITY archit "architecture term">
    //<!ENTITY anat "anatomical term">
    //<!ENTITY astron "astronomy, etc. term">
    //<!ENTITY baseb "baseball term">
    //<!ENTITY biol "biology term">
    //<!ENTITY bot "botany term">
    //<!ENTITY bus "business term">
    //<!ENTITY econ "economics term">
    //<!ENTITY engr "engineering term">
    //<!ENTITY finc "finance term">
    //<!ENTITY geol "geology, etc. term">
    //<!ENTITY law "law, etc. term">
    //<!ENTITY med "medicine, etc. term">
    //<!ENTITY music "music term">
    //<!ENTITY Shinto "Shinto term">
    //<!ENTITY sports "sports term">
    //<!ENTITY sumo "sumo term">
    //<!ENTITY zool "zoology term">
    //<!ENTITY joc "jocular, humorous term">
}
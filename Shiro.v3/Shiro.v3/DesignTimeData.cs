using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Shiro.Library;
using Shiro.Model;

namespace Shiro
{
    public static class DesignTimeData
    {
        public static List<ShiroEntryBzzt> GetDictionaryEntries()
        {
            var designTimeEntries = new List<ShiroEntryBzzt>
                {
                     GetDictionaryEntry1(),
                     GetDictionaryEntry2()
                };
            return designTimeEntries;
        }

        public static ShiroEntryBzzt GetDictionaryEntry1()
        {
            var a = new ShiroEntryBzzt
            {
                Id = 1000580,
                Meanings = new List<ShiroEntry.Meaning>
                {
                    new ShiroEntry.Meaning
                    {
                        Index = 1,
                        Gloss = new List<string>
                        {
                            "that (indicating something distant from both speaker and listener (in space,  time or psychologically),  or something understood without naming it directly)"
                        },
                        Antonyms = "",
                        Dial = "",
                        Misc = new List<string> {"word usually written using kana alone"},
                        Xref = "",
                        PartOfSpeech = new List<string> {"noun","suru verb"}
                    },
                    new ShiroEntry.Meaning
                    {
                        Index = 2,
                        Gloss = new List<string>
                        {
                            "that person (used to refer to one's equals or inferiors)"
                        },
                        PartOfSpeech = new List<string> {"noun"}
                    },
                    new ShiroEntry.Meaning
                    {
                        Index = 3,
                        Gloss = new List<string>
                        {
                            "period, menses"
                        },
                        Misc = new List<string> {"colloquialism"}
                    },
                    new ShiroEntry.Meaning
                    {
                        Index = 4,
                        Gloss = new List<string>
                        {
                            "hey (expression of surprise,  suspicion,  etc.), huh?, eh?"
                        },
                        PartOfSpeech = new List<string> {"interjection"}
                    },
                    new ShiroEntry.Meaning
                    {
                        Index = 5,
                        Gloss = new List<string>
                        {
                            "that (something mentioned before which is distant psychologically or in terms of time)"
                        },
                        PartOfSpeech = new List<string> {"noun"}
                    },
                },
                Spellings = new List<ShiroEntry.Spelling>
                {
                    new ShiroEntry.Spelling
                    {
                        Value = "あれ",
                        KanaInformation = new List<string> {"", ""},
                        KanjiInformation = new List<string> {"", ""},
                        Restrictions = new List<string> {"", ""},
                        Priority = ""
                    }
                },
                KanjiChars = new List<string> { "彼" }
            };

            var groupedSpellingsTuple = new List<Tuple<List<string>, List<string>>>
            {
                new Tuple<List<string>, List<string>>(new List<string> {"あれ", "あ"}, new List<string> {"彼"})
            };
            a.SetGroupedSpellings(groupedSpellingsTuple);
            return a;
        }

        public static ShiroEntryBzzt GetDictionaryEntry2()
        {
            var a = new ShiroEntryBzzt
            {
                Id = 1000580,
                Meanings = new List<ShiroEntry.Meaning>
                {
                    new ShiroEntry.Meaning
                    {
                        Index = 2,
                        Gloss = new List<string>
                        {
                            "that person (used to refer to one's equals or inferiors)"
                        },
                        PartOfSpeech = new List<string> {"noun"}
                    }
                },
                Spellings = new List<ShiroEntry.Spelling>
                {
                    new ShiroEntry.Spelling
                    {
                        Value = "あれ",
                        KanaInformation = new List<string> {"", ""},
                        KanjiInformation = new List<string> {"", ""},
                        Restrictions = new List<string> {"", ""},
                        Priority = ""
                    }
                }
            };

            var groupedSpellingsTuple = new List<Tuple<List<string>, List<string>>>
            {
                new Tuple<List<string>, List<string>>(new List<string> {"あれ", "あ"}, new List<string> {"彼"})
            };
            a.SetGroupedSpellings(groupedSpellingsTuple);
            return a;
        }


        public static KanjiInfo GetKanjiDic2Entry1()
        {
            return new KanjiInfo
            {
                Frequency = 258,
                JlptLvl = 2,
                Kanji = "果",
                StrokeCount = 8,
                SchoolGrade = 4,
                KunReadings = string.Join(", ", new List<string> { "は.たす", "はた.す", "-は.たす", "は.てる", "-は.てる", "は.て" }),
                Meanings =
                    string.Join(", ",
                        new List<string>
                        {
                            "fruit",
                            "reward",
                            "carry out",
                            "achieve",
                            "complete",
                            "end",
                            "finish",
                            "succeed"
                        }),
                NanoriReadings = string.Join(", ", new List<string> { "み" }),
                OnReadings = string.Join(", ", new List<string> { "カ" }),
                Unicode = "679c",
                KanjiGraph = GetKanjiGraph1()
            };
        }

        public static KanjiGraph GetKanjiGraph1()
        {
            return new KanjiGraph
            {
                Element = "果",
                Id = 2312,
                Unicode = "0679c",
                Graphs = new List<KanjiGraph.KGraph>
                {
                    new KanjiGraph.KGraph
                    {
                        Element = "田",
                        Strokes = new List<KanjiGraph.Stroke>
                        {
                            new KanjiGraph.Stroke
                            {
                                Order = 1,
                                Data =
                                    "M28.27,16.76c1.11,1.11,1.66,2.37,1.78,3.35c0.28,2.35,2.58,16.8,3.9,24.85c0.23,1.42,0.43,2.63,0.59,3.55"
                            },
                            new KanjiGraph.Stroke
                            {
                                Order = 2,
                                Data =
                                    "M30.44,18.39c8.07-0.87,34.21-3.65,42.93-4.44c4.09-0.37,6.75,1.64,5.63,6.08c-1.27,5.05-3.81,15.36-5.37,21.47c-0.38,1.49-0.7,2.73-0.93,3.59"
                            },
                            new KanjiGraph.Stroke {Order = 3, Data = "M33.08,32.26c8.67-0.89,35.17-3.64,42.47-3.9"},
                            new KanjiGraph.Stroke {Order = 4, Data = "M35.44,46.22c6.06-0.53,29.31-2.7,36.85-3.15"},
                            new KanjiGraph.Stroke
                            {
                                Order = 6,
                                Data =
                                    "M52.75,18.75c1.12,1.12,1.5,2.72,1.5,3.75c0,2.49-0.19,49.36-0.24,67.12C54,92.37,54,94.41,54,95.5"
                            }
                        }
                    },
                    new KanjiGraph.KGraph
                    {
                        Element = "木",
                        Strokes = new List<KanjiGraph.Stroke>
                        {
                            new KanjiGraph.Stroke
                            {
                                Order = 5,
                                Data =
                                    "M17.5,59.75c3.11,1.04,6.31,0.96,9.49,0.59c12.89-1.47,39.78-3.8,55.39-4.64c2.89-0.16,5.79-0.27,8.63,0.3"
                            },
                            new KanjiGraph.Stroke
                            {
                                Order = 7,
                                Data = "M50.5,59c0,1-0.6,2.07-1.08,2.75C42.13,72.13,26.63,85.39,13.88,91"
                            },
                            new KanjiGraph.Stroke
                            {
                                Order = 8,
                                Data = "M55.88,59.62c4.88,4,22.25,17.62,29.79,23.25c2.33,1.74,4.48,3.16,7.21,4.25"
                            },
                        }
                    },
                },
                Parts = new List<string> { "果", "木", "日", "丨" },
            };
        }

        //<g id="0679c" element="果">
        //    <g id="0679c-g1" element="田" part="1">
        //    <g id="0679c-g2" element="日">
        //        <path id="0679c-s1" type="㇑" d="M28.27,16.76c1.11,1.11,1.66,2.37,1.78,3.35c0.28,2.35,2.58,16.8,3.9,24.85c0.23,1.42,0.43,2.63,0.59,3.55"/>
        //        <path id="0679c-s2" type="㇕a" d="M30.44,18.39c8.07-0.87,34.21-3.65,42.93-4.44c4.09-0.37,6.75,1.64,5.63,6.08c-1.27,5.05-3.81,15.36-5.37,21.47c-0.38,1.49-0.7,2.73-0.93,3.59"/>
        //        <path id="0679c-s3" type="㇐a" d=/>
        //        <path id="0679c-s4" type="㇐a" d=/>
        //    </g>
        //</g>
        //<g id="0679c-g3" element="木" radical="tradit">
        //    <path id="0679c-s5" type="㇐" d="M17.5,59.75c3.11,1.04,6.31,0.96,9.49,0.59c12.89-1.47,39.78-3.8,55.39-4.64c2.89-0.16,5.79-0.27,8.63,0.3"/>
        //    <g id="0679c-g4" element="田" part="2">
        //        <g id="0679c-g5" element="丨" radical="nelson">
        //            <path id="0679c-s6" type="㇑" d="M52.75,18.75c1.12,1.12,1.5,2.72,1.5,3.75c0,2.49-0.19,49.36-0.24,67.12C54,92.37,54,94.41,54,95.5"/>
        //        </g>
        //    </g>
        //    <path id="0679c-s7" type="㇒" d="M50.5,59c0,1-0.6,2.07-1.08,2.75C42.13,72.13,26.63,85.39,13.88,91"/>
        //    <path id="0679c-s8" type="㇏" d="M55.88,59.62c4.88,4,22.25,17.62,29.79,23.25c2.33,1.74,4.48,3.16,7.21,4.25"/>
        //</g>

        public static TatoebaSentence GetExampleSentence1()
        {
            var sentence = new TatoebaSentence();
            sentence.EngSentence = "I don't want to lose my ideas, even though some of them are a bit extreme.";
            sentence.JapSentence = "行き過ぎたものであっても、僕は自分の考えをなくしたくない";
            sentence.JapSentenceTokens = new List<SentenceToken>();
            sentence.JapSentenceTokens.Add(new SentenceToken { Pronounciation = "い", Token = "行" });
            sentence.JapSentenceTokens.Add(new SentenceToken { Pronounciation = "", Token = "き" });
            sentence.JapSentenceTokens.Add(new SentenceToken { Pronounciation = "か", Token = "過ぎ" });
            sentence.JapSentenceTokens.Add(new SentenceToken { Pronounciation = "", Token = "たものであっても" });
            sentence.JapSentenceTokens.Add(new SentenceToken { Pronounciation = "", Token = "僕は自分の考えをなくしたくない" });

            return sentence;
        }

        public static List<BookmarkCollection> GetBookmarkCollections()
        {
            var bookmarkCollections = new List<BookmarkCollection>
            {
                new BookmarkCollection {Name = "MyKanjis",Color = Colors.Coral},
                new BookmarkCollection {Name = "MyWordList1",Color = Colors.SeaGreen},
                new BookmarkCollection {Name = "MyWordList2",Color = Colors.BlueViolet},
            };
            return bookmarkCollections;
        }

        public static List<BookmarkCollectionCheck> GetBookmarkCollectionExts()
        {
            var collections = GetBookmarkCollections();

            var checks = collections.Select(t => new BookmarkCollectionCheck(t)).ToList();
            checks[2].IsBookmarked = true;
            return checks;
        }

        public static BindableResourceDict BindableResourceDictionary1
        {
            get
            {
                return new BindableResourceDict
                {
                    Items = new List<DictionaryItem>
                    {
                        new DictionaryItem("mavi", new SolidColorBrush(Colors.Blue)),
                        new DictionaryItem("yesil", new SolidColorBrush(Colors.Green)),
                        new DictionaryItem("Bizim Yazi Sekli", new FontFamily("Meiryo UI")),
                        new DictionaryItem("Bizim Yazi Sekli", new FontFamily("Comic Sans MS")),
                        new DictionaryItem("Color red", Colors.OrangeRed),
                        new DictionaryItem("md0",new BindableResourceDict
                            {
                                Items =  new List<DictionaryItem>()
                                {
                                    new DictionaryItem("kirmizi", new SolidColorBrush(Colors.Red)),
                                    new DictionaryItem("mor", new SolidColorBrush(Colors.Purple))
                                }
                            }
                        )
                    }
                };
            }
        }
    }
}
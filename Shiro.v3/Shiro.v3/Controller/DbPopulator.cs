using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using FrInterfaces;
using JapaneseResources.Models.RawModels;
using Kelebron.Utils.Japanese;
using Shiro.Library;
using Shiro.Library.FuriganaView;
using Shiro.Model;

namespace Shiro.Controller
{
    public enum KnownFileTypes
    {
        KanjiVg,
        HeisigKanji,
        WordList,
        JMDict,
        Kanjidic,
        Kanjidic2,
    }

    // BIG TODO: BURADAKI TUM DOSYA OKUMA ISLEMLERINDE JapaneseResources projesi kullanilacak
    public class DbPopulator
    {
        //public ShiroFileRepository ShiroFileRepository { get; set; }

        private static readonly Action<int, IProgress<int>> ReportProgress = (t, progress) =>
        {
            if (progress != null) progress.Report(t);
        };

        public DbPopulator()
        {
            ShiroRepository = new ShiroRepository(IoCContainer.Resolve<IRepository>());
        }

        #region KanjiGraph(Stroke Paths) Data From KanjiVg file

        //public int PopulateKanjiGraphDataOnDb(string kanjiVgXmlFilePath)
        //{
        //    //truncate first
        //    ShiroRepository.Delete<KanjiGraph>();
        //    //read xml data
        //    var kanjiVgEntries = ReadKanjiVgFile(kanjiVgXmlFilePath);
        //    //save read data
        //    var kanjiGraphs = ParseKanjiVgData(kanjiVgEntries).ToList();
        //    ShiroRepository.Save(kanjiGraphs);
        //    //for debug logging:
        //    var count = ShiroRepository.Count<KanjiGraph>();

        //    return count;
        //}

        private KanjiVg ReadKanjiVgFile(string xmlFilePath)
        {
            var xmlReaderSettings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse };
            var ser = new XmlSerializer(typeof(KanjiVg));
            KanjiVg kanjiVgRaw;
            using (XmlReader reader = XmlReader.Create(xmlFilePath, xmlReaderSettings))
            {
                kanjiVgRaw = (KanjiVg)ser.Deserialize(reader);
            }
            return kanjiVgRaw;
        }

        private IEnumerable<KanjiGraph> ParseKanjiVgData(KanjiVg kanjiVg)
        {
            foreach (KanjivgKanji kanjivgKanji in kanjiVg.Kanjis)
            {
                var kanjiGraph = new KanjiGraph();
                kanjiGraph.Unicode = kanjivgKanji.Id.TrimStart("kanji_".ToCharArray());
                kanjiGraph.Element = kanjivgKanji.RootG.Element;

                Func<string, string, int> getStrokeOrder = (xmlPathId, kanjiUnicode) =>
                {
                    string startString = kanjiUnicode + "-s";
                    string replace = xmlPathId.Replace(startString, "");
                    return Convert.ToInt32(replace);
                };
                Func<KanjivgKanjiGPath, KanjiGraph.Stroke> getStroke =
                    p => new KanjiGraph.Stroke
                    {
                        Order = getStrokeOrder(p.Id, kanjiGraph.Unicode),
                        Data = p.D
                    };


                var strokePaths = new List<KanjivgKanjiGPath>();
                SelectStrokePathsRecursive(kanjivgKanji.RootG, ref strokePaths);
                var parts = new List<string>();
                SelectKanjiPartsRecursive(kanjivgKanji.RootG, ref parts);

                kanjiGraph.Parts = parts;

                kanjiGraph.Graphs = new List<KanjiGraph.KGraph>();
                //Wrapping Stroke Paths at root level as KanjiGraph.KGraph:
                if (kanjivgKanji.RootG.Paths.Count > 0)
                {
                    var graphForRootStrokes = new KanjiGraph.KGraph
                    {
                        Element = string.Empty,
                        Unicode = string.Empty,
                        Strokes = kanjivgKanji.RootG.Paths.Select(getStroke).ToList()
                    };
                    kanjiGraph.Graphs.Add(graphForRootStrokes);
                }
                foreach (KanjivgKanjiGraph subG in kanjivgKanji.RootG.Gs)
                {
                    var gPaths = new List<KanjivgKanjiGPath>();
                    SelectStrokePathsRecursive(subG, ref gPaths);
                    var graph = new KanjiGraph.KGraph
                    {
                        Element = subG.Element,
                        Unicode = string.Empty, //not provided for G elements in xml  
                        Strokes = gPaths.Select(getStroke).ToList()
                    };
                    kanjiGraph.Graphs.Add(graph);
                }
                //kanjiGraph.Strokes = strokePaths.Select(getStroke).ToList();
                //kanjiGraph.StrokeCount = kanjiGraph.Strokes.Count;
                yield return kanjiGraph;
            }
        }

        public void SelectStrokePathsRecursive(KanjivgKanjiGraph parent, ref List<KanjivgKanjiGPath> strokePaths)
        {
            if (parent.Paths != null)
                strokePaths.AddRange(parent.Paths);
            if (parent.Gs != null)
                foreach (KanjivgKanjiGraph g in parent.Gs)
                {
                    SelectStrokePathsRecursive(g, ref strokePaths);
                }
        }

        public void SelectKanjiPartsRecursive(KanjivgKanjiGraph parent, ref List<string> parts)
        {
            if (parent.Gs != null)
            {
                foreach (KanjivgKanjiGraph g in parent.Gs)
                {
                    if (!string.IsNullOrEmpty(g.Element))
                        parts.Add(g.Element);
                    SelectKanjiPartsRecursive(g, ref parts);
                }
            }
        }

        #endregion

        #region KanjiInfo Data FROM KanjiVg file and KanjiDic2 File

        ///// <summary>
        ///// this one lacks of Graph Data
        ///// </summary>
        ///// <param name="kanjidic2XmlFilePath"></param>
        ///// <returns></returns>
        //public int PopulateKanjiInfoDataOnDb(string kanjidic2XmlFilePath)
        //{
        //    //truncate first
        //    ShiroRepository.Delete<KanjiInfo>();
        //    //read xml data
        //    var kanjidic2 = ReadKanjiDic2Xml(kanjidic2XmlFilePath);
        //    //save read data
        //    var kanjiInfos = kanjidic2.Characters.Select(t => ParseKanjiDic2Entry(t)).ToList();
        //    ShiroRepository.Save(kanjiInfos);
        //    //for debug logging:
        //    var count = ShiroRepository.Count<KanjiInfo>();
        //    return count;
        //}

        private readonly Func<KanjiDic2Char, int> _getFreq =
            s => Convert.ToInt32(s.Misc.Select(m => m.Freq).FirstOrDefault());

        private readonly Func<KanjiDic2Char, string, List<string>> _selectMeaningsByLang =
            (character, s) => character.ReadingMeaning.SelectMany(rm => rm.RmGroup)
                .SelectMany(rmg => rmg.Meaning).Where(o => o.MLang == s).Select(m => m.Value).ToList();

        private readonly Func<KanjiDic2Char, string, List<string>> _selectReadings =
            (character, s) => character.ReadingMeaning.SelectMany(rm => rm.RmGroup)
                .SelectMany(rmg => rmg.Reading).Where(o => o.RType == s).Select(r => r.Value).ToList();

        public async Task<int> PopulateKanjiInfoDataOnDb(string kanjidic2XmlFilePath, string kanjiVgXmlFilePath,
            IProgress<int> progress = null)
        {
            return await Task.Run(() =>
            {
                int taskProgress = 0;

                //truncate first
                ShiroRepository.Drop<KanjiInfo>();
                //read kanjivg xml data
                KanjiVg allKanjiVgData = ReadKanjiVgFile(kanjiVgXmlFilePath);
                ReportProgress(taskProgress += 10, progress);
                //save read data
                List<KanjiGraph> allGraphData = ParseKanjiVgData(allKanjiVgData).ToList();
                ReportProgress(taskProgress += 10, progress);


                //read xml data
                Kanjidic2 kanjidic2 = ReadKanjiDic2Xml(kanjidic2XmlFilePath);
                ReportProgress(taskProgress += 10, progress);

                //save read data
                List<KanjiInfo> kanjiInfos =
                    kanjidic2.Characters.Select(t => ParseKanjiDic2Entry(t, allGraphData)).ToList();
                ReportProgress(taskProgress += 10, progress);

                //ShiroRepository.Save(kanjiInfos); 
                int loopWeight = kanjiInfos.Count / 60;
                int i = 0;
                for (; i < kanjiInfos.Count; i++)
                {
                    List<KanjiInfo> take = kanjiInfos.Skip(i).Take(loopWeight).ToList();
                    ShiroRepository.Save(take);
                    i += loopWeight;
                    ReportProgress(taskProgress += 1, progress);
                }

                //for debug logging:
                int count = ShiroRepository.Count<KanjiInfo>();

                return count;
            });
        }

        private Kanjidic2 ReadKanjiDic2Xml(string kanjidic2XmlFilePath)
        {
            var xmlReaderSettings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse };

            var ser = new XmlSerializer(typeof(Kanjidic2));
            using (XmlReader reader = XmlReader.Create(kanjidic2XmlFilePath, xmlReaderSettings))
            {
                var kanjidic2 = (Kanjidic2)ser.Deserialize(reader);
                kanjidic2.Characters.Select(t => t.Misc).ToList().ForEach(_ => Console.WriteLine());
                //foreach (var misc in kanjidic2.Characters.SelectMany(c => c.Misc))
                //{
                //    //xml deserialize edilirken bu iki alan bos olmasi durumunda 0 olarak parse ediliyor, bu dengesiz bilgi oldugu oldugu icin degistiriyorum
                //    misc.Freq = misc.Freq == 0 ? 5000 : misc.Freq;
                //    misc.Grade = misc.Grade == 0 ? 10 : misc.Grade;
                //}

                kanjidic2.Characters.Sort((x, y) => _getFreq(x).CompareTo(_getFreq(y)));

                return kanjidic2;
            }
        }

        private KanjiInfo ParseKanjiDic2Entry(KanjiDic2Char character, IEnumerable<KanjiGraph> allGraphs = null)
        {
            var kanjiInfo = new KanjiInfo();
            if (character != null)
            {
                kanjiInfo.Unicode =
                    character.Codepoint.Where(cp => cp.CpType == "ucp").Select(sc => sc.Value).FirstOrDefault();
                kanjiInfo.Frequency = _getFreq(character);
                kanjiInfo.JlptLvl = character.Misc.Select(m => m.Jlpt).FirstOrDefault();
                kanjiInfo.SchoolGrade = character.Misc.Select(m => m.Grade).FirstOrDefault();
                kanjiInfo.StrokeCount =
                    character.Misc.SelectMany(m => m.StrokeCount).Select(sc => sc.Value).FirstOrDefault();
                kanjiInfo.Kanji = character.Literal;

                kanjiInfo.KunReadings = string.Join(", ", _selectReadings(character, "ja_kun"));
                kanjiInfo.OnReadings = string.Join(", ", _selectReadings(character, "ja_on"));
                kanjiInfo.NanoriReadings = string.Join(", ",
                    character.ReadingMeaning.SelectMany(rm => rm.NanoriReadings).Select(rmg => rmg.Value));
                kanjiInfo.Meanings = string.Join(", ", _selectMeaningsByLang(character, null));
                // null string for eng meanings

                if (allGraphs != null)
                    kanjiInfo.KanjiGraph = allGraphs.FirstOrDefault(t => t.Element == kanjiInfo.Kanji);
            }
            return kanjiInfo;
        }

        #endregion

        #region Dictionary entries From JMDict file

        /// <summary>
        ///     Inserts ShiroEntry data to db
        /// </summary>
        /// <param name="jmdictXmlPath"></param>
        /// <param name="progress"></param>
        /// <returns>inserted data count</returns>
        public async Task<int> PopulateDictionaryDataOnDb(string jmdictXmlPath, IProgress<int> progress = null)
        {
            return await Task.Run(() =>
            {
                int taskProgress = 0;
                ReportProgress(taskProgress += 1, progress);
                //truncate first
                ShiroRepository.Drop<ShiroEntry>();
                ReportProgress(taskProgress += 10, progress);
                //read xml data
                JMDict entries = ReadJMDictXml(jmdictXmlPath);
                ReportProgress(taskProgress += 20, progress);
                //save read data
                List<ShiroEntry> dictionaryEntries = entries.Entries.Select(ParseJMDictEntry).ToList();
                //ShiroRepository.Save(dictionaryEntries);
                int loopWeight = dictionaryEntries.Count / 60;
                int i = 0;
                for (; i < dictionaryEntries.Count; i++)
                {
                    List<ShiroEntry> take = dictionaryEntries.Skip(i).Take(loopWeight).ToList();
                    ShiroRepository.Save(take);
                    i += loopWeight;
                    ReportProgress(taskProgress += 1, progress);
                }

                //for debug logging:
                int count = ShiroRepository.Count<ShiroEntry>();

                return count;
            });
        }

        private JMDict ReadJMDictXml(string jmDictXmlPath)
        {
            var xmlReaderSettings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse };
            var ser = new XmlSerializer(typeof(JMDict));
            using (XmlReader reader = XmlReader.Create(jmDictXmlPath, xmlReaderSettings))
            {
                var jMdictSample = (JMDict)ser.Deserialize(reader);
                return jMdictSample;
            }
        }

        /// <summary>
        ///     JMDictEntry'den ShiroEntry'leri hazilar
        ///     sadece ingilizce anlamlari alir
        /// </summary>
        private ShiroEntry ParseJMDictEntry(JMDictEntry dictEntry)
        {
            var a = new ShiroEntry
            {
                Id = Convert.ToInt32(dictEntry.EntSeq),
                Spellings = new List<ShiroEntry.Spelling>(),
            };

            List<ShiroEntry.Spelling> writingsWithKanji = dictEntry.KanjiElement
                .Select(
                    k =>
                        new ShiroEntry.Spelling
                        {
                            Value = k.Keb,
                            KanaInformation = k.KeInf,
                            Priority = string.Join(",", k.KePri)
                        }).ToList();
            List<ShiroEntry.Spelling> readings = dictEntry.ReadingElement.Select(
                k => new ShiroEntry.Spelling
                {
                    Value = k.Reb,
                    KanaInformation = k.ReInf,
                    Priority = string.Join(",", k.RePri),
                    Restrictions = k.ReRestr
                }).ToList();
            a.Spellings.AddRange(writingsWithKanji);
            a.Spellings.AddRange(readings);

            //kana [ k1,k2,k3 ] gosterimi groupedReadings' olusturuluyor 
            var comparer = new IgnoreOrderComparer(StringComparer.OrdinalIgnoreCase);
            //kanjiReadingsGroups -->Dictionary<restrictions, readings> :   -->  restrictions'da kanji'ler var veya hic bir sey yok
            IEnumerable<IGrouping<IList<string>, ShiroEntry.Spelling>> kanjiReadingsGroups =
                readings.GroupBy(r => r.Restrictions, comparer);
            IEnumerable<Tuple<List<string>, List<string>>> groupedReadings = kanjiReadingsGroups
                .Select(i => new Tuple<List<string>, List<string>>(
                    i.Select(o => o.Value).ToList(),
                    (i.Key.Any() ? i.Key : writingsWithKanji.Select(r => r.Value)).ToList())
                );


            a.GroupedSpellings = groupedReadings.Select(o => new List<List<string>> { o.Item1, o.Item2 }).ToList();
            a.Meanings = new List<ShiroEntry.Meaning>();
            a.Meanings.AddRange(dictEntry.Sense.Select(x => new ShiroEntry.Meaning
            {
                Index = dictEntry.Sense.IndexOf(x) + 1,
                Gloss = x.Gloss.GroupBy(t => t.Lang)
                    .Select(p => new KeyValuePair<string, string>(p.Key,
                        string.Join(", ", p.Select(y => string.Join(", ", y.Text.Select(z => z.Replace(",", ", ")))))))
                    .Where(t => t.Key == "eng")
                    .Select(y => y.Value)
                    .ToList(),
                PartOfSpeech = x.Pos,
                Antonyms = string.Join(", ", x.Ant),
                Dial = string.Join(", ", x.Dial), // SInf, Lsource, Example,Field|||
                Misc =
                    x.Misc.Concat(x.Example)
                        .Concat(x.Field)
                        .Concat(x.SInf)
                        .Concat(x.Lsource.Select(t => string.Format("{0} {1}", t.Lang, t.Value)))
                        .ToList(),
                Xref = string.Join(", ", x.Xref)
            }
                ));

            //Extract kanjis:
            IEnumerable<char> allChars = a.Spellings.SelectMany(s => s.Value.ToCharArray()).Distinct();
            List<string> kanjiChars =
                allChars.Where(JapaneseKanaClassifier.IsKanji)
                    .Select(t => t.ToString(CultureInfo.InvariantCulture))
                    .ToList();
            a.KanjiChars = kanjiChars;

            return a;
        }

        #endregion

        #region TatoebaSenteces From Files Sentences.csv and Sentece_Links.csv

        public async Task<int> PopulateExampleSentencesOnDb(string sentencesFilePath, string sentenceLinksFilePath, IProgress<int> progress = null)
        {
            return await Task.Run(() =>
            {
                int taskProgress = 0;
                ReportProgress(taskProgress += 1, progress);
                //truncate first
                ShiroRepository.Drop<TatoebaSentence>();
                ReportProgress(taskProgress += 5, progress);

                List<TatoebaSentence> sentences = ReadAllJapToEngTatoebaSentences(sentencesFilePath, sentenceLinksFilePath);
                ReportProgress(taskProgress += 34, progress);

                //ShiroRepository.Save(sentences);

                //to report saving progress instead of saving all at once i divide the data in 60 parts then save one block then report at a time
                int loopWeight = sentences.Count / 60;

                for (int i = 0; i < sentences.Count; i++)
                {
                    List<TatoebaSentence> take = sentences.Skip(i).Take(loopWeight).ToList();
                    ShiroRepository.Save(take);
                    i += loopWeight;
                    ReportProgress(taskProgress += 1, progress);
                }
                //at this point taskProgress should be 100 todo:need test
                int count = ShiroRepository.Count<TatoebaSentence>();
                return count;
            });
        }

        object lockObject = new object();
        /// <summary>
        /// Updates MecabData of all sentences 
        /// analysing each sentence takes pretty some time, so we do this for all sentences at once
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateSentenceTokens(IProgress<int> progress = null)
        {
            return await Task.Run(() =>
            {
                int taskProgress = 0;
                //Initiate, progression 0
                ReportProgress(taskProgress, progress);

                //take all not tokenized sentences
                var tatoebaSentences = ShiroRepository.GetMany<TatoebaSentence>(predicate: t => t.JapSentenceTokens == null || t.JapSentenceTokens.Count == 0).ToList();

                for (int i = 0; i < tatoebaSentences.Count; i++)
                {
                    var tatoebaSentence = tatoebaSentences[i];
                    var meCabDatas = JapaneseTextTokenizer.TokenizeAsMeCabData(tatoebaSentence.JapSentence);
                    tatoebaSentence.JapSentenceTokens = new List<SentenceToken>(meCabDatas.Count);
                    foreach (var meCabData in meCabDatas)
                    {
                        tatoebaSentence.JapSentenceTokens.Add(new SentenceToken
                        {
                            Pronounciation = meCabData.Pronounciation,
                            Token = meCabData.Token
                        });
                    }
                    ShiroRepository.Save(tatoebaSentence);
                    ReportProgress(i + 1, progress);//sending i+1 baceuse i want to report count not the index
                }

                //var tokenizationTasks = tatoebaSentences.Select(tatoebaSentence =>
                //{
                //    var task = Task.Run(() =>
                //    {
                //        var meCabDatas = JapaneseTextTokenizer.TokenizeAsMeCabData(tatoebaSentence.JapSentence);
                //        tatoebaSentence.JapSentenceTokens = new List<SentenceToken>(meCabDatas.Count);
                //        foreach (var meCabData in meCabDatas)
                //        {
                //            tatoebaSentence.JapSentenceTokens.Add(new SentenceToken
                //            {
                //                Pronounciation = meCabData.Pronounciation,
                //                Token = meCabData.Token
                //            });
                //        }
                //        ShiroRepository.Save(tatoebaSentence);
                //    });
                //    return task;
                //}).ToDictionary(task => task.Id, task => task);

                //while (tokenizationTasks.Any())
                //{
                //    var task = await Task.WhenAny(tokenizationTasks.Values);
                //    ReportProgress(++taskProgress, progress);
                //    tokenizationTasks.Remove(task.Id);
                //}

                int count = ShiroRepository.Count<TatoebaSentence>();
                return count;
            });
        }


        /// <summary>
        ///     returns TatoebaSentence obj for each japanese sentence with it's first english translation
        ///     explanation:
        ///     gets all sentences and links between sentences
        ///     gets all japanese sentences
        ///     gets all english sentences
        ///     gets japanese to other lang sentence links ( searches links where first link id is in "all japanese sentences")
        ///     gets japanese to english sentence links  ( searches "japanese to other links" where second link id is in "all
        ///     english sentences")
        ///     groups by japanese sentence id (first link id), b ecause one japanese sentence has more than one translation
        ///     makes TatoebaSentence obj for each japanese sentence with it's first english translation
        /// </summary>
        /// <param name="sentencesFilePath"></param>
        /// <param name="sentenceLinksFilePath"></param>
        /// <returns></returns>
        private List<TatoebaSentence> ReadAllJapToEngTatoebaSentences(string sentencesFilePath,
            string sentenceLinksFilePath)
        {
            var sentences = new HashSet<TatoebaSentenceRaw>(ReadSentences(0, sentencesFilePath));
            var sentenceLinks = new HashSet<TatoebaSentenceLink>(ReadSentenceLinks(0, sentenceLinksFilePath));

            Dictionary<int, TatoebaSentenceRaw> engSentenceXs =
                sentences.Where(s => s.Lang == "eng").ToDictionary(a => a.Id);
            Dictionary<int, TatoebaSentenceRaw> japSentenceXs =
                sentences.Where(s => s.Lang == "jpn").ToDictionary(a => a.Id);

            List<TatoebaSentenceLink> japToAllLinks =
                sentenceLinks.Where(l => japSentenceXs.ContainsKey(l.Id1)).ToList();
            List<TatoebaSentenceLink> japToEngLinks =
                japToAllLinks.Where(l => engSentenceXs.ContainsKey(l.Id2)).ToList();

            // japSentences.Where(y=>leftSideJapRightSideEngLinks.Any(t=>t.Id1 == y.Id))
            Dictionary<int, List<TatoebaSentenceLink>> japToEngGroupedLinks =
                japToEngLinks.GroupBy(u => u.Id1).ToDictionary(xs => xs.Key, xs => xs.ToList());

            var list = new List<TatoebaSentence>();
            foreach (var japSent in japToEngGroupedLinks)
            {
                TatoebaSentenceRaw jap = japSentenceXs[japSent.Key];
                TatoebaSentenceRaw eng = engSentenceXs[japSent.Value[0].Id2];
                var sentenceJapBased = new TatoebaSentence
                {
                    JapId = jap.Id,
                    JapSentence = jap.Sentence,
                    EngId = eng.Id,
                    EngSentence = eng.Sentence
                };
                list.Add(sentenceJapBased);
            }
            return list;
        }

        /// <summary>
        ///     Reads sentences.csv file from Tatoeba.com
        /// </summary>
        /// <param name="takeCount"></param>
        /// <param name="sentencesFilePath"></param>
        /// <returns></returns>
        private IEnumerable<TatoebaSentenceRaw> ReadSentences(int takeCount, string sentencesFilePath)
        {
            var list = new List<TatoebaSentenceRaw>();
            using (StreamReader reader = File.OpenText(sentencesFilePath))
            {
                string line;
                int counter = 0;
                while (((line = reader.ReadLine()) != null) && (counter < takeCount || takeCount == 0))
                {
                    counter++;
                    string[] split = line.Split('\t');
                    var a = new TatoebaSentenceRaw
                    {
                        Id = Convert.ToInt32(split[0]),
                        Lang = split[1],
                        Sentence = split[2]
                    };
                    list.Add(a);

                    //Console.WriteLine(@"First rec: {0}:{1}:{2}", a.Id, a.Lang, a.Sentence);
                }
            }
            return list;
        }

        /// <summary>
        ///     Reads sentence_links.csv file from Tatoeba.com
        /// </summary>
        /// <param name="takeCount"></param>
        /// <param name="sentenceLinksFilePath"></param>
        /// <returns></returns>
        private IEnumerable<TatoebaSentenceLink> ReadSentenceLinks(int takeCount, string sentenceLinksFilePath)
        {
            var list = new List<TatoebaSentenceLink>();
            using (StreamReader reader = File.OpenText(sentenceLinksFilePath))
            {
                string line;
                int counter = 0;
                while (((line = reader.ReadLine()) != null) && (counter < takeCount || takeCount == 0))
                {
                    counter++;
                    string[] split = line.Split('\t');
                    var a = new TatoebaSentenceLink { Id1 = Convert.ToInt32(split[0]), Id2 = Convert.ToInt32(split[1]) };
                    list.Add(a);
                    //Console.WriteLine(@"Link: {0}:{1}", a.Id1, a.Id2);
                }
            }
            return list;
        }

        #endregion

        public ShiroRepository ShiroRepository { get; set; }

        //public List<Word> ReadWordCollectionFromFile(string filePath)
        //{
        //    ShiroFileRepository = new ShiroFileRepository();
        //    var kelimeler = ShiroFileRepository.LoadFromFile(filePath);
        //    return kelimeler;
        //}
        //public List<Word> ReadWordCollectionFromDb(string collectionName)
        //{
        //    var kelimeler = WordController.GetCollectionData(collectionName);
        //    return kelimeler;
        //}

        //public void PopulateWordCollectionOnDb(string filePath, string collectionName)
        //{
        //    var kelimeler = ReadWordCollectionFromFile(filePath);
        //    SetWordCollectionName(kelimeler, collectionName);
        //    ShiroRepository.Save(kelimeler.ToList());
        //    var count = ShiroRepository.Count<Word>();
        //    Console.WriteLine(count);
        //}

        //public void SetWordCollectionName(List<Word> words, string collectionName)
        //{
        //    words.ForEach(a => a.CollectionName = collectionName);
        //}

        //public void PopulateCollectionNamesOnDb()
        //{
        //    var kelimeList = new Collection<> { Name = "Çince" };
        //    ShiroRepository.Save(kelimeList);
        //}
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using Reactive.Bindings;
using Shiro.Converter;
using Shiro.Library;
using Shiro.Library.Extensions;
using Shiro.Model;

namespace Shiro.ViewModel
{
    public class WritingStudyViewModel : MainViewModel
    {
        public WritingStudyViewModel()
        {
            if (!IsInDesignMode)
            {
                CalismaHazirlaCommand = new RelayCommand(() =>
                {
                    CalismaHazirla2();
                    StrokeOrderGraphList.SetPrintTheme1();
                    var yy = RandomWords.SelectMany(t => t.Kanjis)
                        .Select(k => k.KanjiGraph.PathDataList)
                        .Select(z => new StrokeOrderGraphList("", z).GetStrokeOrderGraph()).ToList();
                    StrokeOrderUiElements.Clear();
                    StrokeOrderUiElements.AddRange(yy);
                });

                PrintCommand = new RelayCommand(() =>
                {
                    var isPrinted = PrintingUtils.Print(RtxtCalismaYazisi.Document);
                    if (isPrinted)
                        IncreaseBookmarksStudyProgresses(RandomWords.ToList(), MaxAllowedRepeatCountForAWord.Value);
                });

                PrintStrokeOrdersCommand = new RelayCommand(() =>
                {
                    StrokeOrderGraphList.SetPrintTheme1();
                    var yy = RandomWords.SelectMany(t => t.Kanjis)
                        .Select(k => k.KanjiGraph.PathDataList)
                        .Select(z => new StrokeOrderGraphList("", z).GetStrokeOrderGraph()).ToList();
                    PrintingUtils.VisualToXps(yy.ToList());
                });

                RandomWords = new DistinctObservableCollection<ShiroEntryBzzt>();
                StrokeOrderUiElements = new RangeObservableCollection<StrokeOrderGraphList>();
                WordCollections = new RangeObservableCollection<BookmarkCollection>();
                WordCount = new ReactiveProperty<int>(20);
                TotalRepeatCount = new ReactiveProperty<int>(100);
                MaxAllowedRepeatCountForAWord = new ReactiveProperty<int>(5);
                DocumentXaml = new ReactiveProperty<string>("");

                SelectedCollection = new ReactiveProperty<BookmarkCollection>();
                SelectedCollectionContent = SelectedCollection
                    .Select(t => t != null ? BookmarkController.GetCollectionContent(t.Id) : new List<Bookmark>())
                    .Select(b => b.Select(o => new BookmarkExt(o)).ToList())
                    .ToReactiveProperty();
                Words = SelectedCollection
                    .Select(t => t != null ? BookmarkController.GetCollectionContent(t.Id) : new List<Bookmark>())
                    .Select(z => z.Select(a => new BookmarkExt(a)).Select(y => y.DictEntry).ToList())
                    .ToReactiveProperty();

                BoundView.PropertyChanged += (sender, args) =>
                {
                    if (BoundView.Value != null)
                    {
                        RtxtCalismaYazisi = (RichTextBox)BoundView.Value.GetFramworkElementByName("RtboxCalisma");
                    }
                };

                LoadWordCollections();
            }
            // ReSharper disable RedundantIfElseBlock
            else
            {
                //todo: DesignTime data should be defined here
            }
            // ReSharper restore RedundantIfElseBlock
        }

        private void IncreaseBookmarksStudyProgresses(IEnumerable<ShiroEntryBzzt> shiroEntryBzzts, int countForEachRecord)
        {
            foreach (var shiroEntryBzzt in shiroEntryBzzts)
            {
                WritingProgressController.IncreaseProgress(shiroEntryBzzt.Spellings[0].Value, countForEachRecord);
            }
        }

        public RelayCommand PrintStrokeOrdersCommand { get; set; }
        public RelayCommand CalismaHazirlaCommand { get; set; }
        public RelayCommand PrintCommand { get; set; }

        public ReactiveProperty<BookmarkCollection> SelectedCollection { get; set; }
        public ReactiveProperty<List<BookmarkExt>> SelectedCollectionContent { get; set; }

        public ReactiveProperty<List<ShiroEntryBzzt>> Words { get; set; }

        public ReactiveProperty<string> DocumentXaml { get; set; }

        public RangeObservableCollection<StrokeOrderGraphList> StrokeOrderUiElements { get; set; }

        public RangeObservableCollection<BookmarkCollection> WordCollections { get; set; }

        public DistinctObservableCollection<ShiroEntryBzzt> RandomWords { get; set; }

        public ReactiveProperty<int> WordCount { get; set; }

        public ReactiveProperty<int> TotalRepeatCount { get; set; }

        public ReactiveProperty<int> MaxAllowedRepeatCountForAWord { get; set; }

        public RichTextBox RtxtCalismaYazisi { get; set; }

        private void LoadWordCollections()
        {
            //var strings = Directory.GetFiles(KelimeList.ListDirectory).Select(Path.GetFileName);
            var kelimeLists = BookmarkController.GetAllBookmarkCollections().ToList();
            WordCollections.Clear();
            WordCollections.AddRange(kelimeLists);
        }

        public void CalismaHazirla()
        {
            if (Words.Value.Count > 0)
            {
                var randomWords = GenerateRandomWordList(WordCount.Value, Words.Value.ToList());
                RandomWords.AddRange(randomWords);
                var studyText = GenerateStudyPage(TotalRepeatCount.Value, randomWords, MaxAllowedRepeatCountForAWord.Value);
                string xaml = string.Format("<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" +
                                            "<Paragraph>{0}</Paragraph>" +
                                            "</FlowDocument>", studyText);

                //using (var msDocument = new MemoryStream((new ASCIIEncoding()).GetBytes(_myString)))
                //{
                //    doc = XamlReader.Load(msDocument) as FlowDocument;
                //}
                DocumentXaml.Value = xaml;
            }
            else
            {
                MessageBox.Show("No files is loaded");
            }
        }

        public void CalismaHazirla2()
        {
            RichTextBox rtxtCalismaYazisi = RtxtCalismaYazisi;
            RandomWords.Clear();
            RandomWords.AddRange(GenerateRandomWordList(WordCount.Value, Words.Value.ToList()));
            StringBuilder rastgeleKelimeString = GenerateStudyPage(TotalRepeatCount.Value, RandomWords, MaxAllowedRepeatCountForAWord.Value);

            rtxtCalismaYazisi.Document.Blocks.Clear();
            rtxtCalismaYazisi.AppendText(rastgeleKelimeString.ToString());
        }

        /// <summary>
        ///     yazı çalışması için rastgele kelime kökleri + boşluk yapısında string hazırlar
        /// </summary>
        public StringBuilder GenerateStudyPage(int tekrarSayisi, IEnumerable<ShiroEntryBzzt> kelimeler,
            int maxAllowedRepeatCountForAWord = 0)
        {
            const string whiteSpace = "      ";
            List<ShiroEntryBzzt> kelimeList = kelimeler.ToList();
            var kelimeString = new StringBuilder();
            kelimeList.Sort((t1, t2) => String.Compare(t1.Meanings[0].Gloss[0], t2.Meanings[0].Gloss[0], StringComparison.Ordinal));
            //list readings-writings to be study reference
            foreach (ShiroEntryBzzt word in kelimeList)
            {
                var meaning = word.Meanings[0].Gloss[0].TrimEnd();
                var writing = word.GroupedSpellingsTuple[0].Item2[0];
                var reading = word.GroupedSpellingsTuple[0].Item1[0];
                kelimeString.AppendFormat("[{0} : {1}:{2}]{3}", meaning, writing, reading, whiteSpace);
            }

            //add seperator
            kelimeString.AppendLine();
            kelimeString.AppendLine("------------------------------------");

            //tekrarSayısı kadar tüm kelimeler rastgele tekrar tekrar listeleniyor
            var rnd = new Random();
            var producedRandomList = new List<ShiroEntryBzzt>();
            for (int i = 0; i < tekrarSayisi && kelimeList.Count > 0; i++)
            {
                ShiroEntryBzzt kelime = kelimeList[rnd.Next(kelimeList.Count)];
                producedRandomList.Add(kelime);
                if (maxAllowedRepeatCountForAWord != 0)
                {
                    //kelime izin verilenden cok tekrar etmişse dışlanıyor
                    if (producedRandomList.Count(word => word == kelime) >= maxAllowedRepeatCountForAWord)
                    {
                        kelimeList.RemoveAll(word => word == kelime);
                    }
                }
            }
            foreach (ShiroEntryBzzt word in producedRandomList)
            {
                var meaning = word.Meanings[0].Gloss[0].TrimEnd().RestrictLenght(20);
                var writing = word.GroupedSpellingsTuple[0].Item2[0];
                kelimeString.AppendFormat("[{0}>{1}]   ", meaning, whiteSpace.Multiply(writing.Length));
            }
            return kelimeString;
        }

        public List<ShiroEntryBzzt> GenerateRandomWordList(int farkliKelimeSayisi, List<ShiroEntryBzzt> kelimeler)
        {
            //todo: bu metod strategy pattern şeklinde olabilir, farklı mantıklarla kelime üretilebilir(mesela tamamen rastgele veya en az tekrar edilenlerden rastgele v.s.)
            var rnd = new Random();
            var secilenKelimeler = new DistinctList<ShiroEntryBzzt>();

            if (kelimeler.Count < farkliKelimeSayisi) farkliKelimeSayisi = kelimeler.Count;
            while (secilenKelimeler.Count < farkliKelimeSayisi)
            {
                ShiroEntryBzzt a0 = kelimeler[rnd.Next(kelimeler.Count)];
                secilenKelimeler.Add(a0);
            }
            return secilenKelimeler;
        }
    }
}

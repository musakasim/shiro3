using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using GalaSoft.MvvmLight.Messaging;
using Shiro.Controller;
using Shiro.Library;
using Shiro.Model;
using Reactive.Bindings;

namespace Shiro.ViewModel
{
    public class DictionaryViewModel : MainViewModel
    {
        /// <summary>
        /// this ctor is defined so xaml can initilize an instance of this viewmodel(xaml can't see the ctor with parameter with default value)
        /// </summary>
        public DictionaryViewModel()
            : this(false)
        {

        }

        public DictionaryViewModel(bool useStaticContextData)
        {
            SearchLangModes = Enum.GetValues(typeof(SearchLangMode)).Cast<SearchLangMode>();
            if (IsInDesignMode || useStaticContextData)
            {
                LoadDesignTimeStaticData();
            }
            else
            {
                SelectedDictionaryEntry = new ReactiveProperty<ShiroEntryBzzt>();
                SelectedItemViewModel = new DictionaryEntryViewModel(SelectedDictionaryEntry);

                SelectedDictionaryEntryHistory = new DistinctObservableCollection<ShiroEntryBzzt>();
                SelectedDictionaryEntry.Subscribe(t => { if (t != null) SelectedDictionaryEntryHistory.Add(t, true); });

                KanjiListViewModel = new KanjiListViewModel(SelectedDictionaryEntry);

                SearchLangMode = new ReactiveProperty<SearchLangMode>(Controller.SearchLangMode.JapToEng);
                SearchTerm = new ReactiveProperty<string>();

                SearchResults = SearchTerm
                    .DistinctUntilChanged() //when new SearchTerm is typed then cancel the previous
                    .Throttle(TimeSpan.FromMilliseconds(300))
                    .Select(async searchTerm => await ShiroDictionaryController.SearchAsync(searchTerm, SearchLangMode.Value).ConfigureAwait(false))
                    .Switch()
                    .Select(entries => entries.Select(u => new ShiroEntryBzzt(u)))
                    .ToReactiveProperty();
                //.TakeUntil(SearchTerm)

                /////////////////////////////////////////
                // TODO regİON: BURADA KULLANICININ GİRDİĞİ LATİN DEĞERİ SAKLAYIP TEXTBOX'TA HER ZAMAN HIRAGANA GÖSTERMEYE ÇALIŞIYORUM, taMAMLA
                // tODO: en iyisi bir label ekle transparant olsun, text box üzerinde dursun, sağa yaslı olsun, hiragana değer bunda görünsün
                // bkz.https://stackoverflow.com/questions/23225751/wpf-binding-to-two-properties
                //SearchTermRomaji = SearchTerm
                //    .Select(x =>
                //    JapaneseKanaClassifier.IsRomaji(x ?? "") ? x : ""
                //    )
                //    .ToReactiveProperty();
                //SearchTermHiragana = SearchTermRomaji
                //    .DistinctUntilChanged()
                //    .Select(x =>
                //        {
                //            string hiragana;
                //            JapaneseKanaConverter.TryParseRomajiToHiragana(x ?? "", out hiragana);
                //            if (!String.IsNullOrEmpty(hiragana) || !String.IsNullOrEmpty(SearchTerm.Value))
                //                SearchTerm.Value = hiragana;
                //            return hiragana;
                //        })
                //    .ToReactiveProperty();
                //SearchTerm = SearchTermHiragana.ToReactiveProperty();
                // TODO regİON: BURADA KULLANICININ GİRDİĞİ LATİN DEĞERİ SAKLAYIP TEXTBOX'TA HER ZAMAN HIRAGANA GÖSTERMEYE ÇALIŞIYORUM, taMAMLA
                /////////////////////////////////////////

                Messenger.Default.Register<DictionaryEntryViewModel.SearchDictMessage>(this, searchMessage =>
                {
                    PropertyChangedEventHandler eventHandler = null;

                    eventHandler = (sender, args) =>
                    {
                        SearchResults.Subscribe(t =>
                        {
                            var entries = t as IList<ShiroEntryBzzt> ?? t.ToList();
                            if (entries.Any())
                                SelectedDictionaryEntry.Value = entries.First();

                            // unsubscribe itself after first run
                            SearchResults.PropertyChanged -= eventHandler;
                        });
                    };

                    SearchResults.PropertyChanged += eventHandler;

                    SearchTerm.Value = searchMessage.SearchText;

                });
            }
        }

        private new void LoadDesignTimeStaticData()
        {
            var designTimeEntries = DesignTimeData.GetDictionaryEntries();
            //Prepare Designer Data
            SearchLangMode = new ReactiveProperty<SearchLangMode>(Controller.SearchLangMode.JapToEng);
            SearchTerm = new ReactiveProperty<string>("あら");
            SearchTermRomaji = new ReactiveProperty<string>("ara");
            SearchTermHiragana = new ReactiveProperty<string>("あら");
            SearchResults = new ReactiveProperty<IEnumerable<ShiroEntryBzzt>>(designTimeEntries);
            SelectedDictionaryEntry = new ReactiveProperty<ShiroEntryBzzt>(designTimeEntries[0]);
            SelectedItemViewModel = new DictionaryEntryViewModel();
            KanjiListViewModel = new KanjiListViewModel();
            SelectedDictionaryEntryHistory = new DistinctObservableCollection<ShiroEntryBzzt>();
            SelectedDictionaryEntryHistory.AddRange(new List<ShiroEntryBzzt>
            {
                DesignTimeData.GetDictionaryEntry1(),
                DesignTimeData.GetDictionaryEntry2()
            });
        }

        /// <summary>
        /// ShiroDictionaryController.Search metodu çağrılırken SearchLangMode kullanılıyor,
        /// SearchLangMode değer alıp, bindingler patlamaması için bu prop'un yüklenmesi gerekiyor
        /// </summary>
        public IEnumerable<SearchLangMode> SearchLangModes { get; set; }
        public ReactiveProperty<SearchLangMode> SearchLangMode { get; set; }
        public ReactiveProperty<string> SearchTerm { get; set; }
        public ReactiveProperty<string> SearchTermHiragana { get; set; }
        public ReactiveProperty<string> SearchTermRomaji { get; set; }
        public ReactiveProperty<IEnumerable<ShiroEntryBzzt>> SearchResults { get; set; }

        public DistinctObservableCollection<ShiroEntryBzzt> SelectedDictionaryEntryHistory { get; set; }
        public ReactiveProperty<ShiroEntryBzzt> SelectedDictionaryEntry { get; set; }
        public DictionaryEntryViewModel SelectedItemViewModel { get; set; }
        public KanjiListViewModel KanjiListViewModel { get; set; }
    }

}
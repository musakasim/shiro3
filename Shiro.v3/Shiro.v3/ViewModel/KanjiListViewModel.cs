using System.Collections.Generic;
using System.Reactive.Linq;
using Reactive.Bindings;
using Shiro.Controller;
using Shiro.Model;

namespace Shiro.ViewModel
{
    public class KanjiListViewModel : MainViewModel
    {
        /// <summary>
        /// thıs should be used only by presentetation framework
        /// todo:write test about call references !!! nasıl olacaksa MethodCallCount must be 0
        /// </summary>
        public KanjiListViewModel()
        {
            if (IsInDesignMode)
            {
                var initialValue = new List<KanjiInfo>
                {
                    DesignTimeData.GetKanjiDic2Entry1(),
                    new KanjiInfo {Kanji = "漢"}
                };
                Kanjis = new ReactiveProperty<List<KanjiInfo>>(initialValue);
                SelectedKanji = new ReactiveProperty<KanjiInfo>(initialValue[0]);
                SelectedKanjiViewModel = new KanjiViewModel();
            }
        }

        public KanjiListViewModel(ReactiveProperty<ShiroEntryBzzt> dictionaryEntry)
        {
            if (!IsInDesignMode)
            {
                KanjiInfoController = new KanjiInfoController();

                SelectedKanji = new ReactiveProperty<KanjiInfo>();
                SelectedKanjiViewModel = new KanjiViewModel(SelectedKanji);

                DictionaryEntry = dictionaryEntry.ToReactiveProperty();
                //Set DictionaryEntry.Kanjis to this viewmodel's kanji list
                Kanjis = DictionaryEntry.Select(t => t == null ? new List<KanjiInfo>() : t.Kanjis).ToReactiveProperty();
                //todo:consider:when kanjis loaded first kanji in list will be shown on kanjiView as selected:
            }
        }

        public ReactiveProperty<ShiroEntryBzzt> DictionaryEntry { get; set; }

        /// <summary>
        /// I need count property of the list thats why this is <![CDATA[ReactiveProperty<List<KanjiInfo>>]]>
        /// </summary>
        public ReactiveProperty<List<KanjiInfo>> Kanjis { get; set; }
        public ReactiveProperty<KanjiInfo> SelectedKanji { get; set; }
        public KanjiViewModel SelectedKanjiViewModel { get; set; }

    }
}
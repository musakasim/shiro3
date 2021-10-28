using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using GalaSoft.MvvmLight.Command;
using Reactive.Bindings;
using Shiro.Model;

namespace Shiro.ViewModel
{
    public class BookmarkMiniViewModel : MainViewModel
    {

        public RelayCommand<BookmarkCollection> BookmarkEntryCommand { get; set; }

        public ReactiveProperty<ShiroEntryBzzt> DictionaryEntry { get; set; }

        public ReactiveProperty<List<BookmarkCollection>> BookmarkCollections { get; set; }
        public ReactiveProperty<List<BookmarkCollectionCheck>> DictionaryEntryBookmarkCollections { get; set; }

        //todo:Dependency property yapilip xaml'dan ayarlanabilir mi?
        public BookmarkType BookmarkType { get; set; }

        public BookmarkMiniViewModel()
        {
            if (IsInDesignMode)
            {
                DictionaryEntryBookmarkCollections = new ReactiveProperty<List<BookmarkCollectionCheck>>(DesignTimeData.GetBookmarkCollectionExts());
            }
        }

        public BookmarkMiniViewModel(ReactiveProperty<ShiroEntryBzzt> dictionaryEntry)
        {
            if (!IsInDesignMode)
            {
                BookmarkType = BookmarkType.ShiroEntryBookmark;
                BookmarkCollections = new ReactiveProperty<List<BookmarkCollection>>(BookmarkController.GetAllBookmarkCollections().ToList());
                DictionaryEntryBookmarkCollections = new ReactiveProperty<List<BookmarkCollectionCheck>>(BookmarkCollections.Value.Select(t => new BookmarkCollectionCheck(t)).ToList());

                BookmarkEntryCommand = new RelayCommand<BookmarkCollection>(t =>
                {
                    //Add bookmark or remove bookmark for the entry
                    if (DictionaryEntry.Value.BookmarkCollections.Any(z => z.Id == t.Id))
                    {
                        BookmarkController.RemoveBookmark(BookmarkType, DictionaryEntry.Value.Id, t.Id);
                        //todo:ugly:take care of,to show immediate result in search list i added bookmark to entry, but doesn't affect for now, dictionaryentry.bookmarks must be mend as observablecollection i guess
                        // so here i remove the removed bookmark manually
                        DictionaryEntry.Value.BookmarkCollections.Remove(DictionaryEntry.Value.BookmarkCollections.First(u => u.Id == t.Id));
                    }
                    else
                    {
                        var bookmark = BookmarkController.BookmarkIt(BookmarkType, DictionaryEntry.Value.Id, t.Id);
                        var bookmarkExt = new BookmarkExt(bookmark);
                        DictionaryEntry.Value.BookmarkCollections.Add(new BookmarkCollection { Color = bookmarkExt.Color, Id = bookmarkExt.CollectionId });
                    }
                });

                DictionaryEntry = dictionaryEntry;
                DictionaryEntryBookmarkCollections = DictionaryEntry
                    .Select(u => BookmarkCollections.Value.Select(a => new BookmarkCollectionCheck(a)).ToList())
                    .Do(r =>
                    {
                        //todo:wtf, no binding, doing all manually:,fix this
                        if (DictionaryEntry.Value != null)
                            r.ForEach(p => p.IsBookmarked = DictionaryEntry.Value.BookmarkCollections.Any(i => i.Id == p.Id));
                    })
                    .ToReactiveProperty();
                //DictionaryEntry.PropertyChanged += (sender, args) =>
                //{
                //    if (DictionaryEntry.Value != null)
                //        DictionaryEntryBookmarkCollections.Value.ForEach(p => p.IsBookmarked = DictionaryEntry.Value.BookmarkCollections.Any(i => i.Id == p.Id));
                //};
            }
        }
    }
}

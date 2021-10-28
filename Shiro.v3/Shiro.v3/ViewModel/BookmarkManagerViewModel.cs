using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using GalaSoft.MvvmLight.Command;
using Reactive.Bindings;
using Shiro.Library;
using Shiro.Model;

namespace Shiro.ViewModel
{
    public class BookmarkManagerViewModel : MainViewModel
    {
        public ReactiveProperty<List<BookmarkCollection>> BookmarkCollections { get; set; }
        public ReactiveProperty<BookmarkCollection> SelectedBookmarkCollection { get; set; }
        public ReactiveProperty<List<BookmarkExt>> SelectedBookmarkCollectionContent { get; set; }
        public ReactiveCollection<BookmarkExt> DictEntryBookmarks { get; set; }
        public ReactiveCollection<BookmarkExt> KanjiBookbarks { get; set; }

        public RelayCommand SaveBookmarkCollectionCommand { get; set; }
        public RelayCommand NewBookmarkCollectionCommand { get; set; }
        public RelayCommand ClearBookmarkCollectionCommand { get; set; }
        public RelayCommand ResetBookmarkCollectionProgressCommand { get; set; }
        public RelayCommand ExportBookmarkCollectionCommand { get; set; }
        public RelayCommand ImportBookmarkCollectionCommand { get; set; }

        public BookmarkManagerViewModel()
        {
            if (IsInDesignMode)
            {
                BookmarkCollections = new ReactiveProperty<List<BookmarkCollection>>(DesignTimeData.GetBookmarkCollections());
                SelectedBookmarkCollection = new ReactiveProperty<BookmarkCollection>(BookmarkCollections.Value[1]);
            }
            else
            {
                BookmarkCollections = new ReactiveProperty<List<BookmarkCollection>>(BookmarkController.GetAllBookmarkCollections().ToList());
                SelectedBookmarkCollection = new ReactiveProperty<BookmarkCollection>();

                //SelectedBookmarkCollectionContent = SelectedBookmarkCollection
                //    .Select(t => t != null ? BookmarkController.GetCollectionContent(t.Id).ToList() : new List<Bookmark>())
                //    .Select(a => a.Select(s => new BookmarkExt(s)).ToList())
                //    .ToReactiveProperty();

                //DictEntryBookmarks = SelectedBookmarkCollectionContent
                //    .Select(t => t.Where(r => r.Type == BookmarkType.ShiroEntryBookmark).ToList())
                //    .ToReactiveProperty();
                //KanjiBookbarks = SelectedBookmarkCollectionContent
                //    .Select(t => t.Where(r => r.Type == BookmarkType.KanjiBookmark).ToList())
                //    .ToReactiveProperty();

                DictEntryBookmarks = SelectedBookmarkCollection
                    .Select(t => t != null ? BookmarkController.GetCollectionContent(t.Id, BookmarkType.ShiroEntryBookmark)  : new List<Bookmark>())
                    .SelectMany(a => a.Select(s => new BookmarkExt(s)).ToList())
                    .ToReactiveCollection();
                KanjiBookbarks = SelectedBookmarkCollection
                    .Select(t => t != null ? BookmarkController.GetCollectionContent(t.Id, BookmarkType.KanjiBookmark)  : new List<Bookmark>())
                    .SelectMany(a => a.Select(s => new BookmarkExt(s)).ToList())
                    .ToReactiveCollection();

                SaveBookmarkCollectionCommand = new RelayCommand(() =>
                    {
                        BookmarkController.SaveBookmarkCollection(SelectedBookmarkCollection.Value);
                        BookmarkCollections.Value = BookmarkController.GetAllBookmarkCollections(true).ToList();
                        SelectedBookmarkCollection.Value = null;
                    },
                    () => SelectedBookmarkCollection.Value != null);
                NewBookmarkCollectionCommand = new RelayCommand(() => SelectedBookmarkCollection.Value = new BookmarkCollection());
                ClearBookmarkCollectionCommand = new RelayCommand(() =>
                    {
                        BookmarkController.ClearBookmarkCollection(SelectedBookmarkCollection.Value.Id);
                        DictEntryBookmarks.Clear();
                    },
                    () => DictEntryBookmarks.Any());
                ResetBookmarkCollectionProgressCommand = new RelayCommand(() => BookmarkController.ResetBookmarkCollectionProgress(SelectedBookmarkCollection.Value.Id),
                    () => SelectedBookmarkCollection.Value != null);
                ExportBookmarkCollectionCommand = new RelayCommand(() => BookmarkController.Export(SelectedBookmarkCollection.Value.Id),
                    () => SelectedBookmarkCollection.Value != null);

                ImportBookmarkCollectionCommand = new RelayCommand(() =>
                    {
                        var importFilePath = FileHelper.ShowFileSelectDialog();
                        if (!string.IsNullOrEmpty(importFilePath))
                            BookmarkController.Import(SelectedBookmarkCollection.Value.Id, importFilePath);
                    },
                    () => SelectedBookmarkCollection.Value != null);
            }
        }
    }
}

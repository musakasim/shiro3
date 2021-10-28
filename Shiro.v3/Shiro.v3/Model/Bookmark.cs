using System.Windows.Media;
using FrInterfaces;
using Shiro.Controller;

namespace Shiro.Model
{

    public class BookmarkCollection : IBaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public Color Color { get; set; }

    }

    /// <summary>
    /// used for xaml binding, 
    /// example : a list of BookmarkCollectionCheck is prepared for an entry with it's bookmarks are marked as bookmarked (IsBookmarked = true)
    /// </summary>
    public class BookmarkCollectionCheck : BookmarkCollection
    {
        public bool IsBookmarked { get; set; }

        public BookmarkCollectionCheck(BookmarkCollection bCollection)
        {
            Id = bCollection.Id;
            Name = bCollection.Name;
            Color = bCollection.Color;
        }
    }

    public class Bookmark : IBaseModel
    {
        public int Id { get; set; }

        public BookmarkType Type { get; set; }

        /// <summary>
        /// can be shiroentry or kanji id according to BookmarkType
        /// </summary>
        public int RefId { get; set; }

        /// <summary>
        /// BookmarkCollection Id
        /// </summary>
        public int CollectionId { get; set; }
    }

    public class BookmarkExt : Bookmark
    {
        /// <summary>
        /// todo:find a way to get rid of this controllers in data model
        /// </summary>
        static readonly BookmarkController BookmarkController = new BookmarkController();
        static readonly ShiroDictionaryController ShiroDictionaryController = new ShiroDictionaryController();
        static readonly KanjiInfoController KanjiInfoController = new KanjiInfoController();
        static readonly WritingProgressController WritingProgressController = new WritingProgressController();

        public BookmarkExt(Bookmark bookmark)
        {
            Id = bookmark.Id;
            RefId = bookmark.RefId;
            Type = bookmark.Type;
            CollectionId = bookmark.CollectionId;
        }

        public Color Color
        {
            get
            {
                var bookmarkCollection = BookmarkController.GetCollection(CollectionId);
                return (bookmarkCollection != null) ? bookmarkCollection.Color : Colors.RoyalBlue;
            }
        }
        public ShiroEntryBzzt DictEntry { get { return Type == BookmarkType.ShiroEntryBookmark ? ShiroDictionaryController.Get(RefId) : null; } }
        public KanjiInfo KanjiInfo { get { return Type == BookmarkType.KanjiBookmark ? KanjiInfoController.Get(RefId) : null; } }

        public string PeekString { get { return Type == BookmarkType.KanjiBookmark ? KanjiInfo.Kanji : DictEntry.Spellings[0].Value; } }
        public WritingProgress WritingProgress { get { return WritingProgressController.GetProgress(PeekString); } }
    }

    public enum BookmarkType
    {
        ShiroEntryBookmark,
        KanjiBookmark
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Shiro.Model;

namespace Shiro.Controller
{
    public interface IBookmarkController : IBaseController
    {
        void Export(int bookmarkCollectionId);
        void Import(int bookmarkCollectionId, string fileName);

        /// <summary>
        /// Saves a new bookmark for the type with the record id indicated by the id parameter
        /// </summary>
        Bookmark BookmarkIt(BookmarkType type, int id, int bookmarkCollectionId);

        /// <summary>
        /// Saves a new bookmark for the type with the record id indicated by the id parameter
        /// </summary>
        void RemoveBookmark(BookmarkType type, int id, int bookmarkCollectionId);

        IEnumerable<BookmarkCollection> GetAllBookmarkCollections(bool forceFetchFromDb = false);

        /// <summary>
        /// Selects bookmarks by collection id
        /// </summary>
        /// <param name="bookmarkCollectionId"></param>
        /// <returns></returns>
        IEnumerable<Bookmark> GetCollectionContent(int bookmarkCollectionId);
        IEnumerable<Bookmark> GetCollectionContent(int bookmarkCollectionId, BookmarkType type);
        void SaveBookmarkCollection(BookmarkCollection bookmarkCollection);
        IEnumerable<BookmarkCollection> GetBookmarksOfData(int refId, BookmarkType bookmarkType);

        /// <summary>
        /// Removes all bookmark records for specified Collection
        /// </summary>
        void ClearBookmarkCollection(int bookmarkCollectionId);

        void ResetBookmarkCollectionProgress(int bookmarkCollectionId);
    }

    public class BookmarkController : BaseController, IBookmarkController
    {
        /// <summary>
        /// Application wide Bookmark collections isn't a thing that changes usually
        /// so all collections are hold in this static property
        /// </summary>
        public static List<BookmarkCollection> BookmarkCollections { get; set; }

        public void Export(int bookmarkCollectionId)
        {
            var collection = GetCollection(bookmarkCollectionId);
            var content = GetCollectionContent(bookmarkCollectionId);
            var streamWriter = new StreamWriter(collection.Name + ".csv", false, Encoding.UTF8);
            streamWriter.WriteLine("#{0}", collection.Name);
            streamWriter.WriteLine("#{0}\t{1}\t{2}\t{3}", "item.RefId", "item.Type", "bookmarkExt.PeekString", "bookmarkExt.WritingProgress.WriteCount");
            foreach (var item in content)
            {
                var bookmarkExt = new BookmarkExt(item);
                streamWriter.WriteLine("{0}\t{1}\t{2}\t{3}", item.RefId, item.Type, bookmarkExt.PeekString, bookmarkExt.WritingProgress.WriteCount);
            }
            streamWriter.Close();
        }

        public void Import(int bookmarkCollectionId, string fileName)
        {
            var streamReader = new StreamReader(fileName, Encoding.UTF8);
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                if (line.StartsWith("#")) continue;
                var strings = line.Split('\t');
                var entryId = Convert.ToInt32(strings[0]);
                var type = (BookmarkType)Enum.Parse(typeof(BookmarkType), strings[1]);
                BookmarkIt(type, entryId, bookmarkCollectionId);
            }
        }

        /// <summary>
        /// if bookmarks to be deleted comprise the whole table content table will be deleted(because if not do this way stsdb throws The set is empty error )
        /// </summary>
        /// <param name="bookmarkCollectionId"></param>
        public void ClearBookmarkCollection(int bookmarkCollectionId)
        {
            var bookmarks = GetCollectionContent(bookmarkCollectionId).ToList();
            var allCount = ShiroRepository.Count<Bookmark>();
            if (allCount == bookmarks.Count)
                ShiroRepository.Drop<Bookmark>();
            else
                bookmarks.ToList().ForEach(a => ShiroRepository.Delete<Bookmark>(a.Id));
        }

        public void ResetBookmarkCollectionProgress(int bookmarkCollectionId)
        {
            var bookmarks = GetCollectionContent(bookmarkCollectionId);
            var bookmarkExts = bookmarks.Select(t => new BookmarkExt(t)).ToList();
            foreach (var bookmark in bookmarkExts.Where(e => e.WritingProgress.WriteCount > 0))
            {
                var writingProgress = bookmark.WritingProgress;
                writingProgress.WriteCount = 0;
                ShiroRepository.Delete<WritingProgress>(writingProgress.Id);
            }
        }

        /// <summary>
        /// If a bookmark that fits the parameters doesn't exists  adds new bookmark
        /// otherwise just returns the existing one
        /// </summary> 
        /// <returns>returns added or existing bookmark</returns> 
        public Bookmark BookmarkIt(BookmarkType type, int id, int bookmarkCollectionId)
        {
            var bookmark = ShiroRepository.GetSingle<Bookmark>(t => t.CollectionId == bookmarkCollectionId && t.Type == type && t.RefId == id);

            if (bookmark == null)
            {
                if (type == BookmarkType.ShiroEntryBookmark)
                {
                    bookmark = new Bookmark { RefId = id, Type = type, CollectionId = bookmarkCollectionId };
                    ShiroRepository.Save(bookmark);
                }
                //todo:else???gg
            }

            return bookmark;
        }

        /// <summary>
        /// If a bookmark that fits the parameters exists removes that bookmark
        /// </summary> 
        public void RemoveBookmark(BookmarkType type, int id, int bookmarkCollectionId)
        {
            var bookmark = ShiroRepository.GetSingle<Bookmark>(predicate: t => t.CollectionId == bookmarkCollectionId && t.Type == type && t.RefId == id);
            if (bookmark != null)
            {
                ShiroRepository.Delete<Bookmark>(bookmark.Id);
            }
        }

        public IEnumerable<BookmarkCollection> GetAllBookmarkCollections(bool forceFetchFromDb = false)
        {
            if (BookmarkCollections == null || forceFetchFromDb)
            {
                BookmarkCollections = ShiroRepository.GetMany<BookmarkCollection>().ToList();
            }
            return BookmarkCollections;
        }

        public IEnumerable<Bookmark> GetCollectionContent(int bookmarkCollectionId)
        {
            return ShiroRepository.GetMany<Bookmark>(predicate: t => t.CollectionId == bookmarkCollectionId);
            // var collectionContent = ShiroRepository.GetMany<Bookmark>().ToList();
            //return collectionContent;
        }

        public IEnumerable<Bookmark> GetCollectionContent(int bookmarkCollectionId, BookmarkType type)
        {
            return ShiroRepository.GetMany<Bookmark>(predicate: t => t.CollectionId == bookmarkCollectionId && t.Type == type);
        }

        public void SaveBookmarkCollection(BookmarkCollection bookmarkCollection)
        {
            ShiroRepository.Save(bookmarkCollection);
        }

        public IEnumerable<BookmarkCollection> GetBookmarksOfData(int refId, BookmarkType bookmarkType)
        {
            var bookmarks = ShiroRepository.GetMany<Bookmark>(predicate: t => t.RefId == refId && t.Type == bookmarkType);
            var collections = bookmarks.Select(y => GetCollection(y.CollectionId));
            return collections;
        }

        public BookmarkCollection GetCollection(int collectionId)
        {
            return ShiroRepository.GetSingle<BookmarkCollection>(collectionId);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FrInterfaces;
using JapaneseResources.Models.ReworkedModels;
using Shiro.Model;

namespace Shiro.Controller
{
    /// <summary>
    /// This class provides static data to test interface
    /// basically aimed to use in designtime 
    /// </summary>
    public class ShiroStaticDataRepository : IRepository
    {

        public void Drop(string tableName)
        {
        }
        public void Drop<T>() where T : IBaseModel
        {
        }

        public int Count<T>(Expression<Func<T, bool>> predicate = null) where T : IBaseModel
        {
            return 2;
        }

        public void Delete<T>(T obj) where T : IBaseModel
        {
        }

        public void Delete<T>(int id) where T : IBaseModel
        {
        }

        public void Delete<T>(Expression<Func<T, bool>> predicate = null) where T : IBaseModel
        {
        }

        public void Save<T>(T entity) where T : IBaseModel
        {
        }

        public void Save<T>(List<T> entities) where T : IBaseModel
        {
        }

        public T GetSingle<T>(Expression<Func<T, bool>> predicate = null) where T : IBaseModel
        {
            var typeee = typeof(T);
            if (typeee == typeof(ShiroEntryBzzt))
            {
                return (T)(object)DesignTimeData.GetDictionaryEntry1();
            }
            if (typeee == typeof(TatoebaSentence))
            {
                return (T)(object)DesignTimeData.GetExampleSentence1();
            }
            if (typeee == typeof(KanjiGraph))
            {
                return (T)(object)DesignTimeData.GetKanjiGraph1();
            }
            if (typeee == typeof(KanjiDic2Entry))
            {
                return (T)(object)DesignTimeData.GetKanjiDic2Entry1();
            }
            return default(T);
        }

        public T GetSingle<T>(int id) where T : IBaseModel
        {
            return GetSingle<T>();
        }

        public IEnumerable<T> GetMany<T>(int takeCount = 0, Expression<Func<T, bool>> predicate = null)
            where T : IBaseModel
        {

            var typeee = typeof(T);
            if (typeee == typeof(ShiroEntryBzzt) || typeee == typeof(ShiroEntry))
            {
                return (IEnumerable<T>)(object)new[] { DesignTimeData.GetDictionaryEntry1() };
            }
            if (typeee == typeof(TatoebaSentence))
            {
                return (IEnumerable<T>)(object)new[] { DesignTimeData.GetExampleSentence1() };
            }
            if (typeee == typeof(KanjiGraph))
            {
                return (IEnumerable<T>)(object)new[] { DesignTimeData.GetKanjiGraph1() };
            }
            if (typeee == typeof(KanjiDic2Entry))
            {
                return (IEnumerable<T>)(object)new[] { DesignTimeData.GetKanjiDic2Entry1() };
            }
            if (typeee == typeof(BookmarkCollection))
            {
                return (IEnumerable<T>)DesignTimeData.GetBookmarkCollectionExts();
            } 
            return new List<T>();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FrInterfaces;

namespace Shiro.Controller
{
    /// <summary>
    /// Wrapper class for PersistencyProvider which implements IRepository in case of additional needs
    /// eg: IRepository is not aware of any kind of validator
    /// </summary>
    public class ShiroRepository : IRepository
    {
        public ShiroRepository(IRepository repository)
        {
            PersistencyProvider = repository;
        }

        private static IRepository PersistencyProvider { get; set; }

        public void Drop(string tableName)
        {
            PersistencyProvider.Drop(tableName);
        }
        public void Drop<T>() where T : IBaseModel
        {
            PersistencyProvider.Drop<T>();
        }

        public int Count<T>(Expression<Func<T, bool>> predicate = null) where T : IBaseModel
        {
            return PersistencyProvider.Count(predicate);
        }

        public void Delete<T>(T obj) where T : IBaseModel
        {
            PersistencyProvider.Delete(obj);
        }

        public void Delete<T>(int id) where T : IBaseModel
        {
            PersistencyProvider.Delete<T>(id);
        }

        public void Delete<T>(Expression<Func<T, bool>> predicate = null) where T : IBaseModel
        {
            PersistencyProvider.Delete(predicate);
        }

        public void Save<T>(T entity) where T : IBaseModel
        {
            PersistencyProvider.Save(entity);
        }

        public void Save<T>(List<T> entities) where T : IBaseModel
        {
            PersistencyProvider.Save(entities);
        }

        public T GetSingle<T>(Expression<Func<T, bool>> predicate = null) where T : IBaseModel
        {
            T single = PersistencyProvider.GetSingle(predicate);
            return single;
        }

        public T GetSingle<T>(int id) where T : IBaseModel
        {
            T single = PersistencyProvider.GetSingle<T>(id);
            return single;
        }

        public IEnumerable<T> GetMany<T>(int takeCount = 0, Expression<Func<T, bool>> predicate = null)
            where T : IBaseModel
        {
            IEnumerable<T> many = PersistencyProvider.GetMany(takeCount, predicate);
            return many;
        }
    }
}
using System;

namespace Shiro.Library
{
    public static class LambdaHelpers
    {
        /// <summary>
        ///     Can be used where you use 'Let' in linq.
        ///     But can be use for any pipe action
        ///     instead of :
        ///     from store in Stores
        ///     let AveragePrice =  store.Sales.Average(s => s.Price)
        ///     where AveragePrice == 500
        ///     makes possible to make the call:
        ///     var results = Stores
        ///     .Where(store => store.Sales.Average(s => s.Price)
        ///     .Pipe(averagePrice => averagePrice == 500));
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="_this"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static TResult Pipe<T, TResult>(this T _this, Func<T, TResult> func)
        {
            return func(_this);
        }
    }
}
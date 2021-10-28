using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Shiro.Library
{
    public class DistinctObservableCollection<T> : ObservableCollection<T>
    {
        private bool _suppressNotification;

        public void Add(T item, bool forceItemStandsAtTheEnd = false)
        {
            if (item != null)
            {
                //to enforce item stands at the end, first remove
                if (forceItemStandsAtTheEnd)
                {
                    if (this.Any(x => Equals(x, item)))
                        this.Remove(item);
                }
                if (this.All(x => !Equals(x, item)))
                {
                    base.Add(item);
                }
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
                base.OnCollectionChanged(e);
        }

        public void AddRange(IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            _suppressNotification = true;

            foreach (T item in list)
            {
                //todo: RangeObservableCollection'dan türetildiği takdirde aşağıdaki Add(item); override edilen halini cagırıyorsa bu class RangeObservableCollection'dan türetilip metod burdan kalakacak
                Add(item);
            }
            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}

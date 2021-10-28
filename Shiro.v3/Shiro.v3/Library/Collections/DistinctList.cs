using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiro.Library
{
    public class DistinctList<T> : List<T>
    {
        public new void Add(T item)
        {
            if (item != null && this.All(x => !Equals(x, item)))
            {
                base.Add(item);
            }
        }
    }
}

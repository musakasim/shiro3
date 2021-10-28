﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Shiro.Library
{
    /// <summary>
    ///     Matches two lists even item orders  are different, or same item appears more than once
    ///     having same items is sufficent
    /// </summary>
    public class IgnoreOrderComparer : IEqualityComparer<IList<string>>
    {
        public IgnoreOrderComparer(StringComparer comparer)
        {
            Comparer = comparer;
        }

        public StringComparer Comparer { get; set; }

        public bool Equals(IList<string> x, IList<string> y)
        {
            if (x == null || y == null) return false;
            // remove the Distincts if there are never duplicates
            return !x.Distinct(Comparer).Except(y.Distinct(Comparer), Comparer).Any();
            // btw, this should work if the order matters:
            // return x.SequenceEqual(y, Comparer);
        }

        public int GetHashCode(IList<string> arr)
        {
            if (arr == null) return int.MinValue;
            int hash = 19;
            foreach (string s in arr.Distinct(Comparer))
            {
                hash = hash + s.GetHashCode();
            }
            return hash;
        }
    }
}
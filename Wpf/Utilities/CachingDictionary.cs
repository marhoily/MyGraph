using System;
using System.Collections.Generic;

namespace MyGraph
{
    public sealed class CachingDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        private readonly Func<TKey, TValue> _longCalc;

        public CachingDictionary(Func<TKey, TValue> longCalc)
        {
            _longCalc = longCalc;
        }

        public TValue Get(TKey key)
        {
            TValue value;
            if (!TryGetValue(key, out value))
                this[key] = value = _longCalc(key);
            return value;
        }
    }
}
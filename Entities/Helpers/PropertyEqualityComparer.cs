using System;
using System.Collections.Generic;

namespace Entities.Helpers
{
    public class PropertyEqualityComparer<TBase, TProperty> : IEqualityComparer<TBase>
    {
        private readonly Func<TBase, TProperty> _getter;

        public PropertyEqualityComparer(Func<TBase, TProperty> getter)
        {
            _getter = getter;
        }

        public bool Equals(TBase x, TBase y)
        {
            return EqualityComparer<TProperty>.Default.Equals(_getter(x), _getter(y));
        }

        public int GetHashCode(TBase obj)
        {
            return _getter(obj).GetHashCode();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication.Helpers
{
    public static class EnumerableExtensions
    {
        private static IEnumerable<TMapped> GetAddedItemsByUniqProperty<TSource, TMapped, TProperty>(
            this IEnumerable<TSource> source,
            IEnumerable<TMapped> modified,
            Func<TSource, TProperty> sourceGetter,
            Func<TMapped, TProperty> changedGetter
        )
        {
            if (modified == null || source == null || changedGetter == null || sourceGetter == null)
                throw new ArgumentNullException();

            var originPropertySet = source.Select(sourceGetter).ToHashSet();
            foreach (var changedItem in modified)
            {
                if (!originPropertySet.Contains(changedGetter(changedItem)))
                    yield return changedItem;
            }
        }

        private static IEnumerable<TSource> GetRemovedItemsByUniqProperty<TSource, TMapped, TProperty>(
            this IEnumerable<TSource> source,
            IEnumerable<TMapped> modified,
            Func<TSource, TProperty> originGetter,
            Func<TMapped, TProperty> changedGetter
        )
        {
            if (modified == null || source == null || changedGetter == null || originGetter == null)
                throw new ArgumentNullException();

            var changedPropertySet = modified.Select(changedGetter).ToHashSet();
            foreach (var originItem in source)
            {
                if (!changedPropertySet.Contains(originGetter(originItem)))
                    yield return originItem;
            }
        }

        public static (ICollection<TSource> added, ICollection<TSource> removed) CalculateUpdate<
            TSource, TMapped, TProperty>(
            this IEnumerable<TSource> source,
            IEnumerable<TMapped> modified,
            Func<TSource, TProperty> originGetter,
            Func<TMapped, TProperty> changedGetter,
            Func<TMapped, TSource> map)
        {
            var toAdd = source
                .GetAddedItemsByUniqProperty(modified, originGetter, changedGetter)
                .Select(map);
            var toRemove = source
                .GetRemovedItemsByUniqProperty(modified, originGetter, changedGetter);
            return (toAdd.ToList(), toRemove.ToList());
        }
        
    }
}
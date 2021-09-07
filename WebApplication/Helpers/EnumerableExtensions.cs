using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication.Helpers
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Removes all items from the collection
        /// </summary>
        public static void RemoveAll<TSource>(this ICollection<TSource> source, IEnumerable<TSource> removed)
        {
            foreach (var item in removed)
            {
                source.Remove(item);
            }
        }

        /// <summary>
        /// Adds the items into the collection using the mapping function
        /// </summary>
        public static void AddAll<TSource, TOther>(this ICollection<TSource> source, IEnumerable<TOther> added,
            Func<TOther, TSource> mapFunction)
        {
            foreach (var item in added)
            {
                source.Add(mapFunction(item));
            }
        }

        /// <summary>
        /// Calculate the difference in terms of added and removed entities from the collection
        /// </summary>
        /// <param name="source">data source, original collection from which changes are calculated</param>
        /// <param name="modified">modified source by adding or removing entities</param>
        /// <param name="map">function to map modified entity to original entity</param>
        /// <param name="equalityComparer">Custom equality comparer, e.g. comparer by entity id</param>
        /// <returns>Difference</returns>
        public static UpdateResult<TSource> CalculateUpdate<TSource, TMapped>(
            this ICollection<TSource> source,
            IEnumerable<TMapped> modified,
            Func<TMapped, TSource> map,
            IEqualityComparer<TSource> equalityComparer = null)
        {
            if (source == null || modified == null)
                return UpdateResult<TSource>.Empty;

            var modifiedMappedToSourceType = modified.Select(map).ToList();
            var added = modifiedMappedToSourceType.Except(source, equalityComparer);
            var removed = source.Except(modifiedMappedToSourceType, equalityComparer);
            return new UpdateResult<TSource> { Added = added, Removed = removed };
        }
    }
}
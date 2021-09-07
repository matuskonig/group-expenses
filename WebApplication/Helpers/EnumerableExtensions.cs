using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication.Helpers
{
    public static class EnumerableExtensions
    {
        public static void RemoveAll<TSource>(this ICollection<TSource> source, IEnumerable<TSource> removed)
        {
            foreach (var item in removed)
            {
                source.Remove(item);
            }
        }

        public static void AddAll<TSource>(this ICollection<TSource> source, IEnumerable<TSource> added,
            Func<TSource, TSource> mapFunction = null)
        {
            var map = mapFunction ?? (item => item);
            foreach (var item in added)
            {
                source.Add(map(item));
            }
        }

        public static UpdateResult<TSource> CalculateUpdate<TSource, TMapped>(
            this ICollection<TSource> source,
            IEnumerable<TMapped> modified,
            Func<TMapped, TSource> map,
            IEqualityComparer<TSource> equalityComparer)
        {
            if (source == null || modified == null)
                return UpdateResult<TSource>.Empty;

            var modifiedMapped = modified.Select(map).ToList();
            var added = modifiedMapped.Except(source, equalityComparer);
            var removed = source.Except(modifiedMapped, equalityComparer);
            return new UpdateResult<TSource> { Added = added, Removed = removed };
        }

        public record UpdateResult<TSource>
        {
            public static readonly UpdateResult<TSource> Empty = new();
            public IEnumerable<TSource> Added { get; init; } = Enumerable.Empty<TSource>();
            public IEnumerable<TSource> Removed { get; init; } = Enumerable.Empty<TSource>();
        }
    }
}
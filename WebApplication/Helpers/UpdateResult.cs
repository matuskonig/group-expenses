using System.Collections.Generic;
using System.Linq;

namespace WebApplication.Helpers
{
    /// <summary>
    /// Immutable record used to represent changed in IEnumerable, items added and removed from the enumerable 
    /// </summary>
    /// <typeparam name="TSource">Generic type of enumerable</typeparam>
    public record UpdateResult<TSource>
    {
        public static readonly UpdateResult<TSource> Empty = new();
        public IEnumerable<TSource> Added { get; init; } = Enumerable.Empty<TSource>();
        public IEnumerable<TSource> Removed { get; init; } = Enumerable.Empty<TSource>();
    }
}
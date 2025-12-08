using System.Collections.Generic;
using System.Linq;

public static class QueryableExtensions
{
    public static IQueryable<T> ToAsyncQueryable<T>(this IEnumerable<T> source)
    {
        return new TestAsyncEnumerable<T>(source);
    }
}

using System.Linq.Expressions;
using System;

public static class TestAsyncExtensions
{
    public static Task<T?> FirstOrDefaultAsync<T>(this IQueryable<T> source)
    {
        return Task.FromResult(source.FirstOrDefault());
    }

    public static Task<T?> FirstOrDefaultAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
    {
        return Task.FromResult(source.FirstOrDefault(predicate.Compile()));
    }

    public static Task<List<T>> ToListAsync<T>(this IQueryable<T> source)
    {
        return Task.FromResult(source.ToList());
    }

    public static Task<bool> AnyAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
    {
        return Task.FromResult(source.Any(predicate.Compile()));
    }
}

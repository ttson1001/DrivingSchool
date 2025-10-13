﻿namespace TutorDrive.Repositories
{
    public interface IRepository<T>
    {
        IQueryable<T> Get();

        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        Task AddAsync(T entity, CancellationToken cancellationToken = default);

        void Update(T entity);

        void DeleteRange(IEnumerable<T> entities);

        void Delete(T entity);

        void ClearChangeTracking();

        void BeginTransaction();

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<List<T>?> GetValuesAsync(CancellationToken cancellationToken = default);
    }
}

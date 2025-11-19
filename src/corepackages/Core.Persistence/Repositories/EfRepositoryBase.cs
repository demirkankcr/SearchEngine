using Core.Persistence.Paging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Core.Persistence.Repositories;

public class EfRepositoryBase<TEntity, TEntityId, TContext> : IAsyncRepository<TEntity, TEntityId>
    where TEntity : class
    where TContext : DbContext
{
    protected readonly TContext Context;

    public EfRepositoryBase(TContext context)
    {
        Context = context;
    }

    public IQueryable<TEntity> Query() => Context.Set<TEntity>();

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await Context.AddAsync(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities)
    {
        await Context.AddRangeAsync(entities);
        await Context.SaveChangesAsync();
        return entities;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false)
    {
        Context.Remove(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities, bool permanent = false)
    {
        Context.RemoveRange(entities);
        await Context.SaveChangesAsync();
        return entities;
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking) queryable = queryable.AsNoTracking();
        if (include != null) queryable = include(queryable);
        return await queryable.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IPaginate<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, int index = 0, int size = 10, bool withDeleted = false, bool enableTracking = true, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> queryable = Query();
        if (!enableTracking) queryable = queryable.AsNoTracking();
        if (include != null) queryable = include(queryable);
        if (predicate != null) queryable = queryable.Where(predicate);
        if (orderBy != null) return new Paginate<TEntity>(await orderBy(queryable).Skip(index * size).Take(size).ToListAsync(cancellationToken));
        return new Paginate<TEntity>(await queryable.Skip(index * size).Take(size).ToListAsync(cancellationToken));
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        Context.Update(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities)
    {
        Context.UpdateRange(entities);
        await Context.SaveChangesAsync();
        return entities;
    }
}

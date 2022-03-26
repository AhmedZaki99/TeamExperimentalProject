using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AspServerData
{

    public abstract class EntityStore<TEntity> : EntityStore<TEntity, int>, IEntityStore<TEntity> where TEntity : class, new()
    {
        protected EntityStore(ApplicationDbContext dbContext, DbSet<TEntity> dbSet) : base(dbContext, dbSet)
        { }
    }

    public abstract class EntityStore<TEntity, TKey> : IEntityStore<TEntity, TKey> where TEntity : class, new()
    {

        #region Protected Properties 

        protected ApplicationDbContext DbContext { get; }

        protected DbSet<TEntity> DbSet { get; }

        #endregion

        #region Constructor

        protected EntityStore(ApplicationDbContext dbContext, DbSet<TEntity> dbSet)
        {
            DbContext = dbContext;
            DbSet = dbSet;
        }

        #endregion


        #region CRUD Operations

        /// <summary>
        /// Get Entity by key, if exists.
        /// </summary>
        public async Task<TEntity?> FindAsync(TKey key)
        {
            return await DbSet.FindAsync(key);
        }

        /// <summary>
        /// Get Entity by key, including all relational data; if any exists.
        /// </summary>
        public async Task<TEntity?> FindAndNavigateAsync(TKey key, bool track = false)
        {
            IQueryable<TEntity> query = GetNavigationQuery();
            if (!track)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync(HasKey(key));
        }

        /// <summary>
        /// Returns a list of entities by page and count; default is 30 entities per page.
        /// </summary>
        public async Task<List<TEntity>> ListEntitiesAsync(int page = 1, int perPage = 30)
        {
            perPage = perPage > 100 ? 100 : perPage;

            return await GetOrderedQuery().Skip((page - 1) * perPage)
                                          .Take(perPage)
                                          .AsNoTracking()
                                          .ToListAsync();
        }


        /// <summary>
        /// Creates a new entity.
        /// </summary>
        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            DbSet.Add(entity);
            await DbContext.SaveChangesAsync();

            return entity;
        }

        /// <summary>
        /// Update entity data.
        /// </summary>
        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            DbContext.Entry(entity).State = EntityState.Modified;

            return await DbContext.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Delete entity by key.
        /// </summary>
        public virtual async Task<DeleteResult> DeleteAsync(TKey key)
        {
            var entity = await DbSet.FindAsync(key);
            if (entity is null)
            {
                return DeleteResult.EntityNotFound;
            }

            DbSet.Remove(entity);
            return await DbContext.SaveChangesAsync() > 0 ? DeleteResult.Success : DeleteResult.Failed;
        }

        #endregion

        #region Helper Methods

        public bool EntityExists(TKey key) => DbSet.Any(HasKey(key));

        #endregion


        #region Abstract Methods

        protected abstract Expression<Func<TEntity, bool>> HasKey(TKey key);

        protected abstract IOrderedQueryable<TEntity> GetOrderedQuery();
        protected abstract IQueryable<TEntity> GetNavigationQuery();

        #endregion

    }
}

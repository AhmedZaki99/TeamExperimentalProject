namespace AspServerData
{

    public interface IEntityStore<TEntity> : IEntityStore<TEntity, int> where TEntity : class, new()
    { }

    public interface IEntityStore<TEntity, TKey> where TEntity : class, new()
    {

        #region Read

        /// <summary>
        /// Get Entity by key, if exists.
        /// </summary>
        Task<TEntity?> FindAsync(TKey key);
        /// <summary>
        /// Get Entity by key, including all relational data; if any exists.
        /// </summary>
        Task<TEntity?> FindAndNavigateAsync(TKey key, bool track = false);

        /// <summary>
        /// Returns a list of entities by page and count; default is 30 entities per page.
        /// </summary>
        Task<List<TEntity>> ListEntitiesAsync(int page = 1, int perPage = 30);

        #endregion

        #region Create, Update, Delete, etc..

        /// <summary>
        /// Creates a new entity.
        /// </summary>
        Task<TEntity> CreateAsync(TEntity entity);
        /// <summary>
        /// Update entity data.
        /// </summary>
        Task<bool> UpdateAsync(TEntity entity);
        /// <summary>
        /// Delete entity by key.
        /// </summary>
        Task<DeleteResult> DeleteAsync(TKey key);

        #endregion

        #region Helpers
        
        /// <summary>
        /// Determines wheather the entity exists.
        /// </summary>
        bool EntityExists(TKey key);

        #endregion

    }
}

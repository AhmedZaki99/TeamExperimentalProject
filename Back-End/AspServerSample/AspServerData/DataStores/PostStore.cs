using Microsoft.EntityFrameworkCore;

namespace AspServerData
{
    public class PostStore : IPostStore
    {

        #region Dependencies

        private readonly ApplicationDbContext _dbContext;

        #endregion

        #region Constructor

        public PostStore(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Implementation

        #region Read

        /// <summary>
        /// Get Post by Id, if exists.
        /// </summary>
        public async Task<Post?> FindByIdAsync(int id)
        {
            return await _dbContext.Posts.FindAsync(id);
        }

        /// <summary>
        /// Get Post by id, including all relational data; if any exists.
        /// </summary>
        public async Task<Post?> FindAndNavigateByIdAsync(int id, bool track = false)
        {
            IQueryable<Post> query = _dbContext.Posts.Include(p => p.User)
                                                     .Include(p => p.Comments)
                                                        .ThenInclude(c => c.User)
                                                     .AsSplitQuery();
            if (!track)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync(p => p.PostId == id);
        }

        /// <summary>
        /// Returns a list of posts by page and count; default is 30 posts per page.
        /// </summary>
        public async Task<List<Post>> ListPostsAsync(int page = 1, int perPage = 30, bool includeUser = false)
        {
            perPage = perPage > 100 ? 100 : perPage;

            IQueryable<Post> query = includeUser ? _dbContext.Posts.Include(p => p.User) : _dbContext.Posts;

            return await query.OrderByDescending(p => p.DatePosted)
                              .Skip((page - 1) * perPage)
                              .Take(perPage)
                              .AsNoTracking()
                              .ToListAsync();
        }

        #endregion

        #region Create, Update, Delete, etc..

        /// <summary>
        /// Creates a new post.
        /// </summary>
        public async Task<Post> CreateAsync(Post post)
        {
            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();

            return post;
        }

        /// <summary>
        /// Update post data.
        /// </summary>
        public async Task<bool> UpdateAsync(Post post)
        {
            _dbContext.Entry(post).State = EntityState.Modified;
            post.LastEdited = DateTime.UtcNow;

            return await _dbContext.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Delete post by id.
        /// </summary>
        public async Task<DeleteResult> DeleteAsync(int id)
        {
            var post = await _dbContext.Posts.FindAsync(id);
            if (post is null)
            {
                return DeleteResult.EntityNotFound;
            }

            _dbContext.Posts.Remove(post);
            return await _dbContext.SaveChangesAsync() > 0 ? DeleteResult.Success : DeleteResult.Failed;
        }

        #endregion

        #region Helper Methods

        public bool PostExists(int id) => _dbContext.Posts.Any(p => p.PostId == id);

        #endregion

        #endregion

    }

}

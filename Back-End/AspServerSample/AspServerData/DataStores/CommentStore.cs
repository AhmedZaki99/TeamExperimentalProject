using Microsoft.EntityFrameworkCore;

namespace AspServerData
{
    public class CommentStore : ICommentStore
    {

        #region Dependencies

        private readonly ApplicationDbContext _dbContext;

        #endregion

        #region Constructor

        public CommentStore(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Implementation

        #region Read

        /// <summary>
        /// Get Comment by Id, if exists.
        /// </summary>
        public async Task<Comment?> FindByIdAsync(int id)
        {
            return await _dbContext.Comments.FindAsync(id);
        }

        /// <summary>
        /// Get Comment by id, including relational data; if any exists.
        /// </summary>
        public async Task<Comment?> FindAndNavigateByIdAsync(int id, bool track = false)
        {
            IQueryable<Comment> query = _dbContext.Comments.Include(c => c.User);
            if (!track)
            {
                query = query.AsNoTracking();
            }
            return await query.FirstOrDefaultAsync(p => p.PostId == id);
        }

        /// <summary>
        /// Returns a list of comments by page and count; default is 30 comments per page.
        /// </summary>
        public async Task<List<Comment>> ListCommentsAsync(int page = 1, int perPage = 30)
        {
            perPage = perPage > 100 ? 100 : perPage;

            return await _dbContext.Comments.OrderByDescending(c => c.DatePosted)
                                            .Skip((page - 1) * perPage)
                                            .Take(perPage)
                                            .ToListAsync();
        }

        #endregion

        #region Create, Update, Delete, etc..

        /// <summary>
        /// Creates a new comment.
        /// </summary>
        public async Task<Comment> CreateAsync(Comment comment)
        {
            _dbContext.Comments.Add(comment);
            await _dbContext.SaveChangesAsync();

            return comment;
        }

        /// <summary>
        /// Update comment data.
        /// </summary>
        public async Task<bool> UpdateAsync(Comment comment)
        {
            _dbContext.Entry(comment).State = EntityState.Modified;
            comment.LastEdited = DateTime.UtcNow;

            return await _dbContext.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Delete comment by id.
        /// </summary>
        public async Task<DeleteResult> DeleteAsync(int id)
        {
            var comment = await _dbContext.Comments.FindAsync(id);
            if (comment is null)
            {
                return DeleteResult.EntityNotFound;
            }

            _dbContext.Comments.Remove(comment);
            return await _dbContext.SaveChangesAsync() > 0 ? DeleteResult.Success : DeleteResult.Failed;
        }

        #endregion

        #endregion

    }

}

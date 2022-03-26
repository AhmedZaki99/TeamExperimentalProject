using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AspServerData
{
    public class CommentStore : EntityStore<Comment>, ICommentStore
    {

        #region Constructor

        public CommentStore(ApplicationDbContext dbContext) : base(dbContext, dbContext.Comments)
        { }

        #endregion


        #region Overriden Methods

        /// <summary>
        /// Update comment data.
        /// </summary>
        public override async Task<bool> UpdateAsync(Comment comment)
        {
            bool isSuccessful = await base.UpdateAsync(comment);
            if (isSuccessful)
            {
                comment.LastEdited = DateTime.UtcNow;
            }
            return isSuccessful;
        }

        #endregion

        #region Abstract Implementation

        protected override Expression<Func<Comment, bool>> HasKey(int key)
        {
            return c => c.CommentId == key;
        }

        protected override IOrderedQueryable<Comment> GetOrderedQuery()
        {
            return DbSet.OrderByDescending(c => c.DatePosted);
        }

        protected override IQueryable<Comment> GetNavigationQuery()
        {
            return DbSet.Include(c => c.User);
        }

        #endregion


    }

}

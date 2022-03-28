using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AspServerData
{
    public class PostStore : EntityStore<Post>, IPostStore
    {

        #region Constructor

        public PostStore(ApplicationDbContext dbContext) : base(dbContext, dbContext.Posts)
        { }

        #endregion


        #region Interface Implementation

        /// <summary>
        /// Returns a list of posts - including authors - by page and count; default is 30 posts per page.
        /// </summary>
        public async Task<List<Post>> ListPostsWithAuthorsAsync(int page = 1, int perPage = 30)
        {
            perPage = perPage > 100 ? 100 : perPage;

            return await GetOrderedQuery().Include(p => p.User)
                                          .Skip((page - 1) * perPage)
                                          .Take(perPage)
                                          .AsNoTracking()
                                          .ToListAsync();
        }

        #endregion

        #region Overriden Methods

        /// <summary>
        /// Update post data.
        /// </summary>
        public override async Task<bool> UpdateAsync(Post post)
        {
            bool isSuccessful = await base.UpdateAsync(post);
            if (isSuccessful)
            {
                post.LastEdited = DateTime.UtcNow;
            }
            return isSuccessful;
        }

        #endregion

        #region Abstract Implementation

        protected override Expression<Func<Post, bool>> HasKey(int key)
        {
            return p => p.PostId == key;
        }

        protected override IOrderedQueryable<Post> GetOrderedQuery()
        {
            return DbSet.OrderByDescending(p => p.DatePosted);
        }

        protected override IQueryable<Post> GetNavigationQuery()
        {
            return DbSet.Include(p => p.User)
                        .Include(p => p.Comments)
                           .ThenInclude(c => c.User)
                        .AsSplitQuery();
        }

        #endregion

    }

}

namespace AspServerData
{
    public interface IPostStore
    {
        #region Read

        /// <summary>
        /// Get Post by Id, if exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Post?> FindByIdAsync(int id);
        /// <summary>
        /// Get User by Id, including all relational data; if any exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Post?> FindAndNavigateByIdAsync(int id, bool track = false);

        /// <summary>
        /// Returns a list of posts by page and count; default is 30 posts per page.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <returns></returns>
        Task<List<Post>> ListPostsAsync(int page = 1, int perPage = 30, bool includeUser = false);

        #endregion

        #region Create, Update, Delete, etc..

        /// <summary>
        /// Creates a new post.
        /// </summary>
        Task<Post> CreateAsync(Post post);
        /// <summary>
        /// Update post data.
        /// </summary>
        Task<bool> UpdateAsync(Post post);
        /// <summary>
        /// Delete post by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DeleteResult> DeleteAsync(int id);

        #endregion

        #region Helper Methods

        public bool PostExists(int id);

        #endregion

    }
}

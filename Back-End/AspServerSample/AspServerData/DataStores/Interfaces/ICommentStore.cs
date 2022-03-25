namespace AspServerData
{
    public interface ICommentStore
    {
        #region Read

        /// <summary>
        /// Get Comment by Id, if exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Comment?> FindByIdAsync(int id);
        /// <summary>
        /// Get Comment by Id, including relational data; if any exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Comment?> FindAndNavigateByIdAsync(int id, bool track = false);

        /// <summary>
        /// Returns a list of comments by page and count; default is 30 comments per page.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <returns></returns>
        Task<List<Comment>> ListCommentsAsync(int page = 1, int perPage = 30);

        #endregion

        #region Create, Update, Delete, etc..

        /// <summary>
        /// Creates a new comment.
        /// </summary>
        Task<Comment> CreateAsync(Comment comment);
        /// <summary>
        /// Update comment data.
        /// </summary>
        Task<bool> UpdateAsync(Comment comment);
        /// <summary>
        /// Delete comment by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DeleteResult> DeleteAsync(int id);

        #endregion

    }
}

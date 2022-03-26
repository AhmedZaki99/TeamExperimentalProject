namespace AspServerData
{
    public interface IPostStore : IEntityStore<Post>
    {

        /// <summary>
        /// Returns a list of posts - including authors - by page and count; default is 30 posts per page.
        /// </summary>
        Task<List<Post>> ListPostsWithAuthorsAsync(int page = 1, int perPage = 30);

    }
}

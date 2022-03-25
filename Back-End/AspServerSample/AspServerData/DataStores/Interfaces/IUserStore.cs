namespace AspServerData
{
    public interface IUserStore
    {
        #region Read

        /// <summary>
        /// Get User by Id, if exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<User?> FindByIdAsync(int id);
        /// <summary>
        /// Get User by Id, including all relational data; if any exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<User?> FindAndNavigateByIdAsync(int id, bool track = false);
        /// <summary>
        /// Get User by User name, if exists.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<User?> FindByUserNameAsync(string username, bool track = false);
        /// <summary>
        /// Get User by User name, including all relational data; if any exists.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<User?> FindAndNavigateByUserNameAsync(string username, bool track = false);

        /// <summary>
        /// Returns a list of users by page and count; default is 30 users per page.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <returns></returns>
        Task<List<User>> ListUsersAsync(int page = 1, int perPage = 30);

        #endregion

        #region Create, Update, Delete, etc..

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<User> CreateAsync(User user, string? password);
        /// <summary>
        /// Update user data.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(User user, string? password);
        /// <summary>
        /// Delete user by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DeleteResult> DeleteAsync(int id);
        /// <summary>
        /// Delete user by user name.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<DeleteResult> DeleteAsync(string username);

        /// <summary>
        /// Login to a user account.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<LoginResult> LoginAsync(string username, string password);

        #endregion

        #region Helper Methods

        public bool UserExists(int id);

        public bool UserNameExists(string username);
        public bool UserEmailExists(string email);
        public bool UserNameOrEmailExists(string username, string email);

        #endregion

    }
}

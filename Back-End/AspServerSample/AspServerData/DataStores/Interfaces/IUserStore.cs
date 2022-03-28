namespace AspServerData
{
    public interface IUserStore : IEntityStore<User>
    {
        #region Read

        /// <summary>
        /// Get User by User name, if exists.
        /// </summary>
        Task<User?> FindByUserNameAsync(string username, bool track = false);
        /// <summary>
        /// Get User by User name, including all relational data; if any exists.
        /// </summary>
        Task<User?> FindAndNavigateByUserNameAsync(string username, bool track = false);

        #endregion

        #region Create, Update, Delete, etc..

        /// <summary>
        /// Creates a new user.
        /// </summary>
        Task<User> CreateAsync(User user, string? password);
        /// <summary>
        /// Update user data.
        /// </summary>
        Task<bool> UpdateAsync(User user, string? password);
        /// <summary>
        /// Delete user by user name.
        /// </summary>
        Task<DeleteResult> DeleteAsync(string username);

        /// <summary>
        /// Login to a user account.
        /// </summary>
        Task<LoginResult> LoginAsync(string username, string password);

        #endregion

        #region Helper Methods

        /// <summary>
        /// Determines wheather a user with the given username exists.
        /// </summary>
        public bool UserNameExists(string username);
        /// <summary>
        /// Determines wheather a user with the given email exists.
        /// </summary>
        public bool UserEmailExists(string email);
        /// <summary>
        /// Determines wheather a user with the given username or email exists.
        /// </summary>
        public bool UserNameOrEmailExists(string username, string email);

        #endregion

    }
}

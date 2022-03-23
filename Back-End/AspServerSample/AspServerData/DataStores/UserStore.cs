using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AspServerData
{
    public class UserStore : IUserStore
    {

        #region Dependencies

        private readonly ApplicationDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;

        #endregion

        #region Constructor

        public UserStore(ApplicationDbContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        #endregion

        #region Implementation

        #region Read

        /// <summary>
        /// Get User by Id, if exists.
        /// </summary>
        public async Task<User?> FindByIdAsync(int id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        /// <summary>
        /// Get User by User name, if exists.
        /// </summary>
        public async Task<User?> FindByUserNameAsync(string username, bool track = false)
        {
            if (track)
            {
                return await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
            }
            else return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == username);
        }

        /// <summary>
        /// Get User by User name, including all relational data; if any exists.
        /// </summary>
        public async Task<User?> FindAndNavigateByUserNameAsync(string username, bool track = false)
        {
            IQueryable<User> query = _dbContext.Users.Include(u => u.Posts)
                                                        .ThenInclude(p => p.Comments)
                                                            .ThenInclude(c => c.User)
                                                     .AsSplitQuery();
            if (track)
            {
                return await query.FirstOrDefaultAsync(u => u.UserName == username);
            }
            else return await query.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == username);
        }


        /// <summary>
        /// Returns a list of top 100 users.
        /// </summary>
        public Task<List<User>> ListUsersAsync() => ListUsersAsync(0, 100);

        /// <summary>
        /// Returns a list of users by page and count.
        /// </summary>
        public async Task<List<User>> ListUsersAsync(int page, int perPage)
        {
            perPage = perPage > 100 ? 100 : perPage;

            return await _dbContext.Users.OrderBy(u => u.UserId)
                                         .Skip((page - 1) * perPage)
                                         .Take(perPage)
                                         .AsNoTracking()
                                         .ToListAsync();
        }

        #endregion

        #region Create, Update, Delete, etc..

        /// <summary>
        /// Creates a new user.
        /// </summary>
        public async Task<User> CreateAsync(User user, string? password)
        {
            user.PasswordHash = password is not null ? _passwordHasher.HashPassword(user, password) : null;

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Update user data.
        /// </summary>
        public async Task<bool> UpdateAsync(User user, string? password)
        {
            _dbContext.Entry(user).State = EntityState.Modified;
            user.PasswordHash = password is not null ? _passwordHasher.HashPassword(user, password) : user.PasswordHash;

            return await _dbContext.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Delete user by id.
        /// </summary>
        public async Task<DeleteResult> DeleteAsync(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user is null)
            {
                return DeleteResult.UserNotFound;
            }

            _dbContext.Users.Remove(user);
            return await _dbContext.SaveChangesAsync() > 0 ? DeleteResult.Success : DeleteResult.Failed;
        }

        /// <summary>
        /// Delete user by user name.
        /// </summary>
        public async Task<DeleteResult> DeleteAsync(string username)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user is null)
            {
                return DeleteResult.UserNotFound;
            }

            _dbContext.Users.Remove(user);
            return await _dbContext.SaveChangesAsync() > 0 ? DeleteResult.Success : DeleteResult.Failed;
        }


        /// <summary>
        /// Login to a user account.
        /// </summary>
        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user is null)
            {
                return LoginResult.UserNotFound;
            }

            var verificationResult = user.PasswordHash is null ?
                                        PasswordVerificationResult.Failed :
                                        _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return LoginResult.Failed;
            }

            user.LastSignedIn = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return LoginResult.Success;
        }

        #endregion

        #region Helper Methods

        public bool UserExists(int id) => _dbContext.Users.Any(u => u.UserId == id);

        public bool UserNameExists(string username) => _dbContext.Users.Any(u => u.UserName == username);
        public bool UserEmailExists(string email) => _dbContext.Users.Any(u => u.Email == email);
        public bool UserNameOrEmailExists(string username, string email) => _dbContext.Users.Any(u => u.UserName == username || u.Email == email);

        #endregion

        #endregion

    }

}

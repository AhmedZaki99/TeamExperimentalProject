using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AspServerData
{
    public class UserStore : EntityStore<User>, IUserStore
    {

        #region Dependencies

        private readonly IPasswordHasher<User> _passwordHasher;

        #endregion

        #region Constructor

        public UserStore(ApplicationDbContext dbContext, IPasswordHasher<User> passwordHasher) : base(dbContext, dbContext.Users)
        {
            _passwordHasher = passwordHasher;
        }

        #endregion

        #region Interface Implementation

        #region Read

        /// <summary>
        /// Get User by User name, if exists.
        /// </summary>
        public async Task<User?> FindByUserNameAsync(string username, bool track = false)
        {
            IQueryable<User> query = !track ? DbSet.AsNoTracking() : DbSet;

            username = username.ToUpper();
            return await query.FirstOrDefaultAsync(u => u.NormalizedUserName == username);
        }

        /// <summary>
        /// Get User by User name, including all relational data; if any exists.
        /// </summary>
        public async Task<User?> FindAndNavigateByUserNameAsync(string username, bool track = false)
        {
            IQueryable<User> query = GetNavigationQuery();
            if (!track)
            {
                query = query.AsNoTracking();
            }

            username = username.ToUpper();
            return await query.FirstOrDefaultAsync(u => u.NormalizedUserName == username);
        }

        #endregion

        #region Create, Update, Delete, etc..

        /// <summary>
        /// Creates a new user.
        /// </summary>
        public Task<User> CreateAsync(User user, string? password)
        {
            user.PasswordHash = password is not null ? _passwordHasher.HashPassword(user, password) : null;

            return base.CreateAsync(user);
        }

        /// <summary>
        /// Update user data.
        /// </summary>
        public Task<bool> UpdateAsync(User user, string? password)
        {
            user.PasswordHash = password is not null ? _passwordHasher.HashPassword(user, password) : user.PasswordHash;

            return base.UpdateAsync(user);
        }

        /// <summary>
        /// Delete user by user name.
        /// </summary>
        public async Task<DeleteResult> DeleteAsync(string username)
        {
            username = username.ToUpper();
            var user = await DbSet.FirstOrDefaultAsync(u => u.NormalizedUserName == username);
            if (user is null)
            {
                return DeleteResult.EntityNotFound;
            }

            DbSet.Remove(user);
            return await DbContext.SaveChangesAsync() > 0 ? DeleteResult.Success : DeleteResult.Failed;
        }


        /// <summary>
        /// Login to a user account.
        /// </summary>
        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            username = username.ToUpper();
            var user = await DbSet.FirstOrDefaultAsync(u => u.NormalizedUserName == username);
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
            await DbContext.SaveChangesAsync();

            return LoginResult.Success;
        }

        #endregion

        #region Helper Methods

        public bool UserNameExists(string username)
        {
            username = username.ToUpper();
            return DbSet.Any(u => u.NormalizedUserName == username);
        }
        public bool UserEmailExists(string email)
        {
            email = email.ToUpper();
            return DbSet.Any(u => u.NormalizedEmail == email);
        }
        public bool UserNameOrEmailExists(string username, string email)
        {
            username = username.ToUpper();
            email = email.ToUpper();
            return DbSet.Any(u => u.NormalizedUserName == username || u.NormalizedEmail == email);
        }

        #endregion

        #endregion


        #region Abstract Implementation

        protected override Expression<Func<User, bool>> HasKey(int key)
        {
            return u => u.UserId == key;
        }

        protected override IOrderedQueryable<User> GetOrderedQuery()
        {
            return DbSet.OrderBy(u => u.UserId);
        }

        protected override IQueryable<User> GetNavigationQuery()
        {
            return DbSet.Include(u => u.Posts)
                           .ThenInclude(p => p.Comments)
                               .ThenInclude(c => c.User)
                        .AsSplitQuery();
        }

        #endregion

    }

}

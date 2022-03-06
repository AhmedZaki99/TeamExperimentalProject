using ASPNetCoreData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASPNet6.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        
        #region Dependencies

        private readonly ApplicationDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;

        #endregion

        #region Constructor

        public UsersController(ApplicationDbContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }

        #endregion


        #region Controller Actions

        /// <summary>
        /// List users per page.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> ListUsersAsync([FromQuery] int page = 1, [FromQuery] int per_page = 30)
        {
            per_page = per_page > 100 ? 100 : per_page;

            return await _dbContext.Users.OrderBy(u => u.UserId)
                                         .Skip((page - 1) * per_page)
                                         .Take(per_page)
                                         .Select(u => PartiallyDetailedUser(u))
                                         .ToListAsync();
        }

        /// <summary>
        /// Get user by username.
        /// </summary>
        [HttpGet("{username}")]
        public async Task<ActionResult<object>> GetUserAsync([FromRoute] string username)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return NotFound();
            }
            return PartiallyDetailedUser(user);
        }


        /// <summary>
        /// Create new user, accepting form data.
        /// </summary>
        [HttpPost("register")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<ActionResult<object>> CreateUserAsync([FromForm] UserDTO userDTO)
        {
            if (userDTO.UserName == null || userDTO.Email == null || userDTO.BirthDate == default)
            {
                return BadRequest();
            }

            var user = userDTO.CreateUserFromDTO();

            user.PasswordHash = userDTO.Password != null ? _passwordHasher.HashPassword(user, userDTO.Password) : null;

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserAsync), new { username = user.UserName }, FullyDetailedUser(user));
        }

        /// <summary>
        /// Log in to user account, accepting form data. 
        /// </summary>
        [HttpPost("login")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<ActionResult<object>> LoginUserAsync([FromForm] UserDTO userDTO)
        {
            if (userDTO.UserName == null || userDTO.Password == null)
            {
                return BadRequest();
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userDTO.UserName);
            if (user == null)
            {
                return NotFound();
            }

            var verificationResult = user.PasswordHash == null ? 
                                        PasswordVerificationResult.Failed : 
                                        _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userDTO.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return new
                {
                    status = "Login attempt failed.",
                    error_message = "Incorrect Password."
                };
            }

            user.LastSignedIn = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return new
            {
                status = "User logged in successfully.",
                data = FullyDetailedUser(user)
            };
        }


        /// <summary>
        /// Update user by username, accepting form data.
        /// </summary>
        [HttpPost("edit")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<ActionResult<object>> UpdateUserAsync([FromForm] UserDTO userDTO)
        {
            if (userDTO.UserName == null)
            {
                return BadRequest();
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userDTO.UserName);
            if (user == null)
            {
                return NotFound();
            }

            userDTO.UpdateUserWithDTO(user);
            user.PasswordHash = userDTO.Password != null ? _passwordHasher.HashPassword(user, userDTO.Password) : user.PasswordHash;

            await _dbContext.SaveChangesAsync();

            return PartiallyDetailedUser(user);
        }

        /// <summary>
        /// Delete user by username, accepting form data.
        /// </summary>
        [HttpPost("delete")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<ActionResult<object>> DeleteUserAsync([FromForm] UserDTO userDTO)
        {
            if (userDTO.UserName == null || userDTO.Password == null)
            {
                return BadRequest();
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userDTO.UserName);
            if (user == null)
            {
                return NotFound();
            }

            var verificationResult = user.PasswordHash == null ?
                                        PasswordVerificationResult.Failed :
                                        _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userDTO.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return new
                {
                    status = "Delete attempt failed.",
                    error_message = "Incorrect Password."
                };
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return new { DeleteResult = "Successful", user.UserId, user.UserName };
        }

        #endregion

        #region Helper Methods

        private static object FullyDetailedUser(User user) => new
        {
            user.UserId,
            user.UserName,
            user.Email,
            user.FirstName,
            user.LastName,
            user.BirthDate,
            user.LastSignedIn,
            user.DateCreated
        };

        private static object PartiallyDetailedUser(User user) => new
        {
            user.UserId,
            user.UserName,
            user.Email,
            user.FirstName,
            user.LastName,
            user.BirthDate
        };

        #endregion

    }
}

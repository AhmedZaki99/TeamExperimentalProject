using ASPNetCoreData;
using DataProcessingAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataProcessingAPI.Controllers
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
        public async Task<ActionResult<IEnumerable<UserOutputDTO>>> ListUsersAsync([FromQuery] int page = 1, [FromQuery] int per_page = 30)
        {
            per_page = per_page > 100 ? 100 : per_page;

            return await _dbContext.Users.OrderBy(u => u.UserId)
                                         .Skip((page - 1) * per_page)
                                         .Take(per_page)
                                         .Select(u => UserOutputDTO.Create(u))
                                         .AsNoTracking()
                                         .ToListAsync();
        }

        /// <summary>
        /// Get user by username.
        /// </summary>
        [HttpGet("{username}")]
        public async Task<ActionResult<UserDetailedOutputDTO>> GetUserAsync([FromRoute] string username)
        {
            var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return NotFound();
            }
            return UserDetailedOutputDTO.Create(user);
        }


        /// <summary>
        /// Create new user, accepting form data.
        /// </summary>
        [HttpPost("register")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<ActionResult<UserDetailedOutputDTO>> CreateUserAsync([FromForm] UserCreateInputDTO userDTO)
        {
            if (_dbContext.UserNameExists(userDTO.UserName!))
            {
                ModelState.AddModelError(nameof(userDTO.UserName), "Username already exists, make sure you provided a unique username.");
            }
            if (_dbContext.UserEmailExists(userDTO.Email!))
            {
                ModelState.AddModelError(nameof(userDTO.Email), "Email already exists, make sure you provided a unique email.");
            }
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var user = userDTO.Map();
            user.PasswordHash = userDTO.Password != null ? _passwordHasher.HashPassword(user, userDTO.Password) : null;

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserAsync), new { username = user.UserName }, UserDetailedOutputDTO.Create(user));
        }

        /// <summary>
        /// Log in to user account, accepting form data. 
        /// </summary>
        [HttpPost("login")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> LoginUserAsync([FromForm] UserLoginInputDTO userDTO)
        {
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
                ModelState.AddModelError(nameof(userDTO.Password), "Incorrect Password.");
                return ValidationProblem(title: "Login attempt failed.", modelStateDictionary: ModelState);
            }

            user.LastSignedIn = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                status = 200,
                title = "User logged in successfully.",
                data = UserDetailedOutputDTO.Create(user)
            });
        }


        /// <summary>
        /// Update user by username, accepting form data.
        /// </summary>
        [HttpPost("edit")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<ActionResult<UserOutputDTO>> UpdateUserAsync([FromForm] UserUpdateInputDTO userDTO)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userDTO.UserName);
            if (user == null)
            {
                return NotFound();
            }
            if (userDTO.UserId != user.UserId)
            {
                ModelState.AddModelError(nameof(userDTO.UserId), "User Id provided doesn't match with the username in route.");
            }
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            _dbContext.Entry(user).CurrentValues.SetValues(userDTO);
            user.PasswordHash = userDTO.Password != null ? _passwordHasher.HashPassword(user, userDTO.Password) : user.PasswordHash;

            await _dbContext.SaveChangesAsync();

            return UserOutputDTO.Create(user);
        }

        /// <summary>
        /// Delete user by username, accepting form data.
        /// </summary>
        [HttpPost("delete")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> DeleteUserAsync([FromForm] UserLoginInputDTO userDTO)
        {
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
                ModelState.AddModelError(nameof(userDTO.Password), "Incorrect Password.");
                return ValidationProblem(title: "Delete attempt failed.", modelStateDictionary: ModelState);
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                status = "User deleted successfully.",
                user.UserId,
                user.UserName
            });
        }

        #endregion

    }
}

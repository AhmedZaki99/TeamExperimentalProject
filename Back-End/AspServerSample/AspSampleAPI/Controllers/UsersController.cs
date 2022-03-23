using AspSampleAPI.Models;
using AspServerData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspSampleAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        
        #region Dependencies

        private readonly IUserStore _userStore;
        private readonly ILogger<UsersController> _logger;

        #endregion

        #region Constructor

        public UsersController(IUserStore userStore, ILogger<UsersController> logger)
        {
            _userStore = userStore;
            _logger = logger;
        }

        #endregion


        #region Controller Actions

        /// <summary>
        /// List users per page.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserOutputDTO>>> ListUsersAsync([FromQuery] int page = 1, [FromQuery] int per_page = 30)
        {
            var users = await _userStore.ListUsersAsync(page, per_page);
            return users.Select(u => UserOutputDTO.Create(u)).ToList();
        }

        /// <summary>
        /// Get user by username.
        /// </summary>
        [HttpGet("{username}")]
        public async Task<ActionResult<PostsUserOutputDTO>> GetUserAsync([FromRoute] string username)
        {
            //_logger.LogInformation("[{Time:HH:mm:ss.ffffff}] Fetching data for user {User}...", DateTime.Now, username);

            var user = await _userStore.FindAndNavigateByUserNameAsync(username);

            //_logger.LogInformation("[{Time:HH:mm:ss.ffffff}] End of data processing for user {User}.", DateTime.Now, username);

            if (user is null)
            {
                return NotFound();
            }
            return PostsUserOutputDTO.Create(user);
        }


        /// <summary>
        /// Create new user. 
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UserOutputDTO>> CreateUserAsync([FromBody] UserCreateInputDTO userDTO)
        {
            if (_userStore.UserNameExists(userDTO.UserName!))
            {
                ModelState.AddModelError(nameof(userDTO.UserName), "Username already exists, make sure you provided a unique username.");
            }
            if (_userStore.UserEmailExists(userDTO.Email!))
            {
                ModelState.AddModelError(nameof(userDTO.Email), "Email already exists, make sure you provided a unique email.");
            }
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var user = await _userStore.CreateAsync(userDTO.Map(), userDTO.Password);
            
            return CreatedAtAction(nameof(GetUserAsync), new { username = user.UserName }, UserOutputDTO.Create(user));
        }

        /// <summary>
        /// Log in to user account. 
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> LoginUserAsync([FromBody] UserLoginInputDTO userDTO)
        {
            var loginResult = await _userStore.LoginAsync(userDTO.UserName!, userDTO.Password!);

            if (loginResult == LoginResult.UserNotFound)
            {
                return NotFound();
            }
            else if (loginResult == LoginResult.Failed)
            {
                ModelState.AddModelError(nameof(userDTO.Password), "Incorrect Password.");
                return ValidationProblem(title: "Login attempt failed.", modelStateDictionary: ModelState);
            }

            var user = await _userStore.FindByUserNameAsync(userDTO.UserName!);
            return Ok(new
            {
                status = 200,
                title = "User logged in successfully.",
                data = UserOutputDTO.Create(user!)
            });
        }


        /// <summary>
        /// Update user by username.
        /// </summary>
        [HttpPut("{username}")]
        public async Task<ActionResult<UserOutputDTO>> UpdateUserAsync([FromRoute] string username, [FromBody] UserUpdateInputDTO userDTO)
        {
            var user = await _userStore.FindByUserNameAsync(username);
            if (user is null)
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

            await _userStore.UpdateAsync(userDTO.Update(user), userDTO.Password);

            return UserOutputDTO.Create(user);
        }

        /// <summary>
        /// Delete user by username.
        /// </summary>
        [HttpDelete("{username}")]
        public async Task<IActionResult> DeleteUserAsync(string username)
        {
            var deleteResult = await _userStore.DeleteAsync(username);

            if (deleteResult == DeleteResult.UserNotFound)
            {
                return NotFound();
            }
            else if (deleteResult == DeleteResult.Failed)
            {
                return Problem(statusCode: 500, title: "Server error.", detail: "Failed to delete the user.");
            }

            var user = await _userStore.FindByUserNameAsync(username);
            return Ok(new
            {
                status = "User deleted successfully.",
                user!.UserId,
                user!.UserName
            });
        }

        #endregion

    }
}

using DataProcessingContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DataProcessingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {

        #region Dependencies

        private readonly UserService _userService;
        private readonly UserManager<AppUser> _userManager;

        #endregion

        #region Constructor

        public UsersController(UserService userService, UserManager<AppUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        #endregion


        #region Controller Actions

        [HttpPost("signup")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> SignUpAsync([FromForm] UserSignUpInputDto userDTO)
        {
            var result = await _userService.SignUpAsync(userDTO);
            if (result.IsSuccessful)
            {
                return Ok($"Account {_userManager.GetUserName(HttpContext?.User)} have been created successfully.");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
            return ValidationProblem(ModelState);
        }

        [HttpPost("signin")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> SignInAsync([FromForm] UserSignInInputDto userDTO)
        {
            var result = await _userService.SignInAsync(userDTO);
            if (result == SignInResult.Success)
            {
                return Ok($"Account {_userManager.GetUserName(HttpContext?.User)} have been logged in successfully.");
            }
            if (result == SignInResult.UserLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account locked out.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid signin attempt.");
            }
            return ValidationProblem(ModelState);
        }

        [Authorize]
        [HttpPost("signout")]
        public async Task<IActionResult> SignOutAsync()
        {
            await _userService.SignOutAsync(HttpContext?.User);
            return NoContent();
        }

        #endregion

    }
}

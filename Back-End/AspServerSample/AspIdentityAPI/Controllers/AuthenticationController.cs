using AspIdentityAPI.Models;
using AspIdentityData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspIdentityAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {

        #region Dependencies

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AuthenticationController> _logger;

        #endregion

        #region Constructor

        public AuthenticationController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AuthenticationController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        #endregion


        #region Controller Actions

        [HttpPost("signup")]
        public async Task<IActionResult> SignUpAsync([FromBody] UserSignUpInputDto userDTO)
        {
            AppUser user = userDTO.Map();

            var result = await _userManager.CreateAsync(user, userDTO.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User account named '{username}' created in the database.", user.UserName);

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("private");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return ValidationProblem(title: "Failed to sign up.", modelStateDictionary: ModelState);
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignInAsync([FromBody] UserSignInInputDto userDTO)
        {
            // Clear the existing external cookie to ensure a clean sign in process
            await _signInManager.SignOutAsync();

            var result = await _signInManager.PasswordSignInAsync(userDTO.UserName, userDTO.Password, userDTO.RememberMe ?? false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User account '{username}' signed in.", userDTO.UserName);
                return RedirectToAction("private");
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account '{username}' locked out.", userDTO.UserName);
                ModelState.AddModelError(string.Empty, "Account locked out.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid signin attempt.");
            }
            return ValidationProblem(title: "Failed to sign in.", modelStateDictionary: ModelState);
        }

        [Authorize]
        [HttpPost("signout")]
        public async Task<IActionResult> SignOutAsync()
        {
            await _signInManager.SignOutAsync();

            _logger.LogInformation("User account '{username}' signed out.", _userManager.GetUserName(HttpContext?.User));
            return NoContent();
        }


        // Getting user info for debugging purposes.
        [Authorize]
        [HttpGet("private")]
        public async Task<ActionResult<AppUser>> PrivateAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            
            return user;
        }

        #endregion

    }
}

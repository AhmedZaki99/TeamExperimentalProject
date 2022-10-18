using DataProcessingCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DataProcessingServer.Controllers
{
    public class AccountController : Controller
    {

        #region Properties

        public string? ReturnUrl { get; set; }

        #endregion


        #region Dependencies

        private readonly UserService _userService;

        #endregion

        #region Constructor

        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        #endregion


        #region Actions

        [HttpGet]
        [Authorize]
        public IActionResult Manage()
        {
            return LocalRedirect(UrlSelector.AccountPage);
        }
        
        [HttpGet("UserData")]
        [Authorize]
        public async Task<ActionResult<UserOutputDto>> GetInfoAsync()
        {
            return await _userService.GetUserDataAsync(HttpContext?.User);
        }


        [HttpGet]
        public IActionResult Register([FromQuery] string? returnUrl = null)
        {
            returnUrl ??= Url.Content(UrlSelector.AccountPage);
            ReturnUrl = returnUrl;

            return LocalRedirect($"{UrlSelector.RegisterPage}?{nameof(returnUrl)}={returnUrl}");
        }
        
        [HttpPost]
        public async Task<IActionResult> RegisterAsync([FromForm] UserSignUpInputDto userDto, [FromQuery] string? returnUrl = null)
        {
            returnUrl ??= Url.Content(UrlSelector.AccountPage);

            if (ModelState.IsValid)
            {
                var result = await _userService.SignUpAsync(userDto);
                if (result.IsSuccessful)
                {
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                } 
            }
            return ValidationProblem(ModelState);
        }


        [HttpGet]
        public async Task<IActionResult> LoginAsync([FromQuery] string? returnUrl = null)
        {
            returnUrl ??= Url.Content(UrlSelector.AccountPage);

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl;

            return LocalRedirect($"{UrlSelector.LoginPage}?{nameof(returnUrl)}={returnUrl}");
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromForm] UserSignInInputDto userDto, [FromQuery] string? returnUrl = null)
        {
            returnUrl ??= Url.Content(UrlSelector.AccountPage);

            if (ModelState.IsValid)
            {
                var result = await _userService.SignInAsync(userDto);
                if (result == LogInResult.Success)
                {
                    return LocalRedirect(returnUrl);
                }
                if (result == LogInResult.UserLockedOut)
                {
                    return LocalRedirect(UrlSelector.LockoutPage);
                }

                ModelState.AddModelError(string.Empty, "Invalid signin attempt.");
            }
            return ValidationProblem(ModelState);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> LogoutAsync([FromQuery] string? returnUrl = null)
        {
            returnUrl ??= Url.Content(UrlSelector.IndexPage);

            await _userService.SignOutAsync(HttpContext?.User);

            return LocalRedirect(returnUrl);
        }

        #endregion

    }
}
using AspIdentityAPI.Models;
using AspIdentityData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace AspIdentityAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {

        #region Dependencies

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        #endregion

        #region Constructor

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        #endregion


        #region Controller Actions

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordInputDto changePasswordDTO)
        {
            AppUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation("Password has been changed successfully for user account '{username}'.", user.UserName);

                await _signInManager.RefreshSignInAsync(user);
                return Ok(new
                {
                    status = 200,
                    title = "OK.",
                    details = "Password has been changed successfully."
                });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return ValidationProblem(title: "Failed to change password.", modelStateDictionary: ModelState);
        }


        [HttpPost("change-email")]
        public async Task<IActionResult> ChangeEmailAsync([FromBody] ChangeEmailInputDto changeEmailDTO)
        {
            AppUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (changeEmailDTO.NewEmail == user.Email)
            {
                return Ok(new
                {
                    status = 200,
                    title = "OK.",
                    details = "Email provided is unchanged."
                });
            }

            var emailToken = await _userManager.GenerateChangeEmailTokenAsync(user, changeEmailDTO.NewEmail);

            string? callback = Url.Action("ConfirmEmailChange",
                new
                {
                    userId = user.Id,
                    email = changeEmailDTO.NewEmail,
                    token = emailToken
                });
            if (callback is null)
            {
                throw new InvalidOperationException("Action is not found.");
            }
            
            // TODO: send email verification callback to new email address.
            return Ok(new
            {
                status = 200,
                title = "OK.",
                details = "Confirmation link to change email is provided below.",
                confirmationLink = callback
            });
        }

        [AllowAnonymous]
        [HttpGet("confirm-email-change")]
        public async Task<IActionResult> ConfirmEmailChangeAsync([FromQuery] string? userId, [FromQuery] string? email, [FromQuery] string? token)
        {
            if (userId is null || email is null || token is null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var result = await _userManager.ChangeEmailAsync(user, email, token);
            if (result.Succeeded)
            {
                _logger.LogInformation("Email has been changed successfully for user account '{username}'.", user.UserName);

                await _signInManager.RefreshSignInAsync(user);
                return Ok(new
                {
                    status = 200,
                    title = "OK.",
                    details = "Email has been changed successfully."
                });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return ValidationProblem(title: "Failed to change email.", modelStateDictionary: ModelState);
        }


        #endregion


    }
}

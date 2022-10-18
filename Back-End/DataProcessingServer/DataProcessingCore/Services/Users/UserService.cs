using AutoMapper;
using DataProcessingContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace DataProcessingCore
{
    public class UserService
    {

        #region Dependencies

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;

        #endregion

        #region Constructors

        public UserService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<UserService> logger, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
        }

        #endregion


        #region Authentication

        public async Task<OperationResult<UserOutputDto>> SignUpAsync(UserSignUpInputDto userDto)
        {
            var user = _mapper.Map<AppUser>(userDto);

            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User account named '{username}' created in the database.", user.UserName);

                await _signInManager.SignInAsync(user, isPersistent: false);
                return new(_mapper.Map<UserOutputDto>(user));
            }

            Dictionary<string, string> errors = new(result.Errors.Select(error => new KeyValuePair<string, string>(error.Code, error.Description)));

            return new(errors, OperationError.ExternalError);
        }

        public async Task<SignInResult> SignInAsync(UserSignInInputDto userDto)
        {
            // Clear the existing external cookie to ensure a clean sign in process
            await _signInManager.SignOutAsync();

            var result = await _signInManager.PasswordSignInAsync(userDto.UserName, userDto.Password, userDto.RememberMe ?? false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User account '{username}' signed in.", userDto.UserName);
                return SignInResult.Success;
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account '{username}' locked out.", userDto.UserName);
                return SignInResult.UserLockedOut;
            }
            return SignInResult.Failed;
        }

        public async Task SignOutAsync(ClaimsPrincipal? user)
        {
            await _signInManager.SignOutAsync();

            if (user is not null)
            {
                _logger.LogInformation("User account '{username}' signed out.", _userManager.GetUserName(user)); 
            }
        }

        #endregion


    }
}

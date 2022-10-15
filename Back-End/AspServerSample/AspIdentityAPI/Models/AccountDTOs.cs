using System.ComponentModel.DataAnnotations;

namespace AspIdentityAPI.Models
{

    #region Change Password DTO

    public class ChangePasswordInputDto
    {
        #region Public Properties

        [Required(ErrorMessage = "Old Password is required and can't be empty.")]
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = "New Password is required and can't be empty.")]
        public string? NewPassword { get; set; }

        #endregion
    }

    #endregion


    #region Change Email DTO

    public class ChangeEmailInputDto
    {
        #region Public Properties

        [EmailAddress]
        [Required(ErrorMessage = "New Email is required and can't be empty.")]
        public string? NewEmail { get; set; }

        #endregion
    }

    #endregion

}

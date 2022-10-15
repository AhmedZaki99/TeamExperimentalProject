using AspIdentityData;
using System.ComponentModel.DataAnnotations;

namespace AspIdentityAPI.Models
{

    #region Output DTO

    public class UserOutputDto
    {
        #region Public Properties

        public int? UserId { get; set; }

        public string? UserName { get; set; }
        public string? Email { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        #endregion
    }

    #endregion


    #region Signup Input DTO

    public class UserSignUpInputDto
    {
        #region Public Properties

        [Required(ErrorMessage = "Username is required and can't be empty.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is required and can't be empty.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Email is required and can't be empty.")]
        [EmailAddress]
        public string? Email { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }


        [Required(ErrorMessage = "Birthdate is required and can't be empty.")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        #endregion

        #region Helper Methods

        public AppUser Map() => new()
        {
            UserName = UserName,
            Email = Email,
            FirstName = FirstName,
            LastName = LastName,
            BirthDate = BirthDate ?? throw new InvalidOperationException("User birth date must be provided."),
            LastSignedIn = null,
            DateCreated = DateTime.UtcNow
        };

        #endregion
    }

    #endregion


    #region Signin Input DTO

    public class UserSignInInputDto
    {
        #region Public Properties

        [Required(ErrorMessage = "Username is required and can't be empty.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "User Password is required and can't be empty.")]
        public string? Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool? RememberMe { get; set; }

        #endregion
    }

    #endregion

}

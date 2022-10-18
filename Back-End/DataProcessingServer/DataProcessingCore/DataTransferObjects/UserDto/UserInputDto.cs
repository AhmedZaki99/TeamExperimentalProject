using System.ComponentModel.DataAnnotations;

namespace DataProcessingCore
{

    public class UserSignUpInputDto
    {
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

    }


    public class UserSignInInputDto
    {
        [Required(ErrorMessage = "Username is required and can't be empty.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "User Password is required and can't be empty.")]
        public string? Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool? RememberMe { get; set; }
    }

}

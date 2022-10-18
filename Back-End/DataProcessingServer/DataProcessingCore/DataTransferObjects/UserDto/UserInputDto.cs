using System.ComponentModel.DataAnnotations;

namespace DataProcessingCore
{

    public class UserSignUpInputDto
    {

        [Required(ErrorMessage = "Username is required and can't be empty.")]
        [Display(Name = "User Name")]
        public string? UserName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required and can't be empty.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }


        [Required(ErrorMessage = "Password is required and can't be empty.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string? ConfirmPassword { get; set; }


        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }


        [Required(ErrorMessage = "Birthdate is required and can't be empty.")]
        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }

    }


    public class UserSignInInputDto
    {
        [Required(ErrorMessage = "Please provide Username or Email.")]
        [Display(Name = "User Name")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "User Password is required and can't be empty.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool? RememberMe { get; set; }
    }

}

using AspServerData;
using System.ComponentModel.DataAnnotations;

namespace AspSampleAPI.Models
{
    public class UserCreateInputDTO
    {

        #region Public Properties

        [Required(ErrorMessage = "Username is required and can't be empty.")]
        public string? UserName { get; set; }
        public string? Password { get; set; }

        [Required(ErrorMessage = "Email is required and can't be empty.")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }


        [Required(ErrorMessage = "Birthdate is required and can't be empty.")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        #endregion

        #region Helper Methods

        public User Map() => new()
        {
            UserName = UserName,
            Email = Email,
            EmailConfirmed = false,
            FirstName = FirstName,
            LastName = LastName,
            BirthDate = BirthDate ?? throw new InvalidOperationException("User birth date must be provided."),
            LastSignedIn = null,
            DateCreated = DateTime.UtcNow
        };

        #endregion

    }
}

using ASPNetCoreData;

namespace ASPNet6
{
    public class UserDTO
    {

        #region Public Properties

        public int? UserId { get; set; }

        public string? UserName { get; set; }
        public string? Password { get; set; }

        public string? Email { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public DateTime BirthDate { get; set; }

        #endregion


        #region Public Methods

        public User CreateUserFromDTO() => CreateUserFromDTO(this);
        public void UpdateUserWithDTO(User user) => UpdateUserWithDTO(user, this);

        #endregion

        #region Static Methods

        public static User CreateUserFromDTO(UserDTO userDTO) => new()
        {
            UserName = userDTO.UserName,
            Email = userDTO.Email,
            EmailConfirmed = false,
            FirstName = userDTO.FirstName,
            LastName = userDTO.LastName,
            BirthDate = userDTO.BirthDate,
            LastSignedIn = null,
            DateCreated = DateTime.UtcNow
        };

        public static void UpdateUserWithDTO(User user, UserDTO userDTO)
        {
            user.UserName = userDTO.UserName ?? user.UserName;
            user.Email = userDTO.Email ?? user.Email;
            user.FirstName = userDTO.FirstName ?? user.FirstName;
            user.LastName = userDTO.LastName ?? user.LastName;
            user.BirthDate = userDTO.BirthDate != default ? userDTO.BirthDate : user.BirthDate;
        }

        #endregion

    }
}

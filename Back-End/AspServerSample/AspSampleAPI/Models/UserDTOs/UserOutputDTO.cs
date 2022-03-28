using AspServerData;

namespace AspSampleAPI.Models
{
    public class UserOutputDTO
    {

        #region Public Properties

        public int? UserId { get; set; }

        public string? UserName { get; set; }
        public string? Email { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        #endregion

        #region Static Methods

        public static UserOutputDTO Create(User user) => new()
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            BirthDate = user.BirthDate
        };

        #endregion

    }
}

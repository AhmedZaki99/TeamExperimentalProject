using ASPNetCoreData;

namespace DataProcessingAPI.Models
{
    public class UserDetailedOutputDTO : UserOutputDTO
    {

        #region Public Properties

        public DateTime? DateCreated { get; set; }
        public DateTime? LastSignedIn { get; set; }

        #endregion

        #region Static Methods

        public static new UserDetailedOutputDTO Create(User user) => new()
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            BirthDate = user.BirthDate,
            DateCreated = user.DateCreated,
            LastSignedIn = user.LastSignedIn
        };

        #endregion

    }
}

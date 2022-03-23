using AspServerData;
using System.ComponentModel.DataAnnotations;

namespace AspSampleAPI.Models
{
    public class UserUpdateInputDTO : UserCreateInputDTO
    {
        [Required(ErrorMessage = "User Id must be provided.")]
        public int? UserId { get; set; }


        #region Helper Methods

        public User Update(User user)
        {
            if (user.UserId != UserId)
            {
                throw new InvalidOperationException("User id provided must match with the DTO.");
            }
            user.UserName = UserName;
            user.Email = Email;
            user.FirstName = FirstName;
            user.LastName = LastName;
            user.BirthDate = BirthDate ?? throw new InvalidOperationException("User birth date must be provided.");

            return user;
        }

        #endregion
    }
}

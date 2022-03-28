using ASPNetCoreData;
using System.ComponentModel.DataAnnotations;

namespace DataProcessingAPI.Models
{
    public class UserLoginInputDTO
    {

        #region Public Properties

        [Required(ErrorMessage = "Username is required and can't be empty.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "User Password is required and can't be empty.")]
        public string? Password { get; set; }

        #endregion

    }
}

using System.ComponentModel.DataAnnotations;

namespace DataProcessingAPI.Models
{
    public class UserUpdateInputDTO : UserCreateInputDTO
    {
        [Required(ErrorMessage = "User Id must be provided.")]
        public int? UserId { get; set; }
    }
}

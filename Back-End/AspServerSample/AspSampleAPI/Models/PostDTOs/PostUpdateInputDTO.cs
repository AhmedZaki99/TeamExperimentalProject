using AspServerData;
using System.ComponentModel.DataAnnotations;

namespace AspSampleAPI.Models
{
    public class PostUpdateInputDTO
    {
        #region Public Properties

        [Required(ErrorMessage = "Post Id must be provided.")]
        public int? PostId { get; set; }

        public string? Caption { get; set; }

        [Required(ErrorMessage = "Post Content is required and can't be empty.")]
        public string? Content { get; set; }

        #endregion
    }
}

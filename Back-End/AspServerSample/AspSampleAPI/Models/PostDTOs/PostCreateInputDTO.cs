using AspServerData;
using System.ComponentModel.DataAnnotations;

namespace AspSampleAPI.Models
{
    public class PostCreateInputDTO
    {
        #region Public Properties

        public string? Caption { get; set; }

        [Required(ErrorMessage = "Post Content is required and can't be empty.")]
        public string? Content { get; set; }

        [Required(ErrorMessage = "Author Id must be provided.")]
        public int? AuthorId { get; set; }

        #endregion

        #region Helper Methods

        public Post Map() => new()
        {
            Caption = Caption,
            Content = Content,
            DatePosted = DateTime.UtcNow,
            UserId = AuthorId ?? throw new InvalidOperationException("Author Id must be provided.")
        };

        #endregion
    }
}

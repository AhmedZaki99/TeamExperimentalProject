using AspServerData;
using System.ComponentModel.DataAnnotations;

namespace AspSampleAPI.Models
{
    public class CommentCreateInputDTO
    {
        #region Public Properties
        
        [Required(ErrorMessage = "Comment Content is required and can't be empty.")]
        public string? Content { get; set; }

        [Required(ErrorMessage = "Post Id must be provided.")]
        public int? PostId { get; set; }

        [Required(ErrorMessage = "Author Id must be provided.")]
        public int? AuthorId { get; set; }

        #endregion

        #region Helper Methods

        public Comment Map() => new()
        {
            Content = Content,
            DatePosted = DateTime.UtcNow,
            UserId = AuthorId ?? throw new InvalidOperationException("Author Id must be provided."),
            PostId = PostId ?? throw new InvalidOperationException("Post Id must be provided.")
        };

        #endregion
    }
}

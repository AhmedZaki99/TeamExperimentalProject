using AspServerData;
using System.ComponentModel.DataAnnotations;

namespace AspSampleAPI.Models
{
    public class CommentUpdateInputDTO
    {
        #region Public Properties

        [Required(ErrorMessage = "Comment Id must be provided.")]
        public int? CommentId { get; set; }

        [Required(ErrorMessage = "Comment Content is required and can't be empty.")]
        public string? Content { get; set; }

        #endregion

        #region Helper Methods

        public Comment Update(Comment comment)
        {
            if (comment.CommentId != CommentId)
            {
                throw new InvalidOperationException("Comment id provided must match with the DTO.");
            }
            comment.Content = Content;

            return comment;
        }

        #endregion

    }
}

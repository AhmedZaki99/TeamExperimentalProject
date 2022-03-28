using AspServerData;

namespace AspSampleAPI.Models
{
    public class CommentOutputDTO
    {
        #region Public Properties

        public int? CommentId { get; set; }
        public string? Content { get; set; }

        public DateTime? DatePosted { get; set; }
        public DateTime? LastEdited { get; set; }

        public int? PostId { get; set; }
        public int? AuthorId { get; set; }

        #endregion

        #region Static Methods

        public static CommentOutputDTO Create(Comment comment) => new()
        {
            CommentId = comment.CommentId,
            Content = comment.Content,
            DatePosted = comment.DatePosted,
            LastEdited = comment.LastEdited,
            PostId = comment.PostId,
            AuthorId = comment.UserId
        };

        #endregion
    }
}

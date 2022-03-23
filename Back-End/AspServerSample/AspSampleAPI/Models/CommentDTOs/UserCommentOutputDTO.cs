using AspServerData;
using System.Text.Json.Serialization;

namespace AspSampleAPI.Models
{
    public class UserCommentOutputDTO : CommentOutputDTO
    {
        #region Public Properties

        [JsonPropertyOrder(1)]
        public string? AuthorName { get; set; }

        [JsonPropertyOrder(2)]
        public string? AuthorFullName { get; set; }

        #endregion

        #region Static Methods

        public static new UserCommentOutputDTO Create(Comment comment) => CreateExplicitly(comment, comment.User);

        public static UserCommentOutputDTO CreateExplicitly(Comment comment, User? user) => new()
        {
            CommentId = comment.CommentId,
            Content = comment.Content,
            DatePosted = comment.DatePosted,
            LastEdited = comment.LastEdited,
            PostId = comment.PostId,
            AuthorId = comment.UserId,
            AuthorName = user?.UserName,
            AuthorFullName = user?.FirstName + " " + user?.LastName
        };

        #endregion
    }
}

using AspServerData;
using System.Text.Json.Serialization;

namespace AspSampleAPI.Models
{
    public class CommentsPostOutputDTO : PostOutputDTO
    {
        #region Public Properties

        [JsonPropertyOrder(1)]
        public IEnumerable<UserCommentOutputDTO>? Comments { get; set; }

        #endregion

        #region Static Methods

        public static new CommentsPostOutputDTO Create(Post post) => CreateExplicitly(post, post.User, post.Comments);

        public static new CommentsPostOutputDTO CreateExplicitly(Post post, User? user) => CreateExplicitly(post, user, user?.Comments);

        public static CommentsPostOutputDTO CreateExplicitly(Post post, User? user, IList<Comment>? comments) => new()
        {
            PostId = post.PostId,
            Caption = post.Caption,
            Content = post.Content,
            DatePosted = post.DatePosted,
            LastEdited = post.LastEdited,
            AuthorId = post.UserId,
            AuthorName = user?.UserName,
            AuthorFullName = user?.FirstName + " " + user?.LastName,
            Comments = comments?.Select(c => UserCommentOutputDTO.Create(c))
        };

        #endregion
    }
}

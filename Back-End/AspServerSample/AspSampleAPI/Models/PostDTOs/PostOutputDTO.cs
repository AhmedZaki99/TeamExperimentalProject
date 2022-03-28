using AspServerData;

namespace AspSampleAPI.Models
{
    public class PostOutputDTO
    {
        #region Public Properties

        public int? PostId { get; set; }

        public string? Caption { get; set; }
        public string? Content { get; set; }

        public DateTime? DatePosted { get; set; }
        public DateTime? LastEdited { get; set; }

        public int? AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public string? AuthorFullName { get; set; }

        #endregion

        #region Static Methods

        public static PostOutputDTO Create(Post post) => CreateExplicitly(post, post.User);

        public static PostOutputDTO CreateExplicitly(Post post, User? user) => new()
        {
            PostId = post.PostId,
            Caption = post.Caption,
            Content = post.Content,
            DatePosted = post.DatePosted,
            LastEdited = post.LastEdited,
            AuthorId = post.UserId,
            AuthorName = user?.UserName,
            AuthorFullName = user?.FirstName + " " + user?.LastName
        };

        #endregion
    }
}

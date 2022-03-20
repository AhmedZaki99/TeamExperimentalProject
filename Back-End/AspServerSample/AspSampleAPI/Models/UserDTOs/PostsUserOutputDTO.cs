using AspServerData;

namespace AspSampleAPI.Models
{
    public class PostsUserOutputDTO : UserOutputDTO
    {

        #region Public Properties

        public DateTime? DateCreated { get; set; }
        public DateTime? LastSignedIn { get; set; }

        public IEnumerable<CommentsPostOutputDTO>? Posts { get; set; }

        #endregion


        #region Static Methods

        public static new PostsUserOutputDTO Create(User user) => CreateExplicitly(user, user.Posts);

        public static PostsUserOutputDTO CreateExplicitly(User user, IList<Post>? posts) => new()
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            BirthDate = user.BirthDate,
            DateCreated = user.DateCreated,
            LastSignedIn = user.LastSignedIn,
            Posts = posts?.Select(p => CommentsPostOutputDTO.Create(p))
        };

        #endregion

    }
}

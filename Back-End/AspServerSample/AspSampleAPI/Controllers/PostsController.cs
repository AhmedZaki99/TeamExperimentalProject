using AspSampleAPI.Models;
using AspServerData;
using Microsoft.AspNetCore.Mvc;

namespace AspSampleAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {

        #region Dependencies

        private readonly IPostStore _postStore;
        private readonly IUserStore _userStore;

        #endregion

        #region Constructor

        public PostsController(IPostStore postStore, IUserStore userStore)
        {
            _postStore = postStore;
            _userStore = userStore;
        }

        #endregion


        #region Controller Actions

        /// <summary>
        /// List posts per page, ordered by newest.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostOutputDTO>>> ListPostsAsync([FromQuery] int page = 1, [FromQuery] int per_page = 30)
        {
            var posts = await _postStore.ListPostsWithAuthorsAsync(page, per_page);

            return posts.Select(p => PostOutputDTO.Create(p)).ToList();
        }

        /// <summary>
        /// Get post by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentsPostOutputDTO>> GetPostAsync([FromRoute] int id)
        {
            var post = await _postStore.FindAndNavigateAsync(id);
            if (post is null)
            {
                return NotFound();
            }
            return CommentsPostOutputDTO.Create(post);
        }

        /// <summary>
        /// Create new post.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PostOutputDTO>> CreatePostAsync([FromBody] PostCreateInputDTO postDTO)
        {
            var user = await _userStore.FindAsync((int)postDTO.AuthorId!);
            if (user is null)
            {
                ModelState.AddModelError(nameof(postDTO.AuthorId), "No user exists with the provided Author Id.");
            }
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var post = await _postStore.CreateAsync(postDTO.Map());

            return CreatedAtAction(nameof(GetPostAsync), new { id = post.PostId }, PostOutputDTO.CreateExplicitly(post, user));
        }

        /// <summary>
        /// Update post by id.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePostAsync([FromRoute] int id, [FromBody] PostUpdateInputDTO postDTO)
        {
            if (postDTO.PostId != id)
            {
                ModelState.AddModelError(nameof(postDTO.PostId), "Post Id provided doesn't match with the Post Id in route.");
            }
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var post = await _postStore.FindAsync(id);
            if (post is null)
            {
                return NotFound();
            }

            await _postStore.UpdateAsync(postDTO.Update(post));

            return NoContent();
        }

        /// <summary>
        /// Delete post by id.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostAsync([FromRoute] int id)
        {
            var deleteResult = await _postStore.DeleteAsync(id);

            if (deleteResult == DeleteResult.EntityNotFound)
            {
                return NotFound();
            }
            else if (deleteResult == DeleteResult.Failed)
            {
                return Problem(statusCode: 500, title: "Server error.", detail: "Failed to delete the post.");
            }

            return NoContent();
        }

        #endregion

    }
}

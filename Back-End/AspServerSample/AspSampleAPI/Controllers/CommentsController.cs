using AspSampleAPI.Models;
using AspServerData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspSampleAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {

        #region Dependencies

        private readonly ICommentStore _commentStore;
        private readonly IPostStore _postStore;
        private readonly IUserStore _userStore;

        #endregion

        #region Constructor

        public CommentsController(ICommentStore commentStore, IPostStore postStore, IUserStore userStore)
        {
            _commentStore = commentStore;
            _postStore = postStore;
            _userStore = userStore;
        }

        #endregion


        #region Controller Actions

        /// <summary>
        /// List comments per page, ordered by newest.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentOutputDTO>>> ListCommentsAsync([FromQuery] int page = 1, [FromQuery] int per_page = 30)
        {
            var comments = await _commentStore.ListCommentsAsync(page, per_page);

            return comments.Select(c => CommentOutputDTO.Create(c)).ToList();
        }

        /// <summary>
        /// Get comment by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentOutputDTO>> GetCommentAsync([FromRoute] int id)
        {
            var comment = await _commentStore.FindByIdAsync(id);
            if (comment is null)
            {
                return NotFound();
            }
            return CommentOutputDTO.Create(comment);
        }

        /// <summary>
        /// Create new comment.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CommentOutputDTO>> CreateCommentAsync([FromBody] CommentCreateInputDTO commentDTO)
        {
            if (!_userStore.UserExists((int)commentDTO.AuthorId!))
            {
                ModelState.AddModelError(nameof(commentDTO.AuthorId), "No user exists with the provided Author Id.");
            }
            if (!_postStore.PostExists((int)commentDTO.PostId!))
            {
                ModelState.AddModelError(nameof(commentDTO.PostId), "No post exists with the provided Post Id.");
            }
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var comment = await _commentStore.CreateAsync(commentDTO.Map());

            return CreatedAtAction(nameof(GetCommentAsync), new { id = comment.CommentId }, CommentOutputDTO.Create(comment));
        }

        /// <summary>
        /// Update comment by id.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCommentAsync([FromRoute] int id, [FromBody] CommentUpdateInputDTO commentDTO)
        {
            if (commentDTO.CommentId != id)
            {
                ModelState.AddModelError(nameof(commentDTO.CommentId), "Comment Id provided doesn't match with the Comment Id in route.");
            }
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var comment = await _commentStore.FindByIdAsync(id);
            if (comment is null)
            {
                return NotFound();
            }

            await _commentStore.UpdateAsync(commentDTO.Update(comment));

            return NoContent();
        }

        /// <summary>
        /// Delete comment by id.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommentAsync([FromRoute] int id)
        {
            var deleteResult = await _commentStore.DeleteAsync(id);

            if (deleteResult == DeleteResult.EntityNotFound)
            {
                return NotFound();
            }
            else if (deleteResult == DeleteResult.Failed)
            {
                return Problem(statusCode: 500, title: "Server error.", detail: "Failed to delete the comment.");
            }

            return NoContent();
        }

        #endregion

    }
}

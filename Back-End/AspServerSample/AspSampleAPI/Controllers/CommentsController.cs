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

        private readonly ApplicationDbContext _dbContext;

        #endregion

        #region Constructor

        public CommentsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion


        #region Controller Actions

        /// <summary>
        /// List comments per page, ordered by newest.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentOutputDTO>>> ListCommentsAsync([FromQuery] int page = 1, [FromQuery] int per_page = 30)
        {
            return await _dbContext.Comments.OrderByDescending(c => c.DatePosted)
                                            .Skip((page - 1) * per_page)
                                            .Take(per_page)
                                            .Select(c => CommentOutputDTO.Create(c))
                                            .ToListAsync();
        }

        /// <summary>
        /// Get comment by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentOutputDTO>> GetCommentAsync([FromRoute] int id)
        {
            var comment = await _dbContext.Comments.FindAsync(id);
            if (comment == null)
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
            if (!_dbContext.UserExists((int)commentDTO.AuthorId!))
            {
                ModelState.AddModelError(nameof(commentDTO.AuthorId), "No user exists with the provided Author Id.");
            }
            if (!_dbContext.PostExists((int)commentDTO.PostId!))
            {
                ModelState.AddModelError(nameof(commentDTO.PostId), "No post exists with the provided Post Id.");
            }
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var comment = commentDTO.Map();

            _dbContext.Comments.Add(comment);
            await _dbContext.SaveChangesAsync();

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

            var comment = await _dbContext.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _dbContext.Entry(comment).CurrentValues.SetValues(commentDTO);
            comment.LastEdited = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Delete comment by id.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommentAsync([FromRoute] int id)
        {
            var comments = await _dbContext.Comments.FindAsync(id);
            if (comments == null)
            {
                return NotFound();
            }

            _dbContext.Comments.Remove(comments);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        #endregion

    }
}

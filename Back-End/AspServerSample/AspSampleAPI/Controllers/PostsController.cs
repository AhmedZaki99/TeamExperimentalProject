using AspSampleAPI.Models;
using AspServerData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspSampleAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {

        #region Dependencies

        private readonly ApplicationDbContext _dbContext;

        #endregion

        #region Constructor

        public PostsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion


        #region Controller Actions

        /// <summary>
        /// List posts per page, ordered by newest.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostOutputDTO>>> ListPostsAsync([FromQuery] int page = 1, [FromQuery] int per_page = 30)
        {
            return await _dbContext.Posts.Include(p => p.User)
                                         .OrderByDescending(p => p.DatePosted)
                                         .Skip((page - 1) * per_page)
                                         .Take(per_page)
                                         .Select(p => PostOutputDTO.Create(p))
                                         .AsNoTracking()
                                         .ToListAsync();
        }

        /// <summary>
        /// Get post by id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentsPostOutputDTO>> GetPostAsync([FromRoute] int id)
        {
            var post = await _dbContext.Posts.Include(p => p.User)
                                             .Include(p => p.Comments)
                                                .ThenInclude(c => c.User)
                                             .AsNoTracking()
                                             .FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null)
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
            var user = await _dbContext.Users.FindAsync(postDTO.AuthorId);
            if (user == null)
            {
                ModelState.AddModelError(nameof(postDTO.AuthorId), "No user exists with the provided Author Id.");
            }
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var post = postDTO.Map();

            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();

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

            var post = await _dbContext.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _dbContext.Entry(post).CurrentValues.SetValues(postDTO);
            post.LastEdited = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Delete post by id.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostAsync([FromRoute] int id)
        {
            var post = await _dbContext.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _dbContext.Posts.Remove(post);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        #endregion

    }
}

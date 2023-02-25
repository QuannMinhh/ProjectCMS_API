using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCMS.Data;
using ProjectCMS.Models;
using ProjectCMS.ViewModels;
using System.Xml.Linq;

namespace ProjectCMS.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public CommentController (ApplicationDbContext dbcontext)
        {
            _dbContext = dbcontext;
        }

        // Get all comments
        [HttpGet]
        public async Task<IActionResult> GetAllComments()
        {
            List<Comment> comments = await _dbContext._comments.ToListAsync();
            return Ok(comments);
        }

        // Add a comment
        [HttpPost]
        public async Task<IActionResult> AddComment(CommentViewModel comment)
        {
            if (ModelState.IsValid)
            {
                Comment newComment = new()
                {
                    Content = comment.Content,
                    UserId = comment.UserId,
                    IdeaId = comment.IdeaId,
                };

                await _dbContext._comments.AddAsync(newComment);
                await _dbContext.SaveChangesAsync();
                return Ok(newComment.Content);
            }
            return BadRequest();
        }

        // Get comment by id
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetComment([FromRoute] int id)
        {
            var comment = _dbContext._comments.FindAsync(id);
            if(comment != null)
            {
                return Ok(await _dbContext._comments.FindAsync(id)); 
            }
            return BadRequest("Not found comment !"); 
        }


        // Delete comment
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult>  DeleteComment([FromRoute] int id)
        {
            var comment = await _dbContext._comments.FindAsync(id);
            if (comment != null)
            {
                _dbContext._comments.Remove(comment);
                _dbContext.SaveChanges();
                return Ok("Successfully deleted!");
            }
            return NotFound("Comment does not exist");
        }


        // Edit comment
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> EditComment(CommentViewModel newComment, [FromRoute] int id)
        {
            var comment = await _dbContext._comments.FindAsync(id);
            if (comment != null)
            {
                comment.AddedDate = DateTime.Now;
                comment.IdeaId = newComment.IdeaId;
                comment.UserId = newComment.UserId;
                comment.Content = newComment.Content;
                _dbContext.SaveChanges();
                return Ok("Successfully deleted!");
            }
            return NotFound("Comment does not exist");
        }


        // Sort comment by added date 
        [HttpGet("{sort}")]
        public async Task<IActionResult> SortComments()
        {
            try
            {
                return Ok(await _dbContext._comments.OrderByDescending(x => x.AddedDate).ToListAsync());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                  "Error retrieving data from the database");
            }
        }
    }
}

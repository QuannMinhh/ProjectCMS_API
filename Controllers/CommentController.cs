using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCMS.Data;
using ProjectCMS.Models;
using ProjectCMS.ViewModels;

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

        [HttpGet]
        public async Task<IActionResult> GetComments()
        {
            List<Comment> comments = await _dbContext._comments.ToListAsync();
            return Ok(comments);
        }

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

                _dbContext._comments.Add(newComment);
                _dbContext.SaveChanges();
                return Ok(newComment);
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult>  DeleteComment(int id)
        {
            var comment = await _dbContext._comments.FindAsync(id);
            if (comment != null)
            {
                _dbContext._comments.Remove(comment);
                _dbContext.SaveChanges();
                return Ok("Delete successfully!");
            }
            return NotFound("Comment does not exist");
        }
    }
}

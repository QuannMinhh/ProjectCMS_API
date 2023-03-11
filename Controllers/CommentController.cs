using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCMS.Data;
using ProjectCMS.Models;
using ProjectCMS.Services;
using ProjectCMS.ViewModels;

namespace ProjectCMS.Controllers
{
    [Route("api/comment")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly EmailService _emailService;
        public CommentController(ApplicationDbContext dbcontext, EmailService emailService)
        {
            _dbContext = dbcontext;
            _emailService = emailService;
        }

        // Get all comments of an idea
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetAllComments([FromRoute] int id, string? sort)
        {
            var comments =  _dbContext._comments.Where(x => x.IdeaId == id); 
            if (comments.Any())
            {
                if (!sort.IsNullOrEmpty())
                    return Ok(await comments.OrderByDescending(x => x.AddedDate).ToListAsync());
                return Ok(comments);
            }
            return NotFound(new {message = "This idea has no comment !"});
        }

        // Add a comment
        [HttpPost]
        public async Task<IActionResult> CreateComment(CommentViewModel comment)
        {
                if (ModelState.IsValid)
                {
                    Comment newComment = new()
                    {
                        Content = comment.Content,
                        IdeaId = comment.IdeaId,
                        UserId = comment.UserId,
                        AddedDate = DateTime.Now
                    };
                    await _dbContext._comments.AddAsync(newComment);
                    await _dbContext.SaveChangesAsync();

                    NewCommentNofity(comment.UserId, comment.IdeaId);

                    return Ok(await _dbContext._comments.Where(x => x.IdeaId == comment.IdeaId).ToListAsync());
                }
            return BadRequest(new {message = "Some value is not valid. Please retype the value." }); 
         }


        // Delete comment
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int id)
        {
            var comment = await _dbContext._comments.FindAsync(id);
            if (comment != null)
            {
                _dbContext._comments.Remove(comment);
                await _dbContext.SaveChangesAsync();
                return Ok(await _dbContext._comments.ToListAsync());
            }
            return NotFound(new {message = "Comment does not exist." });
        }


        // Edit comment
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> EditComment(CommentViewModel newComment, [FromRoute] int id)
        {
            var comment = await _dbContext._comments.FindAsync(id);
            if (comment != null)
            {
                if (ModelState.IsValid)
                {
                    comment.Content = newComment.Content;
                    await _dbContext.SaveChangesAsync();
                    return Ok(await _dbContext._comments.ToListAsync());
                }
                return BadRequest(new {message = "Some value is not valid. Please retype the value." });
            }
            return NotFound(new {message = "Comment does not exist." });
        }

        private async Task NewCommentNofity(int idUser, int idIdea)
        {
            var sender = await _dbContext._users.FindAsync(idUser);
            var ownerIdea = (await _dbContext._idea.FindAsync(idIdea)).UserId;
            var owerEmail = (await _dbContext._users.FindAsync(ownerIdea)).Email;

            _emailService.NewCommentNotify(sender.UserName, owerEmail);

            return;
        }

        // Sort comment by added date 
        //[HttpGet("{sort}/{id}")]
        //public async Task<IActionResult> SortComments([FromRoute] int id)
        //{
        //    try
        //    {
        //        var comments = _dbContext._comments.Where(x => x.IdeaId == id).OrderByDescending(x => x.AddedDate);
        //        if (comments.Any())
        //            return Ok(await comments.ToListAsync());
        //        return NotFound(JsonDocument.Parse("{\"Message\":\"This idea has no comment yet.\"}"));
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
        //    }
        //}

        
              
 




    }
}


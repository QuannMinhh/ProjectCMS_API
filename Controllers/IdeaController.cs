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
    [Route("api/idea")]
    [ApiController]
    [Authorize]
    public class IdeaController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _env;
        public IdeaController(
            ApplicationDbContext dbContext,
            EmailService emailservice,
            IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _emailService = emailservice;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetIdeas(string? searchString)
        {
                var ideas =  from i in _dbContext._idea select i;
                if (!searchString.IsNullOrEmpty())
                {
                    ideas = ideas.Where(s => s.Name.Contains(searchString));
                    if (ideas.Any())
                    {
                        return Ok(await ideas.ToListAsync());
                    }
                    return NotFound();
                }

                if (ideas.Any())
                {
                    return Ok(await ideas.ToListAsync());
                }
                return NotFound();
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetDetail([FromRoute] int id)
        {
            var idea = await _dbContext._idea.FindAsync(id);
            if (idea == null)
            {
                return NotFound();
            }

            return Ok(idea);
        }

        [HttpGet("{sort}")]
        public async Task<IActionResult> Sort(string sortType)
        {
            try
            {
                var ideas = from i in _dbContext._idea select i;
                if (sortType != null)
                {
                    switch (sortType)
                    {
                        case "mpi":
                            ideas = ideas.OrderByDescending(s => s.Vote);
                            break;
                        case "mvi":
                            ideas = ideas.OrderByDescending(s => s.Viewed);
                            break;
                        case "lid":
                            ideas = ideas.OrderByDescending(s => s.AddedDate);
                            break;
                    }
                }

                if(ideas.Any())
                {
                    return Ok(await ideas.ToListAsync());
                }

                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }          
        }

        [HttpPost]
        public async Task<IActionResult> CreateIdea(IdeaViewModel idea)
        {

            if (ModelState.IsValid)
            {
                Idea newIdea = new()
                {
                    Name = idea.Title,
                    Content = idea.Content,
                    Vote = idea.Vote,
                    Viewed = idea.Viewed,
                    AddedDate = idea.SubmitedDate,
                    EvId = idea.eId,
                    CateId = idea.cId,
                    UserId = idea.uId,
                    IdeaFile = idea.IdeaFile.FileName
                };
                if (idea.IdeaFile!= null)
                {
                    await SaveFile(idea.IdeaFile);
                }
      
                await _dbContext._idea.AddAsync(newIdea);
                await _dbContext.SaveChangesAsync();

                var eventName = await _dbContext._events.FindAsync(idea.eId);
                var submiter = await _dbContext._users.FindAsync(idea.uId);
                var admin = (await _dbContext._users.Where(u => u.Role == "Admin").ToListAsync())
                                .Select(u => u.Email).ToArray();
                //Send Email to Admin
               _emailService.NewIdeaNotify(eventName.Name, submiter.UserName, admin);

                return Ok(new {message = "Your Idea has been submited"});
            }

            return BadRequest();
        }


        [HttpDelete]
        [Route("id:int")]
        public async Task<IActionResult> DeleteIdea([FromRoute] int id)
        {
            var idea = await _dbContext._idea.FindAsync(id);
            if (idea != null)
            {
                 _dbContext._idea.Remove(idea);
                await _dbContext.SaveChangesAsync();

                if(idea.IdeaFile.IsNullOrEmpty()) 
                {
                    await RemoveFile(idea.IdeaFile);
                }
                
                return Ok();
            }

            return NotFound();
        }

        private async Task<bool> SaveFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return false;
            }

            var webStaticPath = Path.Combine(_env.WebRootPath, "WebStatic");
            var filePath = Path.Combine(webStaticPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return true;
        }
        private async Task<bool> RemoveFile(string file)
        {
            var webStaticPath = Path.Combine(_env.WebRootPath, "WebStatic");
            var filePath = Path.Combine(webStaticPath, file);

            if (!System.IO.File.Exists(filePath))
            {
                return false;
            }
            else
            {
                System.IO.File.Delete(filePath);
                return true;
            }
        }
    }
}

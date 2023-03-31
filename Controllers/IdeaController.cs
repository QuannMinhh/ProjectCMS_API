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
            this._env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetIdeas(string? searchString)
        {
            var ideas = from i in _dbContext._idea
                        join u in _dbContext._users
                        on i.UserId equals u.UserId
                        select new
                        {
                            Id = i.Id,
                            Name = i.Name,
                            Content = i.Content,
                            AddedDate = i.AddedDate,
                            Vote = i.Vote,
                            Viewed= i.Viewed,
                            IdeaFile = i.IdeaFile,
                            EvId = i.EvId,
                            CateId= i.CateId,
                            UserId = i.UserId,
                            UserName = u.UserName,
                            Avatar = u.Avatar
                        };
            //user Name 
            // user avatar
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
            var ideas = from idea in _dbContext._idea
                        join user in _dbContext._users on idea.UserId equals user.UserId
                        join evt in _dbContext._events on idea.EvId equals evt.Id
                        join cate in _dbContext._categories on idea.CateId equals cate.Id
                        where idea.Id == id 
                        select new
                        {
                            IdeaId = idea.Id,
                            IdeaName = idea.Name,
                            IdeaContent = idea.Content,
                            IdeaAddedDate = idea.AddedDate,
                            IdeaVote = idea.Vote,
                            IdeaViewed = idea.Viewed,
                            IdeaFile = idea.IdeaFile,
                            UserName = user.UserName,
                            Avatar = user.Avatar,
                            EventName = evt.Name,
                            EventFirstClosure = evt.First_Closure,
                            EventLastClosure = evt.Last_Closure,
                            CategoryName = cate.Name
                        };
            if (ideas == null)
            {
                return NotFound();
            }

            return Ok(await ideas.ToListAsync());
        }

        [HttpGet("byUser/{id}")]
        [Route("{id:int}")]
        public async Task<IActionResult> GetByUser([FromRoute] int id)
        {
            var byUser = from idea in _dbContext._idea
                         join user in _dbContext._users on idea.UserId equals user.UserId
                         join cate in _dbContext._categories on idea.EvId equals cate.Id
                         join evt in _dbContext._events on idea.EvId equals evt.Id
                         where idea.UserId == id
                         select new
                         {
                             IdeaId = idea.Id,
                             IdeaName = idea.Name,
                             IdeaContent = idea.Content,
                             IdeaAddedDate = idea.AddedDate,
                             IdeaVote = idea.Vote,
                             IdeaViewed = idea.Viewed,
                             IdeaFile = idea.IdeaFile,
                             UserName = user.UserName,
                             Avatar = user.Avatar,
                             EventName = evt.Name,
                             EventFirstClosure = evt.First_Closure,
                             EventLastClosure = evt.Last_Closure,
                             CategoryName = cate.Name
                            };
            return Ok(await byUser.ToListAsync());
        }
        [HttpGet("byEvent/{id}")]
        [Route("{id:int}")]
        public async Task<IActionResult> GetByEvent([FromRoute] int id)
        {
            var byUser = from idea in _dbContext._idea
                         join user in _dbContext._users on idea.UserId equals user.UserId
                         join cate in _dbContext._categories on idea.EvId equals cate.Id
                         join evt in _dbContext._events on idea.EvId equals evt.Id
                         where idea.EvId == id
                         select new
                         {
                             IdeaId = idea.Id,
                             IdeaName = idea.Name,
                             IdeaContent = idea.Content,
                             IdeaAddedDate = idea.AddedDate,
                             IdeaVote = idea.Vote,
                             IdeaViewed = idea.Viewed,
                             IdeaFile = idea.IdeaFile,
                             UserName = user.UserName,
                             Avatar = user.Avatar,
                             EventName = evt.Name,
                             EventFirstClosure = evt.First_Closure,
                             EventLastClosure = evt.Last_Closure,
                             CategoryName = cate.Name
                         };
            return Ok(await byUser.ToListAsync());
        }

        [HttpGet("countByUser/{id}")]
        [Route("{id:int}")]
        public async Task<IActionResult> CountByUser([FromRoute] int id)
        {
            var byUser = await _dbContext._idea.Where(i => i.UserId == id).ToListAsync();
            return Ok(byUser.Count());
        }

        [HttpGet("sort")]
        public async Task<IActionResult> Sort(string sortType)
        {
            try
            {
                var ideas = from i in _dbContext._idea
                            join u in _dbContext._users on i.UserId equals u.UserId
                            select new
                            {
                                Id = i.Id,
                                Name = i.Name,
                                Content = i.Content,
                                AddedDate = i.AddedDate,
                                Vote = i.Vote,
                                Viewed = i.Viewed,
                                IdeaFile = i.IdeaFile,
                                EvId = i.EvId,
                                CateId = i.CateId,
                                UserId = i.UserId,
                                UserName = u.UserName,
                                Avatar = u.Avatar
                            };
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
                        default:
                            return Ok(new {message = "Your sort type is incorrect!" });
                            break;
                    }
                }

                if(ideas.Any())
                {
                    return Ok(await ideas.ToListAsync());
                }

                return Ok(new { message = "Your sort type is incorrect!" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }          
        }

        [HttpPost]
        public async Task<IActionResult> CreateIdea([FromForm] IdeaViewModel idea)
        {
            var deadline = (await _dbContext._events.FindAsync(idea.eId)).First_Closure;
            if(idea.SubmitedDate > deadline)
            {
                return BadRequest(new {message = "Event is over!"});
            }

            if (ModelState.IsValid)
            {
                var eventName = await _dbContext._events.FindAsync(idea.eId);
                var submiter = await _dbContext._users.FindAsync(idea.uId);
                var fileName = "";

                if (idea.IdeaFile != null)
                {
                    fileName = submiter.UserName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + idea.IdeaFile.FileName;
                    await SaveFile(fileName, submiter.UserName, idea.IdeaFile);
                }

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
                    IdeaFile = fileName
                };
                
      
                await _dbContext._idea.AddAsync(newIdea);
                await _dbContext.SaveChangesAsync();

                
                var admin = (await _dbContext._users.Where(u => u.Role == "Admin").ToListAsync())
                                .Select(u => u.Email).ToArray();
                //Send Email to Admin
               _emailService.NewIdeaNotify(eventName.Name, submiter.UserName, admin);

                return Ok(new {message = "Your Idea has been submited"});
            }

            return BadRequest();
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteIdea([FromRoute] int id)
        {
            
            var idea = await _dbContext._idea.FindAsync(id);

            if (idea != null)
            {
                 _dbContext._idea.Remove(idea);
                await _dbContext.SaveChangesAsync();

                if(!idea.IdeaFile.IsNullOrEmpty()) 
                {
                    await RemoveFile(idea.IdeaFile);
                }
                
                return Ok();
            }

            return NotFound();
        }


        private async Task<bool> SaveFile(string fileName, string username, IFormFile file)
        {
            

                if (file == null || file.Length == 0)
                {
                    return false;
                }

                string filePath = _env.WebRootPath + "\\Idea\\" + fileName;

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return true;
            
        }
        private async Task<Task> RemoveFile(string file)
        {
            
            string filePath = _env.WebRootPath + "\\Idea\\" + file;

            if (!System.IO.File.Exists(filePath))
            {
                return Task.CompletedTask;
            }
            else
            {
                System.IO.File.Delete(filePath);
                return Task.CompletedTask;
            }
        }

    }
}

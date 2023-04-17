using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Scripting.Utils;
using Org.BouncyCastle.Crypto;
using ProjectCMS.Data;
using ProjectCMS.Models;
using ProjectCMS.Services;
using ProjectCMS.ViewModels;
using System.Linq;

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

        public IdeaController(ApplicationDbContext dbContext, EmailService emailservice, IWebHostEnvironment env)
        {
            _dbContext = dbContext;
            _emailService = emailservice;
            this._env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetIdeas(string? searchString)
        {
            var ideas = from i in _dbContext._idea
                        join u in _dbContext._users on i.UserId equals u.UserId
                        join dep in _dbContext._departments on u.DepartmentID equals dep.DepId
                        
                        select new
                        {
                            Id = i.Id,
                            Name = i.Name,
                            Content = i.Content,
                            Anonymous = i.IsAnonymous,
                            AddedDate = i.AddedDate,
                            Vote = i.Vote,
                            Viewed= i.Viewed,
                            IdeaFile = i.IdeaFile,
                            EvId = i.EvId,
                            CateId= i.CateId,
                            UserId = i.UserId,
                            UserName = u.UserName,
                            Avatar = u.Avatar,
                            DepartmentName = dep.Name

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
                    return Ok(await ideas.ToListAsync());
            }
                if (ideas.Any())
                {
                    return Ok(await ideas.ToListAsync());
                }
                return Ok(await ideas.ToListAsync()); ;
        }

        [HttpGet("byDetail/{id}")]
        [Route("{id:int}")]
        public async Task<IActionResult> GetDetail([FromRoute] int id)
        {
            var ideas = from idea in _dbContext._idea
                        join user in _dbContext._users on idea.UserId equals user.UserId
                        join evt in _dbContext._events on idea.EvId equals evt.Id
                        join cate in _dbContext._categories on idea.CateId equals cate.Id
                        join dep in _dbContext._departments on user.DepartmentID equals dep.DepId
                        where idea.Id == id 
                        select new
                        {
                            Id = idea.Id,
                            Name = idea.Name,
                            Content = idea.Content,
                            Anonymous = idea.IsAnonymous,
                            AddedDate = idea.AddedDate,
                            Vote = idea.Vote,
                            Viewed = idea.Viewed,
                            IdeaFile = idea.IdeaFile,
                            EvId = idea.EvId,
                            CateId = idea.CateId,
                            UserName = user.UserName,
                            Avatar = user.Avatar,
                            EventName = evt.Name,
                            EventFirstClosure = evt.First_Closure,
                            EventLastClosure = evt.Last_Closure,
                            CategoryName = cate.Name,
                            DepartmentName = dep.Name
                        };
            if (ideas == null)
            {
                return Ok(new {message = "No Idea Found!"});
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
                         join dep in _dbContext._departments on user.DepartmentID equals dep.DepId
                         where idea.UserId == id
                         select new
                         {
                             Id = idea.Id,
                             Name = idea.Name,
                             Content = idea.Content,
                             Anonymous = idea.IsAnonymous,
                             AddedDate = idea.AddedDate,
                             Vote = idea.Vote,
                             Viewed = idea.Viewed,
                             IdeaFile = idea.IdeaFile,
                             EvId = idea.EvId,
                             CateId = idea.CateId,
                             UserName = user.UserName,
                             Avatar = user.Avatar,
                             EventName = evt.Name,
                             EventFirstClosure = evt.First_Closure,
                             EventLastClosure = evt.Last_Closure,
                             CategoryName = cate.Name, 
                             DepartmentName = dep.Name
                            };
            return Ok(await byUser.ToListAsync());
        }
        [HttpGet("byEvent/{id}")]
        [Route("{id:int}")]
        public async Task<IActionResult> GetByEvent([FromRoute] int id)
        {
            var byEvent = from idea in _dbContext._idea
                         join user in _dbContext._users on idea.UserId equals user.UserId
                         join cate in _dbContext._categories on idea.EvId equals cate.Id
                         join evt in _dbContext._events on idea.EvId equals evt.Id
                         join dep in _dbContext._departments on user.DepartmentID equals dep.DepId
                         where idea.EvId == id
                         select new
                         {
                             Id = idea.Id,
                             Name = idea.Name,
                             Content = idea.Content,
                             Anonymous = idea.IsAnonymous,
                             AddedDate = idea.AddedDate,
                             Vote = idea.Vote,
                             Viewed = idea.Viewed,
                             IdeaFile = idea.IdeaFile,
                             EvId = idea.EvId,
                             CateId = idea.CateId,
                             UserName = user.UserName,
                             Avatar = user.Avatar,
                             EventName = evt.Name,
                             EventFirstClosure = evt.First_Closure,
                             EventLastClosure = evt.Last_Closure,
                             CategoryName = cate.Name,
                             DepartmentName = dep.Name
                         };
            return Ok(await byEvent.ToListAsync());
        }

        [HttpGet("countByUser/{id}")]
        [Route("{id:int}")]
        public async Task<IActionResult> CountByUser([FromRoute] int id)
        {
            var byUser = await _dbContext._idea.Where(i => i.UserId == id).ToListAsync();
            return Ok(byUser.Count());
        }
        [HttpGet("byDepartment/{id}")]
        //[AllowAnonymous]
        [Route("{id:int}")]
        public async Task<IActionResult> GetByDepartment([FromRoute] int id)
        {

            //var byDep = await _dbContext._idea
            //    .Where(i => i.User.DepartmentID == id)       
            //    .ToListAsync();
            var result = await (from user in _dbContext._users
                    where user.DepartmentID == id
                    join idea in _dbContext._idea on user.UserId equals idea.UserId into userIdeas
                    from userIdea in userIdeas.DefaultIfEmpty()
                    join evt in _dbContext._events on userIdea.EvId equals evt.Id
                    join cate in _dbContext._categories on userIdea.CateId equals cate.Id
                    join dep in _dbContext._departments on user.DepartmentID equals dep.DepId
                    select new
                    {
                        Id = userIdea.Id,
                        Name = userIdea.Name,
                        Content = userIdea.Content,
                        IsAnonymous = userIdea.IsAnonymous,
                        AddedDate = userIdea.AddedDate,
                        Vote = userIdea.Vote,
                        Viewed = userIdea.Viewed,
                        IdeaFile = userIdea.IdeaFile,
                        EvId = userIdea.EvId,
                        CateId = userIdea.CateId,
                        UserName = user.UserName,
                        Avatar = user.Avatar,
                        EventName = evt.Name,
                        EventFirstClosure = evt.First_Closure,
                        EventLastClosure = evt.Last_Closure,
                        CategoryName = cate.Name,
                        DepartmentName = dep.Name
                    }).ToListAsync();
            return Ok(result);
        }
        [HttpGet("byCategory/{id}")]
        [Route("{id:int}")]
        public async Task<IActionResult> GetByCate([FromRoute] int id)
        {
            var result = await (from user in _dbContext._users
                                join idea in _dbContext._idea on user.UserId equals idea.UserId
                                join evt in _dbContext._events on idea.EvId equals evt.Id
                                join cate in _dbContext._categories on idea.CateId equals cate.Id
                                join dep in _dbContext._departments on user.DepartmentID equals dep.DepId
                                where cate.Id == id
                                select new
                                {
                                    Id = idea.Id,
                                    Name = idea.Name,
                                    Content = idea.Content,
                                    IsAnonymous = idea.IsAnonymous,
                                    AddedDate = idea.AddedDate,
                                    Vote = idea.Vote,
                                    Viewed = idea.Viewed,
                                    IdeaFile = idea.IdeaFile,
                                    EvId = idea.EvId,
                                    CateId = idea.CateId,
                                    UserName = user.UserName,
                                    Avatar = user.Avatar,
                                    EventName = evt.Name,
                                    EventFirstClosure = evt.First_Closure,
                                    EventLastClosure = evt.Last_Closure,
                                    CategoryName = cate.Name,
                                    DepartmentName = dep.Name
                                }).ToListAsync();
            return Ok(result);
        }

        [HttpGet("sort")]
        public async Task<IActionResult> Sort(string sortType)
        {
            try
            {
                //thêm người đăng + avatar
                var ideas = from i in _dbContext._idea
                            join u in _dbContext._users on i.UserId equals u.UserId
                            join dep in _dbContext._departments on u.DepartmentID equals dep.DepId

                            select new
                            {
                                Id = i.Id,
                                Name = i.Name,
                                Content = i.Content,
                                Anonymous = i.IsAnonymous,
                                AddedDate = i.AddedDate,
                                Vote = i.Vote,
                                Viewed = i.Viewed,
                                IdeaFile = i.IdeaFile,
                                EvId = i.EvId,
                                CateId = i.CateId,
                                UserId = i.UserId,
                                UserName = u.UserName,
                                Avatar = u.Avatar,
                                DepartmentName = dep.Name
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
                    fileName = submiter.UserName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + idea.IdeaFile.FileName;
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
                    IdeaFile = fileName,
                    IsAnonymous = idea.IsAnonymous
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
        [HttpGet("download/{filename}")]
        [AllowAnonymous]
        [Route("filename:string")]

        public async Task<IActionResult> DownloadFile(string filename)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), _env.WebRootPath + "\\Idea\\", filename);
            if (!System.IO.File.Exists(filePath))
            {
                return BadRequest();
            }
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            return new FileStreamResult(fileStream, "application/pdf")
            {
                FileDownloadName = "idea.pdf"
            };
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

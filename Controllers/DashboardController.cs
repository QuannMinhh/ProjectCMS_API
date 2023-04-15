using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Scripting.Utils;
using ProjectCMS.Data;
using ProjectCMS.ViewModels.Dashboard;
using System.Data;

namespace ProjectCMS.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public DashboardController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("Count")]
        public async Task<IActionResult> GetCountAll()
        {
            var ideaCount = (await _dbContext._idea.ToListAsync()).Count();
            var eventCount = (await _dbContext._events.ToListAsync()).Count();
            var cateCount = (await _dbContext._categories.ToListAsync()).Count();
            var userCount = (await _dbContext._users.ToListAsync()).Count();

            return Ok(new { idea = ideaCount, even = eventCount, cate = cateCount, user = userCount });
        }
        [HttpGet("IdeaPopular")]
        public async Task<IActionResult> GetMostPopularIdea()
        {
            
            var popularIdeas = await _dbContext._idea
                                .Select(i => new { i.Id, i.Name, i.Vote })
                                .OrderByDescending(i => i.Vote)
                                .ToListAsync();
            return Ok(popularIdeas);
        }
        [HttpGet("IdeaPerCate")]
        public async Task<IActionResult> GetIdeaPerCate()
        {
            var categories = await _dbContext._categories.Select(c => new { c.Id, c.Name }).ToListAsync();
            List<IdeaPerCate> ideaPerCate = new List<IdeaPerCate>();
            foreach (var c in categories)
            {
                var ideas = (await _dbContext._idea.Where(i => i.CateId== c.Id).ToListAsync()).Count;
                IdeaPerCate newIpC = new()
                {
                    CateName = c.Name,
                    Ideas = ideas,
                };
                ideaPerCate.Add(newIpC);
            }
            return Ok(ideaPerCate);
        }
        [HttpGet("IdeaPerYear")]
        public async Task<IActionResult> GetIdeaPerYear()
        {
            var dep = await _dbContext._departments.ToListAsync();  
            var year = await _dbContext._idea.Select(i => i.AddedDate.Year).Distinct().ToListAsync();
            List<Result> results= new List<Result>();
            
            foreach (var y in year)
            {
                List<IdeaPerDep> ideaPerDeps = new List<IdeaPerDep>();
                foreach (var d in dep)
                {
                    var ideas = from department in _dbContext._departments
                               join users in _dbContext._users on department.DepId equals users.DepartmentID
                               join idea in _dbContext._idea on users.UserId equals idea.UserId
                               where d.DepId == department.DepId && y == idea.AddedDate.Year  
                               select new
                               {
                                   Id = idea.Id,
                               };
                   
                    ideaPerDeps.Add(new IdeaPerDep { DepName = d.Name , Ideas = ideas.Count()})  ;
                }
               
                results.Add(new Result
                {
                    Year = y,
                    iderPerDeps = ideaPerDeps
                });
            }


           
            return Ok(results.ToList());
        }
        //
        //[HttpGet("Contributor")]
        //public async Task<IActionResult> Contributor()
        //{
        //    var deps = await _dbContext._departments.ToListAsync();

        //    foreach (var d in deps)
        //    {
        //        var users = from user in _dbContext._users where user.DepartmentID == d.DepId                        
        //    }
        //}
    }
    public class Result
    {
        public int Year { get; set; }
        public List<IdeaPerDep> iderPerDeps { get; set; }
    }
    public class IdeaPerDep
    {
        public string DepName { get; set; }
        public int Ideas { get; set; }
    }

    public class IdeaPerUser
    {
        public string UserName { get; set; }
    }
}

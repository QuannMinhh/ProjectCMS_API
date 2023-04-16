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
                    var ideas =  _dbContext._idea.Count(idea => idea.User.DepartmentID == d.DepId && idea.AddedDate.Year == y);                  
                    ideaPerDeps.Add(new IdeaPerDep { DepName = d.Name , Ideas = ideas})  ;
                }             
                results.Add(new Result
                {
                    Year = y,
                    iderPerDeps = ideaPerDeps
                });
            } 
            return Ok(results.ToList());
        }

        [HttpGet("Contributor")]
        public async Task<IActionResult> Contributor()
        {
            List<Result2> results = new List<Result2>();
            var deps = await _dbContext._departments.ToListAsync();

            foreach (var d in deps)
            {
                
                var users =  _dbContext._users.Count(user => user.DepartmentID == d.DepId);
                var ideas = _dbContext._idea.Count(idea => idea.User.DepartmentID == d.DepId);
                IdeaUser ideaUser = new IdeaUser { Users = users, Ideas = ideas };
                results.Add(new Result2
                {
                    Department = d.Name,
                    iderPerUsers = ideaUser
                }) ;
            }
            return Ok(results);
        }
        
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
    public class Result2
    {
        public string Department { get; set; }
        public IdeaUser iderPerUsers { get; set; }
    }

    public class IdeaUser
    {
        public int Users { get; set; }
        public int Ideas { get; set;}
    }
}

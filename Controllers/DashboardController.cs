using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var userCount = (await _dbContext.Users.ToListAsync()).Count();

            return Ok(new { idea = ideaCount, even = eventCount, cate = cateCount, user = userCount });
        }
        [HttpGet("IdeaPopular")]
        public async Task<IActionResult> GetMostPopularIdea()
        {
            var popularIdeas = await _dbContext._idea
                                .Select(i => new { i.Id, i.Name, i.Vote })
                                .OrderBy(i => i.Vote)
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

        //
    }
}

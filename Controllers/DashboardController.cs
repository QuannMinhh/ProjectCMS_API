using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCMS.Data;
using ProjectCMS.ViewModels.Dashboard;

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
        [HttpGet]
        public async Task<IActionResult> GetAllInfomation()
        {
            var ideaCount = (await _dbContext._idea.ToListAsync()).Count();
            var eventCount = (await _dbContext._events.ToListAsync()).Count();
            var cateCount = (await _dbContext._categories.ToListAsync()).Count();
            var userCount = (await _dbContext.Users.ToListAsync()).Count();

            return Ok(new { idea = ideaCount, even = eventCount, cate = cateCount, user = userCount });
        }
        [HttpGet]
        public async Task<IActionResult> GetMostPopularIdea()
        {
            var popularIdeas = await _dbContext._idea
                                .Select(i => new { i.Id, i.Name, i.Vote })
                                .ToListAsync();
            return Ok(popularIdeas);
        }

        public async Task<IActionResult> GetIdeaPerCate()
        {
            var cateId = await _dbContext._categories.Select(c => new { c.Id, c.Name }).ToListAsync();
            List<IdeaPerCate> ideaPerCate;
            foreach (var c in cateId)
            {

            }

            return Ok();
        }

        //
    }
}

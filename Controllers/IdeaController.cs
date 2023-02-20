using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCMS.Data;
using ProjectCMS.Models;
using ProjectCMS.ViewModels;

namespace ProjectCMS.Controllers
{
    [Route("api/idea")]
    [ApiController]
    public class IdeaController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public IdeaController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetIdeas()
        {
            List<Idea> ideas = await _dbContext._idea.ToListAsync();
            return Ok(ideas);
        }

        [HttpPost]
        public async Task<IActionResult> CreateIdea(IdeaViewModel idea)
        {
            if (ModelState.IsValid)
            {
                Idea newIdea = new()
                {
                    Title = idea.Title,
                    Content = idea.Content,
                    //EvId = idea.eId
                };
                await _dbContext._idea.AddAsync(newIdea);
                await _dbContext.SaveChangesAsync();

                return Ok(newIdea);
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
                return Ok();
            }

            return NotFound();
        }

    }
}

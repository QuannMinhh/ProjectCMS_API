using Duende.IdentityServer.Extensions;
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
                    return NotFound("Cannot find any idea like " + searchString);
                }

                if (ideas.Any())
                {
                    return Ok(await ideas.ToListAsync());
                }
                return NotFound();

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
                    Vote= idea.Vote,
                    Viewed = idea.Viewed,
                    AddedDate= idea.SubmitedDate,
                    EvId = idea.eId,
                    CateId = idea.cId
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

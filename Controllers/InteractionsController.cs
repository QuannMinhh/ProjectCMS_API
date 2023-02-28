using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCMS.Data;
using ProjectCMS.Models;
using ProjectCMS.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectCMS.Controllers
{
    [Route("api/interactions")]
    [ApiController]
    public class InteractionsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public InteractionsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        [HttpPost]
        public async Task<IActionResult> GetInterac(InteractionsViewModel interactions)
        {
            if(ModelState.IsValid)
            {
                Interactions interac = (from i in _dbContext._interactions select i)
                    .Where(s => s.UserId == interactions.UserId && s.IdeaId == interactions.IdeaId)
                    .ToArray()[0];
                if (interac != null)
                {
                    return Ok(interac);
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        Interactions newInterac = new()
                        {
                            IdeaId = interactions.IdeaId,
                            UserId = interactions.UserId,
                            Voted = false,
                            Viewed = true,
                            Vote = false
                        };
                        await _dbContext._interactions.AddAsync(newInterac);
                        _dbContext.SaveChanges();

                        return Ok(newInterac);
                    }
                    return BadRequest();
                }
            }
            return BadRequest();            
        }

        
        [HttpPut]
        public async Task<IActionResult> EditInterac(int id,  EditInteractionModel rq)
        {
            var interac = await _dbContext._interactions.FindAsync(id);
            if (interac != null)
            {
                var idea = await _dbContext._idea.FindAsync(rq.IdeaId);
               if(interac.Voted == false)
                {
                    interac.Voted = true;
                    if (rq.Vote == true)
                    {
                        interac.Vote = rq.Vote;
                        idea.Vote += 1;
                    }
                }               
            }

            return NotFound(new
            {
                message = "Cannot find your interaction"
            });
            
        }
    }
}

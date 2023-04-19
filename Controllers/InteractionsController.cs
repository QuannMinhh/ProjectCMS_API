using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCMS.Data;
using ProjectCMS.Models;
using ProjectCMS.ViewModels;
using System.Text.Json;
using System.Text.Json.Serialization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectCMS.Controllers
{
    public class InteracResult
    {
        public int InteracId { get; set; }
        public int UserId { get; set; }
        public int IdeaId { get; set; }
        public bool Voted { get; set; }
        public bool Viewed { get; set; }
        public bool Vote { get; set; }
    }
    [Route("api/interactions")]
    [ApiController]
    [Authorize]
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
                var interac = await (from i in _dbContext._interactions select i)
                    .FirstOrDefaultAsync(s => s.UserId == interactions.UserId && s.IdeaId == interactions.IdeaId);

                if (interac != null)
                {
                    return Ok(interac);
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        var idea = await _dbContext._idea.FindAsync(interactions.IdeaId);
                        if (idea != null)
                        {
                            idea.Viewed += 1;
                            await _dbContext.SaveChangesAsync();
                        }

                        Interactions newInterac = new()
                        {
                            IdeaId = interactions.IdeaId,
                            UserId = interactions.UserId,
                            Voted = false,
                            Viewed = true,
                            Vote = false,

                        };                      
                        await _dbContext._interactions.AddAsync(newInterac);
                        await _dbContext.SaveChangesAsync();
                        
                        InteracResult result = new InteracResult
                        {
                            InteracId = newInterac.InteracId,
                            UserId = newInterac.UserId,
                            IdeaId = newInterac.IdeaId,
                            Voted = false,
                            Viewed = true,
                            Vote = false
                        };
                        return Ok(result);
                    }
                    return BadRequest();
                }
            }
            return BadRequest();            
        }

        
        [HttpPut]
        public async Task<IActionResult> EditInterac(EditInteractionModel rq)
        {
            var interac = await _dbContext._interactions.FindAsync(rq.InteractionId);
            if (interac != null)
            {
                var idea = await _dbContext._idea.FindAsync(interac.IdeaId);
                int vote = idea.Vote;
                bool voted = interac.Voted;
                bool status = false;
               if(interac.Voted == false)
                {
                    voted = true;
                    if (rq.Vote == true)
                    {
                        status = true;
                        vote += 1;
                    }
                    else
                    {

                        vote -= 1;
                    }
                }

               if(interac.Voted == true)
                {
                    if(interac.Vote == true)
                    {
                        if(rq.Vote == true)
                        {
                            vote -= 1;
                            voted = false;
                        }
                        if(rq.Vote == false)
                        {
                            vote -= 2;
                            voted = true;
                        }
                    }
                    if(interac.Vote == false)
                    {
                        if (rq.Vote == true)
                        {
                            vote += 2;
                            voted = true;
                            status = true;
                        }
                        if (rq.Vote == false)
                        {
                            vote += 1;
                            voted = false;
                        }
                    }
                }

                interac.Voted = voted;
                interac.Vote = status;
                idea.Vote = vote;

                await _dbContext.SaveChangesAsync();
                return Ok(new
                {
                    message = "Edit ok!" 
                });
            }

            return NotFound(new
            {
                message = "Cannot find your interaction"
            });
            
        }
    }
}

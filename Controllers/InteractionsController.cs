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
        // GET: api/<InteractionsController>
        [HttpPost]
        public async Task<IActionResult> GetInterac(GetInteracModel interactions)
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

        // POST api/<InteractionsController>
        //[HttpPost]
        //public async Task<IActionResult> CreateInterac([FromBody] InteractionsViewModel interactions)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        var interac = (from i in _dbContext._interactions select i)
        //            .Where(s => s.UserId == interactions.UserId && s.IdeaId == interactions.IdeaId);
        //        if (interac.Any())
        //        {
        //             return Ok("Interaction Exit!"); ;
        //        }
        //        else
        //        {
        //            Interactions newInterac = new()
        //            {
        //                IdeaId = interactions.IdeaId,
        //                UserId = interactions.UserId,
        //                Voted = interactions.Voted,
        //                Viewed = interactions.Viewed,
        //                Vote = interactions.Vote
        //            };
        //            await _dbContext._interactions.AddAsync(newInterac);
        //            await _dbContext.SaveChangesAsync();

        //            return Ok(newInterac.InteracId);
        //        }

        //    }
        //    return BadRequest();
        //}

        // PUT api/<InteractionsController>/5
        [HttpPut]
        public async Task<IActionResult> EditInterac(int id, bool like)
        {
            var interac = await _dbContext._interactions.FindAsync(id);
            if (interac != null)
            {
                              
            }

            return NotFound(new
            {
                message = "Cannot find your interaction"
            });
            
        }
    }
}

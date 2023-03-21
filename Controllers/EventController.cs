using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCMS.Data;
using ProjectCMS.Models;
using ProjectCMS.ViewModels;

namespace ProjectCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public EventController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        

        [HttpGet,Authorize]
        public async Task<IActionResult> GetEvent()
        {
            return Ok(await _dbContext._events.ToListAsync());
        }
        [HttpGet("EnventInDate"),AllowAnonymous]
        public async Task<IActionResult> EventIndate()
        {            
            var evt = await _dbContext._events.Where(e => e.First_Closure > DateTime.Now).ToListAsync();
            return Ok(evt);    
        }
        [HttpPost, Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateEvent(EventViewModel evt)
        {
            if (ModelState.IsValid)
            {
                Event newEvt = new()
                {
                    Name = evt.Name,
                    Content = evt.Content,
                    First_Closure = evt.First_Closure,
                    Last_Closure = evt.First_Closure.AddDays(7),
                };
                _dbContext._events.Add(newEvt);
                _dbContext.SaveChanges();
                return Ok(await _dbContext._events.ToListAsync());
            }
            return BadRequest();
        }
         
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetEvent([FromRoute] int id)
        {            
            return Ok(await _dbContext._events.FirstOrDefaultAsync(Evt => Evt.Id == id));
        }
        [HttpPut, Authorize(Roles = "Admin")]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateEvent(EventViewModel evtUpdate, [FromRoute] int id)
        {
            var evt = await _dbContext._events.FindAsync(id);
            if(evt != null)
            {
                if(ModelState.IsValid)
                {
                    evt.Name = evtUpdate.Name;
                    evt.Content = evtUpdate.Content;
                    evt.First_Closure = evtUpdate.First_Closure;
                    evt.Last_Closure = evtUpdate.First_Closure.AddDays(7);
                    _dbContext.SaveChanges();
                    return Ok(await _dbContext._events.ToListAsync());
                }
            }
                return BadRequest();
        }
    }
}

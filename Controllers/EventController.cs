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
            var listEvent = await _dbContext._events
                .Join(_dbContext._categories, _event => _event.CateId, _category => _category.Id, (_event, _category) => new EventCateViewModel
                {
                    Id = _event.Id,
                    Name = _event.Name,
                    Content = _event.Content,
                    First_Closure = _event.First_Closure,
                    Last_Closure = _event.Last_Closure,
                    CateId = _event.CateId,
                    CateName = _category.Name,

                }
                ).ToListAsync();
            return Ok(listEvent);
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
                    CateId = evt.CateId
                };
                _dbContext._events.Add(newEvt);
                _dbContext.SaveChanges();
                return Ok(await _dbContext._events.ToListAsync());
            }
            return BadRequest();
        }
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteEvent([FromRoute] int id)
        {
            var evt = await _dbContext._events.FindAsync(id);
            if (evt != null)
            {
                _dbContext._events.Remove(evt);
                await _dbContext.SaveChangesAsync();
                return Ok();
            }

            return NotFound();
        }
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetEvent([FromRoute] int id)
        {
            var Eventt = await _dbContext._events.Join(_dbContext._categories, _event => _event.CateId, _category => _category.Id, (_event, _category) => new EventCateViewModel
            {
                Id = _event.Id,
                Name = _event.Name,
                Content = _event.Content,
                First_Closure= _event.First_Closure,
                Last_Closure= _event.Last_Closure,
                CateId = _event.CateId,
                CateName = _category.Name,
            }).FirstOrDefaultAsync(Evt => Evt.Id == id);            
            return Ok(Eventt);
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
                    evt.CateId = evtUpdate.CateId;
                    _dbContext.SaveChanges();
                    return Ok(await _dbContext._events.ToListAsync());
                }
            }
                return BadRequest();
        }

    }
}

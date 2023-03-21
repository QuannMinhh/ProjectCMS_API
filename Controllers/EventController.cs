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
        public EventController() { }

        [HttpGet,Authorize]
        public async Task<IActionResult> GetEvent()
        {
            var listEvent = await _dbContext._events.ToListAsync();
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
                };
                _dbContext._events.Add(newEvt);
                _dbContext.SaveChanges();
                return Ok(await _dbContext._events.ToListAsync());
            }
            return BadRequest();
        }
        public  Task Timeout()
        {
            Task.Factory.StartNew(() =>
            {
                System.Threading.Thread.Sleep(86400000);
                CheckDateEvent();
            });
            return Task.CompletedTask;
        }
        public  async void CheckDateEvent()
        {
            var listEvent = await _dbContext._events.ToListAsync();
            foreach(var evt in listEvent) 
            {
                Console.WriteLine(evt.Name);
                if(evt.First_Closure < DateTime.Now)
                {
                    evt.First_IsOverDeadline = true;
                }
                if(evt.Last_Closure < DateTime.Now)
                {
                    evt.Second_IsOverDeadline = true;
                }
            }
                await _dbContext.SaveChangesAsync();            
        }
        
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetEvent([FromRoute] int id)
        {
            var Eventt = await _dbContext._events.FirstOrDefaultAsync(Evt => Evt.Id == id);            
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
                    evt.First_IsOverDeadline = evtUpdate.First_IsOverDeadline;
                    evt.Second_IsOverDeadline = evtUpdate.Second_IsOverDeadline;
                    _dbContext.SaveChanges();
                    return Ok(await _dbContext._events.ToListAsync());
                }
            }
                return BadRequest();
        }
    }
}

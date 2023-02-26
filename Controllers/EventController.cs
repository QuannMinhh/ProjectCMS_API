﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCMS.Data;
using ProjectCMS.Models;

namespace ProjectCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public EventController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetEvent()
        {
            List<Event> events = await _dbContext._events.ToListAsync();
            List<Category> cates = await _dbContext._categories.ToListAsync();
            return Ok(events);
        }
        [HttpPost]
        public async Task<ActionResult> CreateEvent(Event evt)
        {
            if (ModelState.IsValid)
            {
                Event newEvt = new Event();
                newEvt.Name = evt.Name;
                newEvt.First_Closure= evt.First_Closure;
                newEvt.Last_Closure= evt.Last_Closure;
                newEvt.CateId = evt.CateId;
                _dbContext._events.Add(newEvt);
                _dbContext.SaveChanges();
                return Ok(await _dbContext._events.ToListAsync());
            }
            return BadRequest();
        }
        [HttpDelete]
        [Route("id:int")]
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
        [HttpPut("id:int")]
        public async Task<IActionResult> UpdateEvent(Event rqEvt)
        {
            var evt = await _dbContext._events.FindAsync(rqEvt.EvId);
            if(evt == null)
            {
                return BadRequest();
            }
            evt.Name = rqEvt.Name;
            evt.CateId = rqEvt.CateId;
            evt.First_Closure = rqEvt.First_Closure;
            evt.Last_Closure= rqEvt.Last_Closure;
            _dbContext.SaveChanges();
            return Ok(await _dbContext._events.ToListAsync());
        }

    }
}

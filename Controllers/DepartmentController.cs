using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCMS.Data;
using ProjectCMS.Models;
using ProjectCMS.ViewModels;

namespace ProjectCMS.Controllers
{
    [Route("api/department")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public DepartmentController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext; 
        }
        [HttpGet]
        public async Task<IActionResult> GetDepartment()
        {
            return Ok(await _dbContext._departments.ToArrayAsync());
        }
        [HttpPost]
        public async Task<IActionResult> CreateDepartment(DepViewModel rqDep)
        {
            if(ModelState.IsValid)
            {
                Department newDep = new()
                {
                    Name = rqDep.DepName
                };

                await _dbContext.AddAsync(newDep);
                await _dbContext.SaveChangesAsync();

                return Ok(newDep);
            }
            return BadRequest();
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> EditDep([FromRoute] int id, DepViewModel rqDep)
        {
            if (ModelState.IsValid)
            {
                var editDep = await _dbContext._departments.FindAsync(id);
                if (editDep == null)
                {
                    return NotFound();
                }
                
                editDep.Name = rqDep.DepName;

                await _dbContext.SaveChangesAsync();

                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteDep([FromRoute] int id)
        {
            var deleteDep = await _dbContext._departments.FindAsync(id);
            if(deleteDep == null)
            {
                return NotFound();
            }

             _dbContext.Remove(deleteDep);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    } 
}

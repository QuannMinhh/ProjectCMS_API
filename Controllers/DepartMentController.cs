using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCMS.Data;
using ProjectCMS.Models;

namespace ProjectCMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartMentController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public DepartMentController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult> GetDep()
        {
            List<Department> Deps = await _dbContext._departments.ToListAsync();
            return Ok(Deps);
        }
    }
}

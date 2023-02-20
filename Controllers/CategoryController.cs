using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCMS.Data;
using ProjectCMS.Models;

namespace ProjectCMS.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory() 
        {
            List<Category> categories = await _dbContext._categories.ToListAsync();
            return Ok(categories);
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateCategory()
        //{

        //}
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCMS.Data;
using ProjectCMS.Models;
using ProjectCMS.ViewModels;

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

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryViewModel category)
        {
            if (ModelState.IsValid)
            {
                Category newCate = new Category();
                newCate.Name = category.Name;
                newCate.Content = category.Content;

                _dbContext._categories.Add(newCate);
                _dbContext.SaveChanges();
                return Ok(await _dbContext._categories.ToListAsync());
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _dbContext._categories.FindAsync(id);
            if(category.Ideas == null)
            {
                _dbContext._categories.Remove(category);
                _dbContext.SaveChanges();
                return Ok("Successfully deleted ~.~ ");
            }
            if(category.Ideas != null)
            {
                return BadRequest("Cannot delete ! This category has events.");
            }
            return NotFound("Can't find category");
        }
    }
}

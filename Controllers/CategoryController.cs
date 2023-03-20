using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectCMS.Data;
using ProjectCMS.Models;
using ProjectCMS.ViewModels;
using System.Security.Cryptography;

namespace ProjectCMS.Controllers
{
    [Route("api/category")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Get all categories
        [HttpGet]
        public async Task<IActionResult> GetAllCategories() 
        {
            List<Category> categories = await _dbContext._categories.ToListAsync();
            return Ok(categories);
        }


        // Create a category
        [HttpPost,Authorize(Roles = "Admin,QAM")]
        
        public async Task<IActionResult> CreateCategory(CategoryViewModel category)
        {
            if (ModelState.IsValid)
            {
                Category newCate = new Category();
                newCate.Name = category.Name;
                newCate.Content = category.Content;
                newCate.AddedDate = DateTime.Now;

                await _dbContext._categories.AddAsync(newCate);
                await  _dbContext.SaveChangesAsync();
                return Ok(await _dbContext._categories.ToListAsync());
            }
            return BadRequest(new {message = "Some value is not valid. Please retype the value." });
        }

        
        // return Ok(new { message = ""});

        // Get a category by id
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetCategory([FromRoute] int id)
        {
             return Ok(await _dbContext._categories.FindAsync(id));   
        }

        // Edit category
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> EditCategory(CategoryViewModel newCate, [FromRoute] int id)
        {
            var category = await _dbContext._categories.FindAsync(id);
            if (category != null)
            {
                if(ModelState.IsValid)
                {
                    category.Content = newCate.Content;
                    category.Name = newCate.Name;
                    await _dbContext.SaveChangesAsync();
                    return Ok(await _dbContext._categories.ToListAsync());
                }
                return BadRequest(new {message = "Some value is not valid. Please retype the value." });
            }
            return BadRequest(new {message = "Category does not exist." });
        }
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteCategory([FromForm] string pwd, [FromRoute] int id)
        {
            {
                var category = await _dbContext._categories.FindAsync(id);
                var ideas = await _dbContext._idea.Where(x => x.CateId == id).ToListAsync();
                if (category != null)
                {
                    if (!ideas.Any())
                    {
                        _dbContext._categories.Remove(category);
                        await _dbContext.SaveChangesAsync();
                        return Ok(await _dbContext._categories.ToListAsync());
                    }
                    return BadRequest(new { message = "Cannot delete! This category has ideas." });
                }
            }
            return NotFound(new { message = "Category does not exist." });
        }
        private bool Verify(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }





    }
}

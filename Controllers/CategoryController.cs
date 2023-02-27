﻿using Microsoft.AspNetCore.Http;
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

        // Get all categories
        [HttpGet]
        public async Task<IActionResult> GetAllCategories() 
        {
            List<Category> categories = await _dbContext._categories.ToListAsync();
            return Ok(categories);
        }


        // Create a category
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryViewModel category)
        {
            if (ModelState.IsValid)
            {
                Category newCate = new Category();
                newCate.Name = category.Name;
                newCate.Content = category.Content;
                newCate.AddedDate = category.AddedDate;

                await _dbContext._categories.AddAsync(newCate);
                await  _dbContext.SaveChangesAsync();
                return Ok(await _dbContext._categories.ToListAsync());
            }
            return BadRequest();
        }


        // Get a category by id
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetCategory([FromRoute] int id)
        {
             return Ok(await _dbContext._categories.FindAsync(id));
           
;        }


        // Delete category
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteCategory([FromRoute]  int id)
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
                return BadRequest("Cannot delete ! This category has ideas.");
            }
            return NotFound("Category does not exist !");
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
                    category.AddedDate = newCate.AddedDate;
                    category.Name = newCate.Name;
                    await _dbContext.SaveChangesAsync();
                    return Ok(await _dbContext._categories.ToListAsync());
                }
                return BadRequest();
            }
            return BadRequest("Can't find category !");
        }

   




    }
}

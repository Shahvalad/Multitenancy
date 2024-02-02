using firstApi.Cryptography;
using firstApi.Data;
using firstApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Formats.Asn1;

namespace firstApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly DataContextFactory _contextFactory;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(DataContext context, ILogger<CategoriesController> logger, DataContextFactory contextFactory)
        {
            _context = context;
            _logger = logger;
            _contextFactory = contextFactory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            _logger.LogInformation("Getting categories");
            using DataContext context = _contextFactory.CreateContext();
            List<Category> categories = await context.Categories.ToListAsync();
            return Ok(categories);
        }

        [HttpPost("Add")]
        public async Task<ActionResult<Category>> Create(Category category)
        {
            try
            {
                _logger.LogInformation($"Creating category: {category.CategoryName}");
                ArgumentNullException.ThrowIfNull(category);

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Category created successfully: {category.CategoryName}");
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating category: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost("Update")]
        public async Task<ActionResult<Category>> Edit(Category category)
        {
            try
            {
                var existingCategory = await _context.Categories.FindAsync(category.CategoryId);
                if (existingCategory == null)
                {
                    _logger.LogWarning($"Category not found for update with id {category.CategoryId}");
                    return NotFound($"No category found with id {category.CategoryId}");
                }

                _logger.LogInformation($"Updating category: {existingCategory.CategoryName}");
                existingCategory.CategoryName = category.CategoryName;
                existingCategory.Description = category.Description;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Category updated successfully: {existingCategory.CategoryName}");
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating category: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult> Remove(int? id)
        {
            try
            {
                if (id == null)
                {
                    _logger.LogWarning("Delete operation called with null id");
                    throw new ArgumentNullException("id is null");
                }

                var existingCategory = await _context.Categories.FindAsync(id);
                if (existingCategory == null)
                {
                    _logger.LogWarning($"Category not found for deletion with id {id}");
                    return NotFound($"No category found with id {id}");
                }

                _logger.LogInformation($"Deleting category: {existingCategory.CategoryName}");
                _context.Categories.Remove(existingCategory);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Category deleted successfully: {existingCategory.CategoryName}");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting category: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
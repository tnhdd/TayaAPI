using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TayaAPI.Data;
using TayaAPI.DTO;
using TayaAPI.Models;

namespace TayaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(MovementDbContext context) : ControllerBase
    {
        private readonly MovementDbContext _context = context;

        /// <summary>
        /// Retrieves all category records from the data store.
        /// </summary>
        /// <returns>An <see cref="ActionResult"/> containing a list of all categories if the operation is successful; otherwise,
        /// an error response with status code 500.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<Category>), StatusCodes.Status200OK)]  // 200 OK
        [ProducesResponseType(StatusCodes.Status404NotFound)]                   // 404 Not Found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        // 500 Internal Server Error
        public async Task<ActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _context.Categories.ToListAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the category that matches the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category to retrieve. Must be a positive integer.</param>
        /// <returns>An <see cref="ActionResult"/> containing the category with the specified ID if found; otherwise, a 404 Not
        /// Found response if no category exists with the given ID.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(List<Category>), StatusCodes.Status200OK)]  // 200 OK
        [ProducesResponseType(StatusCodes.Status404NotFound)]                   // 404 Not Found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]         // 500 Internal Server Error
        public async Task<ActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new category using the specified data transfer object.
        /// </summary>
        /// <param name="dto">The data transfer object containing the details of the category to create. Must not be null.</param>
        /// <returns>An <see cref="ActionResult"/> that represents the result of the create operation. Returns a 201 Created
        /// response with the created category if successful; otherwise, returns a 500 Internal Server Error if an
        /// exception occurs.</returns>
        [HttpPost]
        public async Task<ActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            try
            {
                var category = new Models.Category
                {
                    Name = dto.Name
                };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the details of an existing category with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category to update.</param>
        /// <param name="dto">An object containing the updated category information. Must not be null.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation. Returns <see cref="NoContentResult"/>
        /// if the update is successful; <see cref="NotFoundResult"/> if the category does not exist; or a server error
        /// result if an unexpected error occurs.</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(List<Category>), StatusCodes.Status200OK)]  // 200 OK
        [ProducesResponseType(StatusCodes.Status404NotFound)]                   // 404 Not Found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        // 500 Internal Server Error
        public async Task<ActionResult> UpdateCategory(int id, [FromBody] CreateCategoryDto dto)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }
                category.Name = dto.Name;
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes the category with the specified identifier if it exists and is not associated with any movements.
        /// </summary>
        /// <remarks>This action does not delete categories that are referenced by existing movements.
        /// Attempting to delete such a category will result in a 400 Bad Request response. If an unexpected error
        /// occurs, a 500 Internal Server Error response is returned.</remarks>
        /// <param name="id">The unique identifier of the category to delete.</param>
        /// <returns>A 204 No Content response if the category is successfully deleted; a 404 Not Found response if the category
        /// does not exist; or a 400 Bad Request response if the category is associated with existing movements.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(List<Category>), StatusCodes.Status200OK)]  // 200 OK
        [ProducesResponseType(StatusCodes.Status404NotFound)]                   // 404 Not Found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        // 500 Internal Server Error
        public async Task<ActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }
                var hasMovements = await _context.Movements.AnyAsync(m => m.CategoryId == id);
                if (hasMovements)
                {
                    return BadRequest("Cannot delete category because it is associated with existing movements.");
                }
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

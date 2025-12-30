using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TayaAPI.Data;
using TayaAPI.DTO;
using TayaAPI.DTO.Response_Models;
using TayaAPI.Models;

namespace TayaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovementsController : ControllerBase
    {
        private readonly MovementDbContext _context;    
        public MovementsController(MovementDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Category>), StatusCodes.Status200OK)]  // 200 OK
        [ProducesResponseType(StatusCodes.Status404NotFound)]                   // 404 Not Found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        // 500 Internal Server Error
        public async Task<ActionResult<IEnumerable<Movement>>> GetAllMovements(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int? categoryId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.Movements
                .Include(m => m.Category)
                .AsQueryable(); 
            if (startDate.HasValue)
            {
                query = query.Where(m => m.OperationDate >= startDate.Value);   
            }
            if (endDate.HasValue)
            {
                query = query.Where(m => m.OperationDate <= endDate.Value);
            }
            if (categoryId.HasValue)
            {
                query = query.Where(m => m.CategoryId == categoryId.Value);
            }

            var totalCount = await query.CountAsync();

            var movements = await query
                .OrderByDescending(m => m.OperationDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = new
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Movements = movements
            };

            return Ok(response);
        }

        [HttpGet("summary")]
        [ProducesResponseType(typeof(List<Category>), StatusCodes.Status200OK)]  // 200 OK
        [ProducesResponseType(StatusCodes.Status404NotFound)]                   // 404 Not Found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        // 500 Internal Server Error
        public async Task<ActionResult> GetMovementsSummary(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int? categoryId)
        {
            var query = _context.Movements.AsQueryable();
            if (startDate.HasValue)
            {
                query = query.Where(m => m.OperationDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(m => m.OperationDate <= endDate.Value);
            }
            if (categoryId.HasValue)
            {
                query = query.Where(m => m.CategoryId == categoryId.Value);
            }

            var movements = await query.ToListAsync();

            var summary = new MovementSummary
            {
                TotalMovements = movements.Count,
                TotalIncome = movements.Where(m => m.Amount > 0).Sum(m => m.Amount),
                TotalExpenses = Math.Abs(movements.Where(m => m.Amount < 0).Sum(m => m.Amount))
            };

            return Ok(summary);
        }

        [HttpGet]
        [Route("category/{category}")]
        [ProducesResponseType(typeof(List<Category>), StatusCodes.Status200OK)]  // 200 OK
        [ProducesResponseType(StatusCodes.Status404NotFound)]                   // 404 Not Found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        // 500 Internal Server Error
        public async Task<ActionResult<IEnumerable<Movement>>> GetMovementsByCategory(string category)
        {
            try
            {
                var filteredMovements = await _context.Movements
                    .Include(m => m.Category)
                    .Where(m => m.Category.Name.ToLower() == category.ToLower())
                    .ToListAsync();

                if (filteredMovements.Count == 0)
                {
                    return NotFound($"No movements found for category: {category}");
                }
                return Ok(filteredMovements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(List<Category>), StatusCodes.Status200OK)]  // 200 OK
        [ProducesResponseType(StatusCodes.Status404NotFound)]                   // 404 Not Found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        // 500 Internal Server Error
        public async Task<ActionResult<Movement>> CreateMovement([FromBody] CreateMovementDto dto)
        {
            try
            {
               if(dto.Amount == 0)
               {
                    return BadRequest("Amount cannot be zero.");
               }
                // Verify that the category exists
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
                if (!categoryExists)
                {
                    return BadRequest($"Category with ID {dto.CategoryId} does not exist.");
                }

                var newMovement = new Movement
                {
                    Id = Guid.NewGuid(),
                    OperationDate = dto.OperationDate,
                    ValueDate = dto.ValueDate,
                    Amount = dto.Amount,
                    Description = dto.Description,
                    CategoryId = dto.CategoryId
                };

                _context.Movements.Add(newMovement);
                await _context.SaveChangesAsync();
                await _context.Entry(newMovement).Reference(m => m.Category).LoadAsync();

                return CreatedAtAction(nameof(GetAllMovements), new { id = newMovement.Id }, newMovement);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(List<Category>), StatusCodes.Status200OK)]  // 200 OK
        [ProducesResponseType(StatusCodes.Status404NotFound)]                   // 404 Not Found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        // 500 Internal Server Error
        public async Task<ActionResult<Movement>> UpdateMovement(Guid id, [FromBody] UpdateMovementDto dto)
        {
            try
            {
                var existingMovement = await _context.Movements.FindAsync(id);
                if (existingMovement == null)
                {
                    return NotFound($"Movement with Id {id} not found.");
                }
                // Verify that the category exists
                if(dto.CategoryId != existingMovement.CategoryId) 
                {
                    var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
                    if (!categoryExists)
                    {
                        return BadRequest($"Category with ID {dto.CategoryId} does not exist.");
                    }
                }

                existingMovement.OperationDate = dto.OperationDate;
                existingMovement.ValueDate = dto.ValueDate;
                existingMovement.Amount = dto.Amount;
                existingMovement.Description = dto.Description;
                existingMovement.CategoryId = dto.CategoryId;

                await _context.SaveChangesAsync();

                await _context.Entry(existingMovement).Reference(m => m.Category).LoadAsync();

                return Ok(existingMovement);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(List<Category>), StatusCodes.Status200OK)]  // 200 OK
        [ProducesResponseType(StatusCodes.Status404NotFound)]                   // 404 Not Found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]        // 500 Internal Server Error
        public async Task<ActionResult<Movement>> DeleteMovement(Guid id)
        {
            var movementToDelete = await _context.Movements.FindAsync(id);
            if (movementToDelete == null)
            {
                return NotFound($"Movement with Id {id} not found.");
            }
            _context.Remove(movementToDelete);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

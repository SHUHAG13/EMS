using EMS.API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DesignationController:ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public DesignationController(EmployeeDbContext context)
        {
            _context = context;
        }

        //  GET: api/Designation
        [HttpGet]
        public async Task<IActionResult> GetDesignations()
        {
            try
            {
                var designations = await _context.Designation.ToListAsync();
                return Ok(designations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }



        }

        //  GET: api/Designation/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDesignationById(int id)
        {
            try
            {
                var designation = await _context.Designation
                    .FirstOrDefaultAsync(d => d.DesignationId == id);

                if (designation == null)
                    return NotFound("Designation not found");

                return Ok(designation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> CreateDesignation([FromBody]Designation designation)
        {
            try
            {
                _context.Designation.Add(designation);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetDesignationById),
                    new { id = designation.DesignationId }, designation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDesignation(int id, [FromBody]Designation designation)
        {
            try
            {
                if (id != designation.DesignationId)
                    return BadRequest("Designation ID mismatch");

                var existingDesignation = await _context.Designation.FindAsync(id);

                if (existingDesignation == null)
                    return NotFound("Designation not found");

                existingDesignation.DesignationName = designation.DesignationName;
                existingDesignation.DepartmentId = designation.DepartmentId;

                await _context.SaveChangesAsync();

                return Ok(existingDesignation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //  DELETE: api/Designation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDesignation(int id)
        {
            try
            {
                var designation = await _context.Designation.FindAsync(id);

                if (designation == null)
                    return NotFound("Designation not found");

                _context.Designation.Remove(designation);
                await _context.SaveChangesAsync();

                return Ok("Designation deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

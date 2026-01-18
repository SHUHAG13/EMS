using EMS.API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public DepartmentController(EmployeeDbContext context)
        {
            _context = context;
        }

       
        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            try
            {
                var departments = await _context.Department.ToListAsync();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
           
        }


       
        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] Department department)
        {
            try
            {
                _context.Department.Add(department);
                await _context.SaveChangesAsync();
                return Ok("Department added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }


        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment([FromBody] Department department)
        { 

            var existingDepartment = await _context.Department.FindAsync(department.DepartmentId);

            if (existingDepartment == null)
                return NotFound("Department not found");
            try 
            {
                existingDepartment.DepartmentName = department.DepartmentName;
                existingDepartment.IsActive = department.IsActive;
                await _context.SaveChangesAsync();
                return Ok("Department updated successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }




        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);

            if (department == null)
                return NotFound("Department not found");

            try
            {
                _context.Department.Remove(department);
                await _context.SaveChangesAsync();
                return Ok("Department deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }



    }
}

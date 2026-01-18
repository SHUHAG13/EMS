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

        // 🔹 GET: api/Department
        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _context.Department.ToListAsync();
            return Ok(departments);
        }


        // 🔹 POST: api/Department
        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] Department department)
        {
            _context.Department.Add(department);
            await _context.SaveChangesAsync();
            return Ok("Department added successfully");
        }


        // 🔹 PUT: api/Department/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment([FromBody] Department department)
        { 

            var existingDepartment = await _context.Department.FindAsync(department.DepartmentId);

            if (existingDepartment == null)
                return NotFound("Department not found");

            existingDepartment.DepartmentName = department.DepartmentName;
            existingDepartment.IsActive = department.IsActive;

            await _context.SaveChangesAsync();

            return Ok("Department updated successfully!");
        }


        // 🔹 DELETE: api/Department/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);

            if (department == null)
                return NotFound("Department not found");

            _context.Department.Remove(department);
            await _context.SaveChangesAsync();
            return Ok("Department deleted successfully");
        }



    }
}

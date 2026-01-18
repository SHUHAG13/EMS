using EMS.API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public EmployeeController(EmployeeDbContext context)
        {
            _context = context;
        }


        // =====================================================
        // 🔹 NORMAL GET: api/Employee
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var employees = await _context.Employee.ToListAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =====================================================
        // 🔹 ADVANCED GET: Filter + Sort + Pagination
        // api/Employee/search?name=a&city=dhaka&sortBy=name&sortDir=asc&page=1&pageSize=10
        // =====================================================
        [HttpGet("search")]
        public async Task<IActionResult> SearchEmployees(
            string? name,
            string? city,
            string? sortBy = "name",
            string? sortDir = "asc",
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.Employee.AsQueryable();

                // 🔍 Filtering
                if (!string.IsNullOrEmpty(name))
                    query = query.Where(e => e.Name.Contains(name));

                if (!string.IsNullOrEmpty(city))
                    query = query.Where(e => e.City.Contains(city));

                // 🔃 Sorting
                query = sortBy.ToLower() switch
                {
                    "city" => sortDir == "desc"
                        ? query.OrderByDescending(e => e.City)
                        : query.OrderBy(e => e.City),

                    "createddate" => sortDir == "desc"
                        ? query.OrderByDescending(e => e.CreatedDate)
                        : query.OrderBy(e => e.CreatedDate),

                    _ => sortDir == "desc"
                        ? query.OrderByDescending(e => e.Name)
                        : query.OrderBy(e => e.Name)
                };

                // 📄 Pagination
                var totalRecords = await query.CountAsync();
                var employees = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new
                {
                    TotalRecords = totalRecords,
                    Page = page,
                    PageSize = pageSize,
                    Data = employees
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =====================================================
        // 🔹 GET BY ID
        // =====================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                var employee = await _context.Employee.FindAsync(id);

                if (employee == null)
                    return NotFound("Employee not found");

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =====================================================
        // 🔹 CREATE
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody]Employee employee)
        {
            try
            {
                // 🔐 Unique ContactNo & Email
                bool exists = await _context.Employee.AnyAsync(e =>
                    e.ContactNo == employee.ContactNo ||
                    e.Email == employee.Email);

                if (exists)
                    return BadRequest("Contact number or Email already exists");

                employee.CreatedDate = DateTime.Now;

                _context.Employee.Add(employee);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetEmployeeById),
                    new { id = employee.EmployeeId }, employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =====================================================
        // 🔹 UPDATE
        // =====================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id,[FromBody] Employee employee)
        {
            try
            {
                if (id != employee.EmployeeId)
                    return BadRequest("Employee ID mismatch");

                var existingEmployee = await _context.Employee.FindAsync(id);

                if (existingEmployee == null)
                    return NotFound("Employee not found");

                // 🔐 Unique check (exclude current employee)
                bool exists = await _context.Employee.AnyAsync(e =>
                    (e.ContactNo == employee.ContactNo || e.Email == employee.Email)
                    && e.EmployeeId != id);

                if (exists)
                    return BadRequest("Contact number or Email already exists");

                existingEmployee.Name = employee.Name;
                existingEmployee.ContactNo = employee.ContactNo;
                existingEmployee.AltContactNo = employee.AltContactNo;
                existingEmployee.Email = employee.Email;
                existingEmployee.City = employee.City;
                existingEmployee.Pincode = employee.Pincode;
                existingEmployee.Address = employee.Address;
                existingEmployee.DesignationId = employee.DesignationId;
                existingEmployee.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(existingEmployee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =====================================================
        // 🔹 DELETE
        // =====================================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employee = await _context.Employee.FindAsync(id);

                if (employee == null)
                    return NotFound("Employee not found");

                _context.Employee.Remove(employee);
                await _context.SaveChangesAsync();

                return Ok("Employee deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }




}

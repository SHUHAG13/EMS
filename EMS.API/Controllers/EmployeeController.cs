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

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                var employee = await _context.Employee.FirstOrDefaultAsync(e => e.Email == loginDto.Email && e.Password == loginDto.Password);

                if (employee == null)
                    return Unauthorized("Invalid email or password");

                return Ok(new {
                Message = "Login successful",
                Data=new
                {
                    employee.EmployeeId,
                    employee.Name,
                    employee.Email,
                    employee.DesignationId,
                    employee.Role
                }  
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
   



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




        [HttpGet("search")]
        public async Task<IActionResult> SearchEmployees(string? name,string? city, string? sortBy = "name",string? sortDir = "asc",int page = 1,int pageSize = 10)
        {
            try
            {
                var query = _context.Employee.AsQueryable();

                // 🔍 Filtering
                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(e => e.Name.Contains(name));
                }

                if (!string.IsNullOrEmpty(city))
                {
                    query = query.Where(e => e.City.Contains(city));
                }

                // 🔃 Sorting (if-else version)
                if (sortBy.ToLower() == "city")
                {
                    if (sortDir.ToLower() == "desc")
                        query = query.OrderByDescending(e => e.City);
                    else
                        query = query.OrderBy(e => e.City);
                }
                else if (sortBy.ToLower() == "createddate")
                {
                    if (sortDir.ToLower() == "desc")
                        query = query.OrderByDescending(e => e.CreatedDate);
                    else
                        query = query.OrderBy(e => e.CreatedDate);
                }
                else // default: name
                {
                    if (sortDir.ToLower() == "desc")
                        query = query.OrderByDescending(e => e.Name);
                    else
                        query = query.OrderBy(e => e.Name);
                }

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

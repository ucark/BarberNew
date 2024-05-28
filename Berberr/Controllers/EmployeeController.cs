using Barber.Models.DTO;
using Barber.Models.Request;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using Barber.Models.Update;

namespace Barber.Controllers
{
    [Route("API/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly BarberDbContext _context;

        public EmployeeController(BarberDbContext context)
        {
            _context = context;
        }

        [HttpGet("Get-Employees")]
        public IActionResult GetEmployees()
        {
            try
            {
                var employees = _context.Employees.ToList();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpGet("Get-Employees/{id}")]
        public IActionResult GetEmployeeById(int id)
        {
            try
            {
                var employee = _context.Employees.Find(id);
                if (employee == null)
                    return NotFound("Belirtilen kimlik numarasına sahip çalışan bulunamadı.");

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpPost("Create-Employees")]
        public IActionResult CreateEmployee([FromForm] EmployeeCreate employeeData)
        {
            if (employeeData == null)
                return BadRequest("Geçersiz veri: Çalışan verisi boş.");

            string newFileName = null;
            if (employeeData.EmployeeFile != null && employeeData.EmployeeFile.Length > 0)
            {
                newFileName = Guid.NewGuid().ToString() + ".jpg";

                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "EmployeePictures");
                var filePath = Path.Combine(folderPath, newFileName);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    employeeData.EmployeeFile.CopyTo(stream);
                }
            }

            var newEmployee = new Employees
            {
                BarberID = employeeData.BarberID,
                Name = employeeData.Name,
                LastName = employeeData.LastName,
                EmployeeUrl = newFileName != null ? Path.Combine("\\Pictures\\EmployeePictures", newFileName) : null
            };

            try
            {
                _context.Employees.Add(newEmployee);
                _context.SaveChanges();
                return Ok(newEmployee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message + " Inner Exception: " + ex.InnerException?.Message);
            }
        }


        [HttpPut("Update-Employees/{id}")]
        public IActionResult UpdateEmployee(int id, [FromForm] EmployeeUpdate employeeData)
        {
            var existingEmployee = _context.Employees.Find(id);
            if (existingEmployee == null)
                return NotFound("Belirtilen kimlik numarasına sahip çalışan bulunamadı.");

            existingEmployee.Name = employeeData.Name;
            existingEmployee.LastName = employeeData.LastName;

            // Kullanıcı bir dosya göndermişse ve dosya boş değilse
            if (employeeData.EmployeeFile != null && employeeData.EmployeeFile.Length > 0)
            {
                // Yeni dosya adı oluşturma
                var newFileName = Guid.NewGuid().ToString() + ".jpg";

                // Dosya yolu
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "EmployeePictures");
                var filePath = Path.Combine(folderPath, newFileName);

                // Dosyayı sunucuya kaydetme
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    employeeData.EmployeeFile.CopyTo(stream);
                }

                // Resmin URL'sini oluşturma
                var fileUrl = Path.Combine("\\Pictures\\EmployeePictures", newFileName);

                // Resim URL'sini güncelleme
                existingEmployee.EmployeeUrl = fileUrl;
            }

            try
            {
                _context.SaveChanges();
                return Ok(existingEmployee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }


        [HttpDelete("Delete-Employees")]
        public IActionResult DeleteEmployee(int id)
        {
            var existingEmployee = _context.Employees.Find(id);
            if (existingEmployee == null)
                return NotFound("Belirtilen kimlik numarasına sahip çalışan bulunamadı.");

            try
            {
                _context.Employees.Remove(existingEmployee);
                _context.SaveChanges();
                return Ok("Çalışan başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }
    }
}

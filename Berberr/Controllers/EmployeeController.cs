using Barber.Models.DTO;
using Barber.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Barber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class EmployeeController : ControllerBase
    {
        private readonly BarberDbContext _context;

        public EmployeeController(BarberDbContext context)
        {
            _context = context;
        }

        [HttpGet("get-employees")]
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

        [HttpGet("get-employees/{id}")]

        public IActionResult GetBarberById(int id)
        {
            try
            {
                var employees = _context.Employees.Find(id);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpPost("create-employees")]

        public IActionResult CreateEmployee([FromForm] EmployeeCreate employeeData)
        {
            // Veri doğrulaması
            if (employeeData == null)
            {
                return BadRequest("Geçersiz veri: Çalışan verisi boş.");
            }

            // Base64 kodlanmış resmi byte dizisine dönüştürme
            if (employeeData.Picture == null || employeeData.Picture.Length == 0)
            {
                return BadRequest("Geçersiz veri: Profil resmi yüklenmedi.");
            }

            byte[] pictureData;
            using (var memoryStream = new MemoryStream())
            {
                employeeData.Picture.CopyTo(memoryStream);
                pictureData = memoryStream.ToArray();
            }

            var newEmployee = new Employees
            {
                Name = employeeData.Name,
                LastName = employeeData.LastName,
                Picture = pictureData
            };

            try
            {
                _context.Employees.Add(newEmployee);
                _context.SaveChanges();
                return Ok(newEmployee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpPut("update-employees/{id}")]

        public IActionResult UpdateEmployee(int id, [FromBody] Employees employeeData)
        {
            var existingEmployee = _context.Employees.Find(id);
            if (existingEmployee == null)
            {
                return NotFound("Belirtilen kimlik numarasına sahip çalışan bulunamadı.");
            }
            existingEmployee.Name = employeeData.Name;
            existingEmployee.LastName = employeeData.LastName;
            existingEmployee.Picture = employeeData.Picture;

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

        [HttpDelete("delete-employees/{id}")]

        public IActionResult DeleteEmployee(int id)
        {
            var existingEmployee = _context.Employees.Find(id);
            if (existingEmployee == null)
            {
                return NotFound("Belirtilen kimlik numarasına sahip çalışan bulunamadı.");
            }

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


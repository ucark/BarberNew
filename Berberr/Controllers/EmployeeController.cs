using Barber.Models.DTO;
using Barber.Models.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
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

        [HttpPost("create-employees")]
        public IActionResult CreateEmployee([FromForm] EmployeeCreate employeeData)
        {
            // Veri doğrulaması
            if (employeeData == null)
                return BadRequest("Geçersiz veri: Çalışan verisi boş.");

            // Resim yolu doğrulama
            if (employeeData.PictureFile == null || employeeData.PictureFile.Length == 0)
                return BadRequest("Geçersiz veri: Profil resmi yüklenmedi.");

            // Yeni dosya adı oluşturma
            var newFileName = Guid.NewGuid().ToString() + ".jpg";

            // Dosya yolu
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "EmployeePictures");
            var filePath = Path.Combine(folderPath, newFileName);

            // Dosyayı sunucuya kaydetme
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                employeeData.PictureFile.CopyTo(stream);
            }

            // Resmin URL'sini oluşturma
            var fileUrl = Path.Combine("/EmployeePictures", newFileName);

            // Yeni çalışan oluşturma
            var newEmployee = new Employees
            {
                BarberID = employeeData.BarberID,
                Name = employeeData.Name,
                LastName = employeeData.LastName,
                PictureUrl = fileUrl // Resmin URL'sini kaydetme
            };

            try
            {
                _context.Employees.Add(newEmployee);
                _context.SaveChanges();
                return Ok(newEmployee);
            }
            catch (Exception ex)
            {
                // Hata durumunda dosyayı silme
                System.IO.File.Delete(filePath);
                return StatusCode(500, "Hata: " + ex.Message + " Inner Exception: " + ex.InnerException?.Message);
            }
        }

        [HttpPut("update-employees/{id}")]
        public IActionResult UpdateEmployee(int id, [FromBody] Employees employeeData)
        {
            var existingEmployee = _context.Employees.Find(id);
            if (existingEmployee == null)
                return NotFound("Belirtilen kimlik numarasına sahip çalışan bulunamadı.");

            existingEmployee.Name = employeeData.Name;
            existingEmployee.LastName = employeeData.LastName;
            existingEmployee.PictureUrl = employeeData.PictureUrl;

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
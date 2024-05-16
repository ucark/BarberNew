using Barber.Models.DTO;
using Barber.Models.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Barber.Controllers
{
    [Route("API/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly BarberDbContext _context;

        public ServiceController(BarberDbContext context)
        {
            _context = context;
        }
        [HttpPost("Create-Services")]
        public IActionResult CreateService([FromBody] ServiceRequest servicesRequest)
        {
            if (servicesRequest == null)
            {
                return BadRequest("Geçersiz veri. Hizmet Bilgisi Boş");
            }

            var newService = new Services
            {
                Name = servicesRequest.Name,
                BarberId = servicesRequest.BarberId,
                CategoryId = servicesRequest.CategoryId,
                Price = servicesRequest.Price,
            };

            try
            {
                _context.Services.Add(newService);
                _context.SaveChanges();
                return Ok(newService);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpGet("Get-Services")]
        public IActionResult GetServices()
        {
            try
            {
                var services = _context.Services.ToList();
                return Ok(services);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpGet("Get-Service/{id}")]
        public IActionResult GetService(int id)
        {
            try
            {
                var service = _context.Services.Find(id);
                if (service == null)
                    return NotFound("Belirtilen ID'ye sahip hizmet bulunamadı.");

                return Ok(service);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }
        [HttpPut("Update-Service/{id}")]
        public IActionResult UpdateService(int id, [FromBody] ServiceRequest serviceRequest)
        {
            var existingService = _context.Services.Find(id);
            if (existingService == null)
                return NotFound("Belirtilen ID'ye sahip hizmet bulunamadı.");

            existingService.Name = serviceRequest.Name;
            existingService.BarberId = serviceRequest.BarberId;
            existingService.CategoryId = serviceRequest.CategoryId;
            existingService.Price = serviceRequest.Price;

            try
            {
                _context.SaveChanges();
                return Ok(existingService);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpDelete("Delete-Service/{id}")]
        public IActionResult DeleteService(int id)
        {
            var existingService = _context.Services.Find(id);
            if (existingService == null)
                return NotFound("Belirtilen ID'ye sahip hizmet bulunamadı.");

            try
            {
                _context.Services.Remove(existingService);
                _context.SaveChanges();
                return Ok("Hizmet başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

    }
}

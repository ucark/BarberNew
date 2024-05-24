using Barber.Models.DTO;
using Barber.Models.Request;
using Barber.Models.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using Barber.Models.Update;

namespace Barber.Controllers
{
    [Route("API/[controller]")]
    [ApiController]
    public class BarberController : ControllerBase
    {
        private readonly BarberDbContext _context;
        private readonly TokenService _tokenService;
        private readonly JwtSettings _jwtSettings;
        public BarberController(BarberDbContext context, TokenService tokenService, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpGet("Get-Barbers")]
        public IActionResult GetBarbers()
        {
            try
            {
                var barbers = _context.Barbers.ToList();
                return Ok(barbers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpGet("Get-Barber/{id}")]
        public IActionResult GetBarberById(int id)
        {
            try
            {
                var barbers = _context.Barbers.Find(id);
                return Ok(barbers);
            }
            catch(Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpGet("Get-Barber-With-Employees/{id}")]
        public IActionResult GetBarberWithEmployees(int id)
        {
            try
            {
                var barber = _context.Barbers.Find(id);
                if (barber == null)
                    return NotFound("Belirtilen kimlik numarasına sahip berber bulunamadı.");

                var employees = _context.Employees.Where(e => e.BarberID == id).Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.LastName,
                    e.EmployeeUrl
                }).ToList();

                var result = new
                {
                    Barber = barber,
                    Employees = employees
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpPost("Create-Barbers")]
        public IActionResult CreateBarber([FromBody] BarberCreate request)
        {
            // Veri doğrulaması
            if (request == null)
                return BadRequest("Geçersiz veri: Berber verisi boş.");

            // Yeni berber oluşturma    
            var newBarber = new Barbers
            {
                Name = request.Name,
                LastName = request.LastName,
                UserName = request.UserName,
                WorkPlaceName = request.WorkPlaceName,
                Mail = request.Mail,
                Password = request.Password,
                Phone = request.Phone,
                City = request.City,
                District = request.District,
                Street = request.Street,
                BuildingNo = request.BuildingNo,
                DoorNumber = request.DoorNumber,
                TaxNo = request.TaxNo,
                BarberUrl = ""
            };

            try
            {
                _context.Barbers.Add(newBarber);
                _context.SaveChanges();
                return Ok(newBarber);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message + " Inner Exception: " + ex.InnerException?.Message);
            }
        }



        [HttpPost("Login")]
        public IActionResult Login([FromBody] Barber.Models.Request.LoginRequest loginData)
        {
            try
            {
                var user = _context.Barbers.FirstOrDefault(u => u.UserName == loginData.Username
                && u.Password == loginData.Password);
                if (user == null)
                {
                    return BadRequest("Kullanıcı adı veya şifresi hatalı.");
                }
                //kullanıcı doğrulandıysa JWT oluştur bakam.
                var token = _tokenService.GenerateJwtToken(
                    _jwtSettings.Issuer,
                    _jwtSettings.Audience,
                    _jwtSettings.ExpireMinutes,
                    user.Id.ToString(), //kullanıcı kimlik bilgisi
                    "Barber" //Kullanıcı rolü
                    );
                return Ok(new
                {
                    Token = token,
                    User = user.Id,
                    user.UserName,
                    user.City,
                    user.District,
                    user.Street
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Sunucu hatası: " + ex.Message);
            }
        }

        [HttpPut("Update-Barber")]
        public IActionResult UpdateBarber([FromForm] BarberUpdate request)
        {
            var existingBarber = _context.Barbers.Find(request.Id);
            if (existingBarber == null)
            {
                return NotFound("Belirtilen kimlik numarasına sahip bir berber bulunamadı.");
            }

            // Resim yolu doğrulama
            if (request.BarberFile == null || request.BarberFile.Length == 0)
                return BadRequest("Geçersiz veri: Profil resmi yüklenmedi.");

            // Yeni dosya adı oluşturma
            var newFileName = Guid.NewGuid().ToString() + ".jpg";

            // Dosya yolu
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "BarberPictures");
            var filePath = Path.Combine(folderPath, newFileName);

            // Resmin URL'sini oluşturma
            var fileUrl = Path.Combine("\\Pictures\\BarberPictures", newFileName);

            // Dosyayı sunucuya kaydetme
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                request.BarberFile.CopyTo(stream);
            }

            existingBarber.Name = request.Name;
            existingBarber.LastName = request.LastName;
            existingBarber.UserName = request.UserName;
            existingBarber.WorkPlaceName = request.WorkPlaceName;
            existingBarber.Mail = request.Mail;
            existingBarber.Password = request.Password;
            existingBarber.Phone = request.Phone;
            existingBarber.City = request.City;
            existingBarber.District = request.District;
            existingBarber.Street = request.Street;
            existingBarber.BuildingNo = request.BuildingNo;
            existingBarber.DoorNumber = request.DoorNumber;
            existingBarber.TaxNo = request.TaxNo;
            existingBarber.BarberUrl = fileUrl;
            //existingBarber.BarberUrl = fileUrl != null ? "" : filePath;

            try
            {
                _context.SaveChanges();
                return Ok(existingBarber);
            }
            catch (Exception ex)
            {
                // Hata durumunda dosyayı silme
                System.IO.File.Delete(filePath);
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpDelete("Delete-Barber/{id}")]
        public IActionResult DeleteBarber(int id)
        {
            var existingBarber = _context.Barbers.Find(id);
            if (existingBarber == null)
            {
                return NotFound("Belirtilen kimlik numarasına sahip bir berber bulunamadı.");
            }
            try
            {
                _context.Barbers.Remove(existingBarber);
                _context.SaveChanges();
                return Ok("Berber başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }
    }
}

using Barber.Models.DTO;
using Barber.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Barber.Models.Request;
using System.Linq;

namespace Barber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly BarberDbContext _context;
        private readonly TokenService _tokenService;
        private readonly JwtSettings _jwtSettings;

        public CustomerController(BarberDbContext context, TokenService tokenService, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpGet("get-customers")]
        public IActionResult GetCustomers()
        {
            try
            {
                var customers = _context.Customers.ToList();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpGet("get-customer/{id}")]
        public IActionResult GetCustomerById(int id)
        {
            try
            {
                var customer = _context.Customers.Find(id);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }
        
        [HttpPost("create-customer")]
        public IActionResult CreateCustomer([FromBody] Customers customerData)
        {
            var newCustomer = new Customers
            {
                Name = customerData.Name,
                LastName = customerData.LastName,
                Age = customerData.Age,
                Gender = customerData.Gender,
                UserName = customerData.UserName,
                Mail = customerData.Mail,
                Password = customerData.Password,
                Phone = customerData.Phone,
                City = customerData.City,
                District = customerData.District,
                Street = customerData.Street
            };
            try
            {
                _context.Customers.Add(newCustomer);
                _context.SaveChanges();
                return Ok(newCustomer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error!: " + ex.Message);
            }
        }
        
        [HttpPost("login")]
        public IActionResult Login([FromBody] Barber.Models.Request.LoginRequest loginData)
        {
            try
            {
                var user = _context.Customers.FirstOrDefault(u => u.UserName == loginData.Username && u.Password == loginData.Password);

                if (user == null)
                {
                    return BadRequest("Kullanıcı adı veya şifresi hatalı.");
                }
                // Kullanıcı doğrulandıysa JWT token oluştur.
                var token = _tokenService.GenerateJwtToken(
                    _jwtSettings.Issuer,
                    _jwtSettings.Audience,
                    _jwtSettings.ExpireMinutes,
                    user.Id.ToString(), // Kullanıcı kimlik bilgisi
                    "Customer" // Kullanıcı rolü
                );

                return Ok(new { Token = token, User = user.Id, user.UserName, user.City, user.District, user.Street });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Sunucu hatası: " + ex.Message);
            }
        }

        [HttpPost("token")]
        public IActionResult GenerateJwtToken([FromBody] TokenRequest tokenRequest)
        {
            try
            {
                // Token oluşturma işlemleri...
                // Dönüş değeri
                return Ok("Token oluşturuldu.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Sunucu hatası: " + ex.Message);
            }
        }

        [HttpPost("create-customers")]
        public IActionResult CreateCustomer([FromForm] CustomerCreate customerData)
        {
            // Veri doğrulaması
            if (customerData == null)
                return BadRequest("Geçersiz veri: Çalışan verisi boş.");

            // Resim yolu doğrulama
            if (customerData.CustomerFile == null || customerData.CustomerFile.Length == 0)
                return BadRequest("Geçersiz veri: Profil resmi yüklenmedi.");

            // Yeni dosya adı oluşturma
            var newFileName = Guid.NewGuid().ToString() + ".jpg";

            // Dosya yolu
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "CustomerPictures");
            var filePath = Path.Combine(folderPath, newFileName);

            // Dosyayı sunucuya kaydetme
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                customerData.CustomerFile.CopyTo(stream);
            }

            // Resmin URL'sini oluşturma
            var fileUrl = Path.Combine("/CustomerPictures", newFileName);

            // Yeni çalışan oluşturma
            var newCustomer = new Customers
            {
                Name = customerData.Name,
                LastName = customerData.LastName,
                Age = customerData.Age,
                Gender = customerData.Gender,
                UserName = customerData.UserName,
                Mail = customerData.Mail,
                Password = customerData.Password,
                Phone = customerData.Phone,
                City = customerData.City,
                District = customerData.District,
                Street = customerData.Street, 
                CustomerUrl = fileUrl
            };

            try
            {
                _context.Customers.Add(newCustomer);
                _context.SaveChanges();
                return Ok(newCustomer);
            }
            catch (Exception ex)
            {
                // Hata durumunda dosyayı silme
                System.IO.File.Delete(filePath);
                return StatusCode(500, "Hata: " + ex.Message + " Inner Exception: " + ex.InnerException?.Message);
            }
        }

        [HttpPut("update-customer/{id}")]
        public IActionResult UpdateCustomer(int id, [FromBody] Customers customerData)
        {
            var existingCustomer = _context.Customers.Find(id);
            if (existingCustomer == null)
            {
                return NotFound("Belirtilen kimlik numarasına sahip bir müşteri bulunamadı.");
            }
            existingCustomer.Name = customerData.Name;
            existingCustomer.LastName = customerData.LastName;
            existingCustomer.Age = customerData.Age;
            existingCustomer.Gender = customerData.Gender;
            existingCustomer.UserName = customerData.UserName;
            existingCustomer.Mail = customerData.Mail;
            existingCustomer.Password = customerData.Password;
            existingCustomer.Phone = customerData.Phone;
            existingCustomer.City = customerData.City;
            existingCustomer.District = customerData.District;
            existingCustomer.Street = customerData.Street;
            existingCustomer.CustomerUrl = customerData.CustomerUrl;

            try
            {
                _context.SaveChanges();
                return Ok(existingCustomer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpDelete("delete-customer/{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            var existingCustomer = _context.Customers.Find(id);
            if (existingCustomer == null)
            {
                return NotFound("Belirtilen kimlik numarasına sahip bir müşteri bulunamadı.");
            }
            try
            {
                _context.Customers.Remove(existingCustomer);
                _context.SaveChanges();
                return Ok("Müşteri başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }
    }
}

using Barber.Models.DTO;
using Barber.Models.Request;
using Barber.Models.Response;
using Barber.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
<<<<<<< HEAD
using Microsoft.AspNetCore.Http;

=======
using System.Security.Claims;
using System.Text;
>>>>>>> aa434d440d63176c9f849ea37d72d6a060f2ab08

namespace Barber.Controllers
{
    [Route("API/[controller]")]
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

        [HttpGet("Get-Customers")]
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

        [HttpGet("Get-Customer/{id}")]
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

<<<<<<< HEAD
        [HttpPost("Login")]
        public IActionResult Login([FromForm] Barber.Models.Request.LoginRequest loginData)
=======
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
>>>>>>> aa434d440d63176c9f849ea37d72d6a060f2ab08
        {
            try
            {
                var user = _context.Customers.FirstOrDefault(u => u.UserName == loginData.Username && u.Password == loginData.Password);

                if (user == null)
                {
                    return BadRequest("Kullanıcı adı veya şifresi hatalı.");
                }

                var token = _tokenService.GenerateJwtToken(
                    _jwtSettings.Issuer,
                    _jwtSettings.Audience,
                    _jwtSettings.ExpireMinutes,
                    user.Id.ToString(),
                    "Customer"
                );

<<<<<<< HEAD
                return Ok(new { Token = token, User = user.Id, user.UserName, user.City, user.District, user.Street });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Sunucu hatası: " + ex.Message);
            }
        }
        /*
        [HttpPost("token")]
        public IActionResult GenerateJwtToken([FromForm] TokenRequest tokenRequest)
        {
            try
            {
                // Token oluşturma işlemleri...
                // Dönüş değeri
                return Ok("Token oluşturuldu.");
=======
                user.RefreshToken = token;

                CustomerResponse customerResponse = new CustomerResponse()
                {
                    Token = token,
                    Id = user.Id,
                    UserName = user.UserName,
                    City = user.City,
                    District = user.District,
                    Street = user.Street
                };
                return Ok(customerResponse);
>>>>>>> aa434d440d63176c9f849ea37d72d6a060f2ab08
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Sunucu hatası: " + ex.Message);
            }
        }
        */
        [HttpPost("Create-Customers")]
        public IActionResult CreateCustomer([FromForm] CustomerCreate customerData)
        {
            if (customerData == null)
                return BadRequest("Geçersiz veri: Çalışan verisi boş.");

            if (customerData.CustomerFile == null || customerData.CustomerFile.Length == 0)
                return BadRequest("Geçersiz veri: Profil resmi yüklenmedi.");

            var newFileName = Guid.NewGuid().ToString() + ".jpg";

<<<<<<< HEAD
            // Dosya yolu
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(),"Pictures", "CustomerPictures");
=======
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "CustomerPictures");
>>>>>>> aa434d440d63176c9f849ea37d72d6a060f2ab08
            var filePath = Path.Combine(folderPath, newFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                customerData.CustomerFile.CopyTo(stream);
            }

<<<<<<< HEAD
            // Resmin URL'sini oluşturma
            var fileUrl = Path.Combine("\\Pictures\\CustomerPictures", newFileName);
            // Yeni çalışan oluşturma
=======
            var fileUrl = Path.Combine("/CustomerPictures", newFileName);

>>>>>>> aa434d440d63176c9f849ea37d72d6a060f2ab08
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
                System.IO.File.Delete(filePath);
                return StatusCode(500, "Hata: " + ex.Message + " Inner Exception: " + ex.InnerException?.Message);
            }
        }

        [HttpPut("Update-Customer/{id}")]
        public IActionResult UpdateCustomer(int id, [FromForm] Customers customerData)
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

        [HttpDelete("Delete-Customer/{id}")]
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
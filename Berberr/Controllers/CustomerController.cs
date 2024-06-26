﻿using Barber.Models.DTO;
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
using Microsoft.AspNetCore.Http;
using Barber.Models.Update;

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

        [HttpPost("Login")]
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

        [HttpPost("Create-Customers")]
        public IActionResult CreateCustomer([FromForm] CustomerCreate customerData)
        {
            // Veri doğrulaması
            if (customerData == null)
                return BadRequest("Geçersiz veri: Çalışan verisi boş.");

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
                CustomerUrl = ""
            };

            try
            {
                _context.Customers.Add(newCustomer);
                _context.SaveChanges();
                return Ok(newCustomer);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message + " Inner Exception: " + ex.InnerException?.Message);
            }
        }

        [HttpPut("Update-Customer")]
        public IActionResult UpdateCustomer([FromForm] CustomerUpdate request)
        {
            var existingCustomer = _context.Customers.Find(request.Id);
            if (request == null)
            {
                return BadRequest("Geçersiz veri: Müşteri verisi boş.");
            }

            if (existingCustomer == null)
            {
                return NotFound("Belirtilen kimlik numarasına sahip bir müşteri bulunamadı.");
            }

            // Resim yolu doğrulama
            if (request.CustomerFile == null || request.CustomerFile.Length == 0)
                return BadRequest("Geçersiz veri: Profil resmi yüklenmedi.");
            

            // Yeni dosya adı oluşturma
            var newFileName = Guid.NewGuid().ToString() + ".jpg";

            // Dosya yolu
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Pictures", "CustomerPictures");
            var filePath = Path.Combine(folderPath, newFileName);

            // Resmin URL'sini oluşturma
            var fileUrl = Path.Combine("Pictures", "CustomerPictures", newFileName);

            // Klasörün varlığını kontrol et ve yoksa oluştur
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Dosyayı sunucuya kaydetme
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    request.CustomerFile.CopyTo(stream);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Dosya yüklenirken bir hata oluştu: " + ex.Message);
            }

            // Müşteri bilgilerini güncelleme
            existingCustomer.Name = request.Name;
            existingCustomer.LastName = request.LastName;
            existingCustomer.UserName = request.UserName;
            existingCustomer.Age = request.Age;
            existingCustomer.Gender = request.Gender;
            existingCustomer.Mail = request.Mail;
            existingCustomer.Password = request.Password;
            existingCustomer.Phone = request.Phone;
            existingCustomer.City = request.City;
            existingCustomer.District = request.District;
            existingCustomer.Street = request.Street;
            existingCustomer.CustomerUrl = fileUrl;

            try
            {
                _context.SaveChanges();
                return Ok(existingCustomer);
            }
            catch (Exception ex)
            {
                // Hata durumunda dosyayı silme
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                return StatusCode(500, "Veritabanı işlemi sırasında bir hata oluştu: " + ex.Message);
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

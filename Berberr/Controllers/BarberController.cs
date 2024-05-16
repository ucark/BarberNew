﻿using Barber.Models.DTO;
using Barber.Models.Request;
using Barber.Models.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

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
                var barber = _context.Barbers.Find(id);

                return Ok(barber);
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
        [HttpPost("create-barber")]
        public IActionResult CreateBarber([FromBody] Barbers barberData)
        {
            if (barberData == null)
            {
                return BadRequest("Geçersiz veri: BarberCreate verisi boş.");
            }
            try
            {
                var newBarber = new Barbers
                {
                    Name = barberData.Name,
                    LastName = barberData.LastName,
                    UserName = barberData.UserName,
                    WorkPlaceName = barberData.WorkPlaceName,
                    Mail = barberData.Mail,
                    Password = barberData.Password,
                    Phone = barberData.Phone,
                    City = barberData.City,
                    District = barberData.District,
                    Street = barberData.Street,
                    BuildingNo = barberData.BuildingNo,
                    DoorNumber = barberData.DoorNumber,
                    TaxNo = barberData.TaxNo,
                    BarberUrl = barberData.BarberUrl
                };

                _context.Barbers.Add(newBarber);
                _context.SaveChanges();
                return Ok(newBarber);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Hata: " + ex.Message);
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Barber.Models.Request.LoginRequest loginData)
>>>>>>> aa434d440d63176c9f849ea37d72d6a060f2ab08
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
        /*
        [HttpPost("token")]
        public IActionResult GenerateJwtToken([FromForm] TokenRequest tokenRequest)
        {
            try
            {
                //token oluşturma işlemi
                // dönüş değeri
                return Ok("Token Oluşturuldu.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Sunucu hatası: " + ex.Message);
            }
        }
<<<<<<< HEAD
        */
        [HttpPost("Create-Barbers")]
        public IActionResult CreateBarber([FromForm] BarberCreate barberData)
=======

        [HttpPut("update-barber")]
        public IActionResult CreateBarber([FromForm] BarberUpdate barberData)
>>>>>>> aa434d440d63176c9f849ea37d72d6a060f2ab08
        {
            if (barberData == null)
                return BadRequest("Geçersiz veri: Berber verisi boş.");

            if (barberData.BarberFile == null || barberData.BarberFile.Length == 0)
                return BadRequest("Geçersiz veri: Profil resmi yüklenmedi.");

            var newFileName = Guid.NewGuid().ToString() + ".jpg";

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(),"Pictures", "BarberPictures");
            var filePath = Path.Combine(folderPath, newFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                barberData.BarberFile.CopyTo(stream);
            }
            var fileUrl = Path.Combine("\\Pictures\\BarberPictures", newFileName);

            var newBarber = new Barbers
            {
                BarberUrl = fileUrl
            };

            try
            {
                _context.Barbers.Add(newBarber);
                _context.SaveChanges();
                return Ok(newBarber);
            }
            catch (Exception ex)
            {
                System.IO.File.Delete(filePath);
                return StatusCode(500, "Hata: " + ex.Message + "Inner Exception: ");
            }
            
        }

        [HttpPut("Update-Barber/{id}")]
        public IActionResult UpdateBarber(int id, [FromForm] Barbers barberData)
        {
            var existingBarber = _context.Barbers.Find(id);
            if (existingBarber == null)
            {
                return NotFound("Belirtilen kimlik numarasına sahip bir berber bulunamadı.");
            }
            existingBarber.Name = barberData.Name;
            existingBarber.LastName = barberData.LastName;
            existingBarber.UserName = barberData.UserName;
            existingBarber.WorkPlaceName = barberData.WorkPlaceName;
            existingBarber.Mail = barberData.Mail;
            existingBarber.Password = barberData.Password;
            existingBarber.Phone = barberData.Phone;
            existingBarber.City = barberData.City;
            existingBarber.District = barberData.District;
            existingBarber.Street = barberData.Street;
            existingBarber.BuildingNo = barberData.BuildingNo;
            existingBarber.DoorNumber = barberData.DoorNumber;
            existingBarber.TaxNo = barberData.TaxNo;
            existingBarber.BarberUrl = barberData.BarberUrl;

            try
            {
                _context.SaveChanges();
                return Ok(existingBarber);
            }
            catch (Exception ex)
            {
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

using Microsoft.AspNetCore.Mvc;
using Barber.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Barber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly BarberDbContext _context;

        public AuthController(TokenService tokenService, BarberDbContext context)
        {
            _tokenService = tokenService;
            _context = context;
        }

        // Kullanıcı girişi için endpoint
        [HttpPost("login")]
        public IActionResult Login([FromBody] LogIn model)
        {
            // Kullanıcı adı ve şifrenin null olup olmadığını kontrol et
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Kullanıcı adı veya şifre boş olamaz.");
            }

            // Kullanıcıyı veritabanında ara
            var user = _context.LogIn.SingleOrDefault(u => u.UserName == model.UserName && u.Password == model.Password);

            if (user == null)
            {
                // Kullanıcı bulunamadıysa 401 Unauthorized hatası döndür
                return Unauthorized();
            }

            // Kullanıcı adı ve şifre doğruysa JWT tokeni oluştur
            var token = _tokenService.GenerateJwtToken("My Barber App", "API Servers", 60, model.UserName);

            // Token istemciye gönderilir
            return Ok(new { token });
        }
    }
}

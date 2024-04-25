using Microsoft.AspNetCore.Mvc;
using Barber.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Barber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            // Burada normalde kullanıcı doğrulama işlemleri gerçekleştirilir.
            // Örneğin, veritabanından kullanıcı adı ve şifreyi kontrol edebilirsiniz.

            // Örnek bir kullanıcı doğrulama işlemi, burada sabit bir kullanıcı için yapılacak.
            if (loginModel.Username == "kullanici" && loginModel.Password == "sifre")
            {
                // Kullanıcı doğrulandı, JWT oluşturulacak

                // JWT oluşturulması için gerekli ayarlar
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Token:SecurityKey"]); // JWT'nin imzalanması için kullanılan gizli anahtar
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, loginModel.Username) // Kullanıcı adı, JWT içinde saklanabilir
                        // Burada ekstra yetkilendirme bilgileri de eklenebilir
                    }),
                    Expires = DateTime.UtcNow.AddHours(1), // Token süresi (örnekte 1 saat)
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                // JWT oluştur
                var token = tokenHandler.CreateToken(tokenDescriptor);

                // Oluşturulan JWT'yi string formatına çevir
                var tokenString = tokenHandler.WriteToken(token);

                // JWT'yi kullanıcıya döndür
                return Ok(new { Token = tokenString });
            }

            // Kullanıcı doğrulanamadı
            return Unauthorized();
        }
    }
}

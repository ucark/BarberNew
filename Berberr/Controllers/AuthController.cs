using Microsoft.AspNetCore.Mvc;
using Barber.Models;

namespace Barber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public AuthController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            // Gerçek kullanıcı bilgileri yerine bu örnek bilgileri kullanıyoruz
            string validUsername = "vaybe23";
            string validPassword = "123456";

            // Gelen kullanıcı adı ve şifreyi kontrol ediyoruz
            if (username == validUsername && password == validPassword)
            {
                // 256 bit uzunluğunda gizli anahtar
                var secretKey = "my_super_secret_key_with_256_bits_length_my_super_secret_key_with_256_bits_length";
                var issuer = "MyBarberApp";
                var audience = "BarberAPI";

                var token = _tokenService.GenerateJwtToken(secretKey, issuer, audience, 60, username);

                // Token istemciye gönderilir
                return Ok(new { token });
            }
            else
            {
                // Kullanıcı adı veya şifre hatalı
                return Unauthorized();
            }
        }
    }
}

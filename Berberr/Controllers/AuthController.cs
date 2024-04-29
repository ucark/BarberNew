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

        // Yeni kullanıcı kaydı için endpoint
        [HttpPost("register")]
        public IActionResult Register([FromBody] LogIn model)
        {
            // Kullanıcı adı ve şifrenin null olup olmadığını kontrol et
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Kullanıcı adı veya şifre boş olamaz.");
            }

            // Kullanıcı oluşturulduktan sonra JWT tokeni oluştur
            var token = _tokenService.GenerateJwtToken("My Barber App", "API Servers", 60, model.UserName);

            // Token istemciye gönderilir
            return Ok(new { token });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Barber.Models;
using Microsoft.EntityFrameworkCore;

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

        // Kullanıcı verilerini getirmek için endpoint
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _context.LogIn.Find(id);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }
            return Ok(user);
        }

        // Kullanıcı verilerini güncellemek için endpoint
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] LogIn model)
        {
            if (id != model.Id)
            {
                return BadRequest("Geçersiz id.");
            }

            _context.Entry(model).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.LogIn.Any(e => e.Id == id))
                {
                    return NotFound("Kullanıcı bulunamadı.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Kullanıcı verilerini silmek için endpoint
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = _context.LogIn.Find(id);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            _context.LogIn.Remove(user);
            _context.SaveChanges();

            return Ok("Kullanıcı başarıyla silindi.");
        }
    }
}

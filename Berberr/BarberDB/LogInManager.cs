using Barber.Models.DTO;

namespace Barber.BarberDB
{
    public class LogInManager(BarberDbContext context)
    {
        private readonly BarberDbContext _context = context;

        public void AddLogin(string username, string password)
        {
            var newLogin = new Login
            {
                Username = username,
                Password = password
            };
            _context.Login.Add(newLogin);
            _context.SaveChanges();
        }
    }
}

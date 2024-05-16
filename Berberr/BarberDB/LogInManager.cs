using Barber.Models.DTO;
using Barber;

public class LogInManager
{
    private readonly BarberDbContext _context;

    public LogInManager(BarberDbContext context)
    {
        _context = context;
    }

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

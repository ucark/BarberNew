using Barber.Models.DTO;
using Barber;

public class EmployeeManager
{
    private readonly BarberDbContext _context;

    public EmployeeManager(BarberDbContext context)
    {
        _context = context;
    }

    public void AddEmployee(string name, string lastName, string employeeUrl)
    {
        var newEmployee = new Employees
        {
            Name = name,
            LastName = lastName,
            EmployeeUrl = employeeUrl
        };
        _context.Employees.Add(newEmployee);
        _context.SaveChanges();
    }
}

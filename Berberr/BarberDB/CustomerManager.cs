using Barber.Models.DTO;
using Barber.Models;

namespace Barber.BarberDB
{
    public class CustomerManager
    {
        private readonly BarberDbContext _context;

        public CustomerManager(BarberDbContext context)
        {
            _context = context;
        }

        public void AddCustomer(string name, string lastName, string age, bool gender, 
            string userName, string mail, string password, 
            string phone, string city, string district, string street)
        {
            var newCustomer = new Customers
            {
                Name = name,
                LastName = lastName,
                Age = age,
                Gender = gender,
                UserName = userName,
                Mail = mail,
                Password = password,
                Phone = phone,
                City = city,
                District = district,
                Street = street
            };
            _context.Customers.Add(newCustomer);
            _context.SaveChanges();
        }
    }
}

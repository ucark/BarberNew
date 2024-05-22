using Barber.Models.DTO;
using Barber.Models;

namespace Barber.BarberDB
{
    public class BarberManager
    {
        private readonly BarberDbContext _context;

        public BarberManager(BarberDbContext context)
        {
            _context = context;
        }

        public void AddBarber(string name, string lastName, string userName, string workPlaceName, string mail, string password,
            string phone, string city, string district, string street,
            string buildingNo, string doorNumber, string taxNo)
        {
            var newBarber = new Barbers
            {
                Name = name,
                LastName = lastName,
                UserName = userName,
                WorkPlaceName = workPlaceName,
                Mail = mail,
                Password = password,
                Phone = phone,
                City = city,
                District = district,
                Street = street,
                BuildingNo = buildingNo,
                DoorNumber = doorNumber,
                TaxNo = taxNo,
            };
            _context.Barbers.Add(newBarber);
            _context.SaveChanges();
        }
    }
}

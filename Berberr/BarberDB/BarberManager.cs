using Barber.Models.DTO;

namespace Barber.BarberDB
{
    public class BarberManager
    {
        private readonly BarberDbContext _context;

        public BarberManager(BarberDbContext context)
        {
            _context = context;
        }

        public void AddBarber(string name, string lastName ,string userName, string workPlaceName, string mail, string password, 
            string phone, string city, string district, string street, 
            string BuildingNo, string DoorNumber, string TaxNo, string barberUrl)
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
                BuildingNo = BuildingNo,
                DoorNumber = DoorNumber,
                TaxNo = TaxNo,
                BarberUrl = barberUrl
            };
            _context.Barbers.Add(newBarber);
            _context.SaveChanges();
        }
    }
}

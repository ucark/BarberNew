using Barber.Models.Request;

namespace Barber.Models.Update
{
    public class BarberUpdate : BarberCreate
    {
        public int Id { get; set; }
        public IFormFile BarberFile { get; set; }
    }
}

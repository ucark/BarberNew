namespace Barber.Models.Request
{
    public class BarberUpdate : BarberCreate
    {
        public int Id { get; set; }
        public IFormFile BarberFile { get; set; }
    }
}

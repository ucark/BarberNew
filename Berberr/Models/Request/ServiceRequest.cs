namespace Barber.Models.Request
{
    public class ServiceRequest
    {
        public string Name { get; set; }
        public int BarberId { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
    }
}

using Barber.Models.Request;

namespace Barber.Models.Update
{
    public class CustomerUpdate : CustomerCreate
    {
        public int Id { get; set; }
        public IFormFile CustomerFile { get; set; }
    }
}

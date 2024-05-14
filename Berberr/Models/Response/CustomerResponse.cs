using System.ComponentModel.DataAnnotations;

namespace Barber.Models.Response
{
    public class CustomerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Age { get; set; }
        public bool Gender { get; set; }
        public string UserName { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Street { get; set; }
        public string CustomerUrl { get; set; }

        public string Token { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Barber.Models.Request
{
    public class EmployeeCreate
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public int BarberID { get; set; }
        public IFormFile EmployeeFile { get; set; }
    }
}

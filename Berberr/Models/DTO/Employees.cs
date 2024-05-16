using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barber.Models.DTO
{
    [Table("Employees")]
    public class Employees
    {
        [Key] public int Id { get; set; }
        public int BarberID { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string EmployeeUrl { get; set; }
    }
}

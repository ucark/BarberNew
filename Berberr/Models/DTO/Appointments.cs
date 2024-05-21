using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barber.Models.DTO
{
    [Table("Appointments")]
    public class Appointments
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }

        [ForeignKey("BarberID")]
        public int BarberID { get; set; }
        public Barbers Barbers { get; set; }

        [ForeignKey("customerID")]
        public int CustomerID { get; set; }
        public Customers Customers { get; set; }
        public int ServiceID { get; set; }
        public Services Services { get; set; }
    }
}

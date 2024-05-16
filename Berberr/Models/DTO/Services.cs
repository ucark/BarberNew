using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barber.Models.DTO
{
    [Table("Services")]
    public class Services
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int BarberId { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
    }
}

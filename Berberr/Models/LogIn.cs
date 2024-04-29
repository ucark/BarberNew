using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barber.Models
{
    [Table("LogIn")]
    public class LogIn
    {
        [Key] public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}

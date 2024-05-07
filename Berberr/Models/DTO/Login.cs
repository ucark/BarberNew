using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barber.Models.DTO
{
    [Table("Login")]
    public class Login
    {
        [Key] public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

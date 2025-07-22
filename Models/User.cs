using System.ComponentModel.DataAnnotations.Schema;
using Models.Enums;

namespace Models
{
    [Table("User")]
    public class User
    {
        public long UserId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; }
        public string GoogleId { get; set; }
        public string Img { get; set; }
    }
}

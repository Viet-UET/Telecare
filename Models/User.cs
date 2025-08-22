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
        public string? GoogleId { get; set; }
        public string? Img { get; set; }

        // Navigation Properties
        public Patient Patient { get; set; }
        public Hospital Hospital { get; set; }
        public Doctor Doctor { get; set; }
        public ICollection<Token> Tokens { get; set; } = new List<Token>();

        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    }
}

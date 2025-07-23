using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("Token")]
    public class Token
    {
        public long UserId { get; set; }
        [Key]
        public string TokenValue { get; set; }
        public bool Expires { get; set; }
    }
}

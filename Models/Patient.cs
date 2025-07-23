using System.ComponentModel.DataAnnotations.Schema;
using Models.Enums;

namespace Models
{
    [Table("Patient")]
    public class Patient
    {
        public long PatientId { get; set; }
        public long? UserId { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public Sex? Sex { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        // Navigation Property
        public User User { get; set; }
    }
}

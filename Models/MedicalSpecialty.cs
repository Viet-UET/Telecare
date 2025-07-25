using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("MedicalSpecialty")]
    public class MedicalSpecialty
    {
        public int MedicalSpecialtyId { get; set; }
        public string EnglishName { get; set; }
        public string VietnameseName { get; set; }

        // Navigation Property
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}

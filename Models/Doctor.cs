using System.ComponentModel.DataAnnotations.Schema;
using Models.Enums;

namespace Models
{
    [Table("Doctor")]
    public class Doctor
    {
        public long DoctorId { get; set; }
        public long? UserId { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public Sex? Sex { get; set; }
        public string Phone { get; set; }
        public int? MedicalSpecialtyId { get; set; }
        public long? HospitalId { get; set; }
        public string Degree { get; set; }
        public float ConsultingPriceViaMessage { get; set; }
        public float ConsultingPriceViaCall { get; set; }
        public float? Point { get; set; }
    }
}

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
        public double? ConsultingPriceViaMessage { get; set; }
        public double? ConsultingPriceViaCall { get; set; }
        public double? Point { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public MedicalSpecialty MedicalSpecialty { get; set; }
        public Hospital Hospital { get; set; }
        public ICollection<CallAppointment> CallAppointments { get; set; } = new List<CallAppointment>();
        public ICollection<DoctorComment> DoctorComments { get; set; } = new List<DoctorComment>();

    }
}

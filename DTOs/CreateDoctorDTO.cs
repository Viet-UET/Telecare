using Models.Enums;

namespace DTOs
{
    public class CreateDoctorDTO
    {
        public Role CreatorRole { get; set; }

        public int CreatorId { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public Sex Sex { get; set; }

        public string Phone { get; set; }

        public int MedicalSpecialtyId { get; set; }

        public int HospitalId { get; set; }

        public string Degree { get; set; }

        public double ConsultingPriceViaMessage { get; set; }

        public double ConsultingPriceViaCall { get; set; }
    }
}
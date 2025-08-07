using System;
using Models.Enums;

namespace DTOs
{
    public class DoctorUpdateDTO
    {
        public Role CreatorRole { get; set; }
        public long CreatorId { get; set; }
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
    }
}
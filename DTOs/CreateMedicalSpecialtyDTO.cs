using Models.Enums;

namespace Models
{
    public class CreateMedicalSpecialtyDTO
    {
        public Role createrRole { get; set; }
        public string EnglishName { get; set; }
        public string VietnameseName { get; set; }

    }
}
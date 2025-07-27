using Models.Enums;

namespace DTOs

{
    public class CreatePatientDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public Sex Sex { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}
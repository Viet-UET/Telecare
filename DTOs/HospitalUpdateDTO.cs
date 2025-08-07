using Models.Enums;

namespace DTOs
{
    public class HospitalUpdateDTO
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string WorkingTime { get; set; }
        public double? Point { get; set; }
    }
}

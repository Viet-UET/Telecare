using Models.Enums;

namespace DTOs
{
    public class HospitalAppointmentDTO
    {
        public DateTime Time { get; set; }
        public long HospitalId { get; set; }
        public long PatientId { get; set; }
        // public AppointmentState State { get; set; }
    }
}
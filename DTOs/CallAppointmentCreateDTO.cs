namespace DTOs
{
    public class CallAppointmentCreateDTO
    {
        public DateTime Time { get; set; }
        public string? Form { get; set; }
        public long DoctorId { get; set; }
        public long PatientId { get; set; }
    }
}
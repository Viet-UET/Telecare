namespace DTOs
{
    public class DoctorCommentDTO
    {
        public long DoctorId { get; set; }
        public long PatientId { get; set; }
        public string? Comment { get; set; }
        public float? Point { get; set; }
    }
}
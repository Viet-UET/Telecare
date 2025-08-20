namespace DTOs
{
    public class HospitalCommentDTO
    {
        public long HospitalId { get; set; }
        public long PatientId { get; set; }
        public string? Comment { get; set; }
        public float? Point { get; set; }
    }
}
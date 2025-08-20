namespace Models
{
    public class HospitalComment
    {
        public int HospitalCommentId { get; set; }
        public DateTime? Time { get; set; }
        public int HospitalId { get; set; }
        public int PatientId { get; set; }
        public string? Comment { get; set; }
        public float? Point { get; set; }



        public Patient? Patient { get; set; }
        public Hospital? Hospital { get; set; }


        // public ICollection<HospitalComment> HospitalComments { get; set; } = new List<HospitalComment>();

    }
}
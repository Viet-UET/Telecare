using System.Drawing;

namespace Models
{
    public class DoctorComment
    {
        public int DoctorCommentId { get; set; }
        public DateTime? Time { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public string? Comment { get; set; }
        public float? Point { get; set; }

        // Navigation properties
        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }

        // public ICollection<DoctorComment> DoctorComments { get; set; } = new List<DoctorComment>();

    }
}
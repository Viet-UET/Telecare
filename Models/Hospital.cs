using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("Hospital")]
    public class Hospital
    {
        public long HospitalId { get; set; }
        public long? UserId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string WorkingTime { get; set; }
        public double? Point { get; set; }

        // Navigation Properties
        public User User { get; set; }
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        public ICollection<HospitalAppointment> HospitalAppointments { get; set; } = new List<HospitalAppointment>();
        public ICollection<HospitalComment> HospitalComments { get; set; } = new List<HospitalComment>();

    }
}

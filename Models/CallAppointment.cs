
using System.ComponentModel.DataAnnotations.Schema;
using Models.Enums;

namespace Models
{
    [Table("CallAppointment")]
    public class CallAppointment
    {
        public int CallAppointmentId { get; set; }
        public DateTime Time { get; set; }
        public string? Form { get; set; }
        public long DoctorId { get; set; }
        public long PatientId { get; set; }
        public AppointmentState State { get; set; } = AppointmentState.UNCONFIRM;

        public Doctor? Doctor { get; set; }
        public Patient? Patient { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using Models.Enums;

namespace Models
{
    [Table("HospitalAppointment")]
    public class HospitalAppointment
    {
        public int HospitalAppointmentId { get; set; }
        public DateTime Time { get; set; }
        public long HospitalId { get; set; }
        public long PatientId { get; set; }
        public AppointmentState State { get; set; } = AppointmentState.UNCONFIRM;

        public Hospital? Hospital { get; set; }
        public Patient? Patient { get; set; }
    }
}
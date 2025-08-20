using Data;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;

namespace Controllers
{
    // [ApiController]
    // [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public AppointmentController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost("make-call-appointment")]
        public async Task<ActionResult<object>> MakeCallAppointment(CallAppointmentCreateDTO callAppointmentCreateDTO)
        {
            if (callAppointmentCreateDTO.Time < DateTime.Now)
            {
                return BadRequest(new { message = "Appointment time cannot be in the past." });
            }

            var appointments = await _context.CallAppointments.Where(c => c.DoctorId == callAppointmentCreateDTO.DoctorId).OrderBy(x => x.Time).ToListAsync();
            foreach (var app in appointments)
            {
                if ((callAppointmentCreateDTO.Time <= app.Time.AddHours(1)) && (callAppointmentCreateDTO.Time.AddHours(1) >= app.Time))
                {
                    return Conflict(new { message = "doctor is busy" });
                }
            }


            var appointments2 = await _context.CallAppointments.Where(c => c.PatientId == callAppointmentCreateDTO.PatientId).OrderBy(x => x.Time).ToListAsync();
            foreach (var app in appointments2)
            {
                if ((callAppointmentCreateDTO.Time <= app.Time.AddHours(1)) && (callAppointmentCreateDTO.Time.AddHours(1) >= app.Time))
                {
                    return Conflict(new { message = "patient is busy" });
                }
            }

            var appointment = new CallAppointment
            {
                Time = callAppointmentCreateDTO.Time,
                Form = callAppointmentCreateDTO.Form,
                DoctorId = callAppointmentCreateDTO.DoctorId,
                PatientId = callAppointmentCreateDTO.PatientId
            };
            await _context.AddAsync(appointment);
            await _context.SaveChangesAsync();
            return Ok(appointment);
        }

        [HttpPost("change-time-call-appointment")]
        public async Task<ActionResult<object>> ChangeTimeCallAppointment(long userId, int callAppointmentId, DateTime time)
        {
            var isDoctor = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (isDoctor.Role != Role.DOCTOR)
            {
                return Forbid("Only doctors can change the appointment time.");
            }
            var currentAppointment = await _context.CallAppointments.FirstOrDefaultAsync(c => c.CallAppointmentId == callAppointmentId);
            currentAppointment.Time = time;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Appointment time successfully changed." });
        }


        [HttpPost("change-state-call-appointment")]
        public async Task<ActionResult<object>> ChangeStateCallAppointment(long userId, int callAppointmentId, AppointmentState state)
        {
            var isDoctor = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (isDoctor.Role != Role.DOCTOR)
            {
                return Forbid("Only doctors can change the appointment state.");
            }
            var currentAppointment = await _context.CallAppointments.FirstOrDefaultAsync(c => c.CallAppointmentId == callAppointmentId);
            currentAppointment.State = state;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Appointment state successfully changed." });
        }






        [HttpPost("make-hospital-appointment")]
        public async Task<ActionResult<object>> MakeHospitalAppointment(HospitalAppointmentDTO hospitalAppointmentDTO)
        {
            if (hospitalAppointmentDTO.Time < DateTime.Now)
            {
                return BadRequest(new { message = "Appointment time cannot be in the past." });
            }

            var appointment = new HospitalAppointment
            {
                Time = hospitalAppointmentDTO.Time,
                HospitalId = hospitalAppointmentDTO.HospitalId,
                PatientId = hospitalAppointmentDTO.PatientId,
                State = AppointmentState.UNCONFIRM
            };
            await _context.AddAsync(appointment);
            await _context.SaveChangesAsync();
            return Ok(appointment);
        }


        [HttpPost("change-time-hospital-appointment")]
        public async Task<ActionResult<object>> ChangeTimeHospitalAppointment(long userId, int hospitalAppointmentId, DateTime time)
        {
            var isHospital = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (isHospital.Role != Role.HOSPITAL)
            {
                return Forbid("Only hospitals can change the appointment time.");
            }
            var currentAppointment = await _context.hospitalAppointments.FirstOrDefaultAsync(c => c.HospitalAppointmentId == hospitalAppointmentId);
            currentAppointment.Time = time;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Appointment time successfully changed." });
        }


        [HttpPost("change-state-hospital-appointment")]
        public async Task<ActionResult<object>> ChangeStateHospitalAppointment(long userId, int hospitalAppointmentId, AppointmentState state)
        {
            var isDoctor = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (isDoctor.Role != Role.HOSPITAL)
            {
                return Forbid("Only doctors can change the appointment state.");
            }
            var currentAppointment = await _context.hospitalAppointments.FirstOrDefaultAsync(c => c.HospitalAppointmentId == hospitalAppointmentId);
            currentAppointment.State = state;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Appointment state successfully changed." });
        }

        [HttpGet("users/{userId}")]
        public async Task<ActionResult<object>> GetAppointmentOfUser([FromRoute] long userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user.Role == Role.PATIENT)
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
                var appointment = await _context.CallAppointments.Where(c => c.PatientId == patient.PatientId).OrderByDescending(c => c.Time).Select(c => c.Time).ToListAsync();
                return Ok(appointment);
            }
            if (user.Role == Role.DOCTOR)
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
                var appointment = await _context.CallAppointments.Where(c => c.DoctorId == doctor.DoctorId).OrderByDescending(c => c.Time).Select(c => c.Time).ToListAsync();
                return Ok(appointment);

            }
            if (user.Role == Role.HOSPITAL)
            {
                var hospital = await _context.Hospitals.FirstOrDefaultAsync(h => h.UserId == userId);
                var appointment = await _context.hospitalAppointments.Where(h => h.HospitalId == hospital.HospitalId).OrderByDescending(c => c.Time).Select(c => c.Time).ToListAsync();
                return Ok(appointment);

            }
            return BadRequest(new { message = "Unsupported role." });
        }

        [HttpGet("users/quantity")]
        public async Task<ActionResult<object>> GetNumberOfUser()
        {
            return Ok(await _context.Users.CountAsync());
        }


        [HttpGet("doctors/quantity")]
        public async Task<ActionResult<object>> GetNumberOfDoctor()
        {
            return Ok(await _context.Doctors.CountAsync());
        }

        [HttpGet("hospitals/quantity")]
        public async Task<ActionResult<object>> GetNumberOfHospital()
        {
            return Ok(await _context.Hospitals.CountAsync());
        }



    }
}
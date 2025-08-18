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



    }
}
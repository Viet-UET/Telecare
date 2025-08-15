using Data;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Controllers
{
    // [ApiController]
    // [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        private AppointmentController(ApplicationDBContext context)
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




    }
}
using System.Diagnostics.Tracing;
using Data;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Enums;

namespace Telecare.Controllers
{
    // [ApiController]
    // [Route("[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public DoctorController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPut("update-doctor")]
        public async Task<ActionResult<string>> UpdateDoctor(DoctorUpdateDTO doctorUpdateDTO, long doctorId)
        {
            if (doctorUpdateDTO.CreatorRole == Role.PATIENT)
            {
                return BadRequest("not permission");
            }
            var hospital = await _context.Hospitals.FirstOrDefaultAsync(h => h.HospitalId == doctorUpdateDTO.HospitalId);
            if (hospital == null)
            {
                return NotFound("Hospital not exist");
            }
            var medicalspecialty = await _context.MedicalSpecialties.FirstOrDefaultAsync(h => h.MedicalSpecialtyId == doctorUpdateDTO.MedicalSpecialtyId);
            if (medicalspecialty == null)
            {
                return NotFound("Medical specialty not exist");
            }

            var doctor = await _context.Doctors.FirstOrDefaultAsync(h => h.DoctorId == doctorId);
            if (doctor == null)
            {
                return NotFound("Doctor not found");
            }

            if ((doctorUpdateDTO.CreatorRole == Role.HOSPITAL) & (doctorUpdateDTO.CreatorId != doctor.HospitalId))
            {
                return BadRequest("not permission");
            }
            if ((doctorUpdateDTO.CreatorRole == Role.DOCTOR) & (doctorUpdateDTO.CreatorId != doctor.DoctorId))
            {
                return BadRequest("not permission");
            }

            if (doctorUpdateDTO.Name != null)
            {
                doctor.Name = doctorUpdateDTO.Name;
            }
            if (doctorUpdateDTO.Age != null)
            {
                doctor.Age = doctorUpdateDTO.Age;
            }
            if (doctorUpdateDTO.Sex != null)
            {
                doctor.Sex = doctorUpdateDTO.Sex;
            }
            if (doctorUpdateDTO.Phone != null)
            {
                doctor.Phone = doctorUpdateDTO.Phone;
            }
            if (doctorUpdateDTO.MedicalSpecialtyId != null)
            {
                doctor.MedicalSpecialtyId = doctorUpdateDTO.MedicalSpecialtyId;
            }
            if (doctorUpdateDTO.HospitalId != null)
            {
                doctor.HospitalId = doctorUpdateDTO.HospitalId;
            }
            if (doctorUpdateDTO.Degree != null)
            {
                doctor.Degree = doctorUpdateDTO.Degree;
            }
            if (doctorUpdateDTO.ConsultingPriceViaMessage != null)
            {
                doctor.ConsultingPriceViaMessage = doctorUpdateDTO.ConsultingPriceViaMessage;
            }
            if (doctorUpdateDTO.ConsultingPriceViaCall != null)
            {
                doctor.ConsultingPriceViaCall = doctorUpdateDTO.ConsultingPriceViaCall;
            }
            if (doctorUpdateDTO.Point != null)
            {
                doctor.Point = doctorUpdateDTO.Point;
            }

            await _context.SaveChangesAsync();
            return Ok("Doctor updated successfully");
        }

        [HttpGet("list-doctor")]
        public async Task<ActionResult<List<Doctor>>> GetListDoctor()
        {
            var doctors = await _context.Doctors.ToListAsync();
            return Ok(doctors);
        }

        [HttpGet("list-doctor/name")]
        public async Task<ActionResult<Doctor>> GetListDoctorByName(string name)
        {
            var DoctorHasName = await _context.Doctors.
        Where(d => d.Name.Contains(name)).ToListAsync();
            return Ok(DoctorHasName);
        }

        [HttpGet("doctorbyuserid/{userId}")]
        public async Task<ActionResult<Object>> GetDoctorByUserId([FromRoute] long userId)
        {
            var user = await _context.Users
            .Where(u => u.UserId == userId)
            .Select(o => new { o.Email, o.Role, o.GoogleId })
            .FirstOrDefaultAsync();
            var doctorByUserId = await _context.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            return Ok(new { doctor = doctorByUserId, user });
        }

        [HttpGet("doctor-in-hospital/{hospitalId}")]
        public async Task<ActionResult<Doctor>> GetDoctorInHospital([FromRoute] long hospitalId)
        {
            var doctorInHospital = await _context.Doctors.Where(d => d.HospitalId == hospitalId).ToListAsync();
            return Ok(doctorInHospital);
        }
    }
}
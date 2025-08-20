using Data;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Controllers
{
    // [ApiController]
    // [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {

        private readonly ApplicationDBContext _context;

        public CommentController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPost("add-doctor-comment")]
        public async Task<ActionResult<object>> AddDoctorComment(DoctorCommentDTO doctorCommentDTO)
        {
            var cm = new DoctorComment
            {
                Time = DateTime.Now,
                PatientId = doctorCommentDTO.PatientId,
                DoctorId = doctorCommentDTO.DoctorId,
                Comment = doctorCommentDTO.Comment,
                Point = doctorCommentDTO.Point
            };

            await _context.DoctorComments.AddAsync(cm);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Comment added successfully" });
        }


        [HttpPost("add-hospital-comment")]
        public async Task<ActionResult<object>> AddHospitalComment(HospitalCommentDTO hospitalCommentDTO)
        {
            var cm = new HospitalComment
            {
                Time = DateTime.Now,
                PatientId = hospitalCommentDTO.PatientId,
                HospitalId = hospitalCommentDTO.HospitalId,
                Comment = hospitalCommentDTO.Comment,
                Point = hospitalCommentDTO.Point
            };

            await _context.HospitalComments.AddAsync(cm);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Comment added successfully" });
        }



        [HttpGet("get-doctor-comment/{doctorId}")]
        public async Task<ActionResult<List<object[]>>> GetDoctorComment([FromRoute] long doctorId)
        {
            var res = new List<object[]>();

            var comment = await _context.DoctorComments.Where(x => x.DoctorId == doctorId).OrderByDescending(x => x.Time).ToListAsync();

            foreach (var item in comment)
            {
                var patientName = await _context.Patients.Where(x => x.PatientId == item.PatientId).Select(x => x.Name).FirstOrDefaultAsync();
                res.Add(new object[] { item, patientName });
            }
            return Ok(res);
        }

        [HttpGet("get-hospital-comment/{hospitalId}")]
        public async Task<ActionResult<List<object[]>>> GetHospitalComment([FromRoute] long hospitalId)
        {
            var res = new List<object[]>();

            var comment = await _context.HospitalComments.Where(x => x.HospitalId == hospitalId).OrderByDescending(x => x.Time).ToListAsync();

            foreach (var item in comment)
            {
                var patientName = await _context.Patients.Where(x => x.PatientId == item.PatientId).Select(x => x.Name).FirstOrDefaultAsync();
                res.Add(new object[] { item, patientName });
            }
            return Ok(res);
        }

    }
}
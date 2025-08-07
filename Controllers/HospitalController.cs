using Data;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Controllers
{
    // [ApiController]
    // [Route("[controller]")]
    public class HospitalController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public HospitalController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> UpdateHospital([FromRoute] long id, [FromBody] HospitalUpdateDTO hospitalUpdateDTO)
        {
            var currentHospital = await _context.Hospitals.FirstOrDefaultAsync(h => h.UserId == id);
            if (currentHospital != null)
            {
                if (hospitalUpdateDTO.Name != null)
                {
                    currentHospital.Name = hospitalUpdateDTO.Name;
                }
                if (hospitalUpdateDTO.Address != null)
                {
                    currentHospital.Address = hospitalUpdateDTO.Address;
                }
                if (hospitalUpdateDTO.WorkingTime != null)
                {
                    currentHospital.WorkingTime = hospitalUpdateDTO.WorkingTime;
                }
                if (hospitalUpdateDTO.Point != null)
                {
                    currentHospital.Point = hospitalUpdateDTO.Point;
                }

                await _context.SaveChangesAsync();

                return Ok("Hospital updated successfully.");
            }
            else
            {
                return NotFound($"Hospital with UserId {id} not found.");
            }
        }


        [HttpGet("list-hospital")]
        public async Task<ActionResult<List<Hospital>>> GetListHospital()
        {
            var listOfHospital = await _context.Hospitals.ToListAsync();
            return Ok(listOfHospital);

        }

        [HttpGet("list-hospital/name")]
        public async Task<ActionResult<Hospital>> GetListHospitalsByName(string name)
        {
            var hospitalHasName = await _context.Hospitals.
        Where(h => h.Name.Contains(name)).ToListAsync();
            return Ok(hospitalHasName);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<Hospital>> GetHospitalsByUserId(long userId)
        {
            var hospitalByUserId = await _context.Hospitals.FirstOrDefaultAsync(h => h.UserId == userId);
            return Ok(hospitalByUserId);
        }










    }
}
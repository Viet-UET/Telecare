using Data;
using DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;
using Models;

namespace Controllers
{
    // [ApiController]
    // [Route("[controller]")]
    public class MedicalSpecialtyController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public MedicalSpecialtyController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet("list-medicalspecialty")]
        public async Task<ActionResult<List<MedicalSpecialty>>> GetListMedicalSpecialty()
        {
            var listMS = await _context.MedicalSpecialties.ToListAsync();
            return Ok(listMS);
        }

        [HttpGet("medicalspecialty/{medicalspecialtyId}")]
        public async Task<ActionResult<MedicalSpecialty>> GetMedicalSpecialtyById([FromRoute] int medicalspecialtyId)
        {
            var MS = await _context.MedicalSpecialties.Where(m => m.MedicalSpecialtyId == medicalspecialtyId).FirstOrDefaultAsync();
            return Ok(MS);

        }


        //  fix
        [HttpGet("medicalspecialty/vi/{vi}")]
        public async Task<ActionResult<IEnumerable<MedicalSpecialty>>> GetMedicalSpecialtyByVi([FromRoute] string vi)
        {
            var ListMS = await _context.MedicalSpecialties
            .AsNoTracking()
            .Where(m => EF.Functions.Like(EF.Functions.Collate(m.VietnameseName, "Vietnamese_100_CI_AI"), $"%{vi}%"))
            .OrderBy(m => m.VietnameseName)
            .ToListAsync();

            return Ok(ListMS);

        }

        [HttpGet("medicalspecialty/en/{en}")]
        public async Task<ActionResult<IEnumerable<MedicalSpecialty>>> GetMedicalSpecialtyByEn([FromRoute] string en)
        {
            var ListMS = await _context.MedicalSpecialties
            .AsNoTracking()
            .Where(m => EF.Functions.Like(EF.Functions.Collate(m.EnglishName, "Latin1_General_100_CI_AI"), $"%{en}%"))
            .OrderBy(m => m.EnglishName)
            .ToListAsync();

            return Ok(ListMS);

        }


    }
}
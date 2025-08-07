using Data;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


    }
}
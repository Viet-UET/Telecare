using Data;
using Microsoft.AspNetCore.Mvc;

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


    }
}
using System.Drawing;
using Data;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Models;
using Models.Enums;
using System.Drawing.Imaging;


namespace Controllers
{

    public class ConversationController : ControllerBase
    {

        private readonly ApplicationDBContext _context;

        public ConversationController(ApplicationDBContext context)
        {
            _context = context;
        }


        [HttpGet("Conversation/{conversationId}/Mess")]
        public async Task<ActionResult<List<object>>> GetConversationMessage([FromRoute] long conversationId)
        {
            var listMess = await _context.Messages.Where(x => x.ConversationId == conversationId).OrderByDescending(x => x.Time).ToListAsync();
            return Ok(listMess);
        }


        [HttpGet("Conversation/{conversationId}/Att")]
        public async Task<ActionResult<List<object>>> GetConversationAtt([FromRoute] long conversationId)
        {
            var listAtt = await _context.Attachments.Where(x => x.ConversationId == conversationId).OrderByDescending(x => x.Time).ToListAsync();
            return Ok(listAtt);
        }



        [HttpPost("Conversation")]
        public async Task<ActionResult<long>> GetConversations([FromQuery] long doctorId, [FromQuery] long patientId)
        {

            var currentConv = await _context.Conversations.Where(x => x.PatientId == patientId && x.DoctorId == doctorId).FirstOrDefaultAsync();
            if (currentConv == null)
            {
                var conv = new Conversation
                {
                    PatientId = patientId,
                    DoctorId = doctorId,
                    StartTime = DateTime.Now,
                    State = ConversationState.KEEP
                };
                await _context.Conversations.AddAsync(conv);
                await _context.SaveChangesAsync();

                var convDb = await _context.Conversations.Where(x => x.PatientId == patientId && x.DoctorId == doctorId).FirstOrDefaultAsync();
                return Ok(convDb.ConversationId);
            }
            else
            {
                return Ok(currentConv.ConversationId);
            }
        }

        [HttpPost("Send_Message")]
        public async Task<ActionResult<object>> SendMess(MessageDTO messageDTO)
        {
            var mess = new Message
            {
                ConversationId = messageDTO.ConversationId,
                UserId = messageDTO.UserId,
                Text = messageDTO.Text,
                Time = DateTime.Now,
                State = MessageState.NOT_SEEN
            };

            await _context.Messages.AddAsync(mess);
            await _context.SaveChangesAsync();
            return Ok(mess);
        }


        [HttpPost("Send_Attachment")]
        public async Task<ActionResult<object>> SendAttachment(long conversationId, long userId, IFormFile image)
        {
            var attachmentId = await _context.Attachments.Where(a => a.ConversationId == conversationId).CountAsync() + 1;

            // đuôi file
            var ext = Path.GetExtension(image.FileName);

            var fileName = $"{conversationId}_{attachmentId}{ext}";

            var dir = Path.Combine(Directory.GetCurrentDirectory(), "attachment");
            var savePath = Path.Combine(dir, fileName);

            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            var imagePath = $"attachment/{fileName}";

            var att = new Attachment
            {
                ConversationId = conversationId,
                userId = userId,
                File = imagePath,
                Time = DateTime.UtcNow,
                State = MessageState.NOT_SEEN
            };


            await _context.Attachments.AddAsync(att);
            await _context.SaveChangesAsync();

            return Ok(att);

        }



        [HttpGet("Get_Attachment")]
        public async Task<ActionResult<object>> GetAttachment([FromQuery] string path)
        {
            var fullPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), path);

            if (System.IO.File.Exists(fullPath))
            {
                using var img = Image.FromFile(fullPath);
                var imageFormat = img.RawFormat.ToString().ToLower();
                Console.WriteLine(imageFormat);
                return PhysicalFile(fullPath, $"image/{imageFormat}");

            }
            else
            {
                return StatusCode(404);
            }


        }




    }
}
using System.Drawing;
using Data;
using DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Models;
using Models.Enums;

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



    }
}
using System;
using Models.Enums;

namespace Models
{
    public class Conversation
    {
        public long ConversationId { get; set; }
        public long PatientId { get; set; }
        public long DoctorId { get; set; }
        public DateTime? StartTime { get; set; }
        public ConversationState State { get; set; }


        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }


        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
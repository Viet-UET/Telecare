using Models.Enums;

namespace Models
{
    public class Attachment
    {
        public long AttachmentId { get; set; }
        public long ConversationId { get; set; }
        public long userId { get; set; } // FK -> User.userId
        public string File { get; set; }
        public DateTime? Time { get; set; }
        public MessageState State { get; set; }

        public Conversation Conversation { get; set; }
        public User User { get; set; }
    }
}
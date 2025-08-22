using Models.Enums;

namespace Models
{
    public class Message
    {
        public long MessageId { get; set; }
        public long ConversationId { get; set; }
        public long UserId { get; set; }
        public string Text { get; set; }
        public DateTime? Time { get; set; }
        public MessageState State { get; set; }

        public Conversation Conversation { get; set; }
        public User User { get; set; }
    }
}
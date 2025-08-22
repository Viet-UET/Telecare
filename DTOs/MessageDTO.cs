using Models.Enums;

namespace DTOs
{
    public class MessageDTO
    {
        public long ConversationId { get; set; }
        public long UserId { get; set; }
        public string Text { get; set; }
    }
}
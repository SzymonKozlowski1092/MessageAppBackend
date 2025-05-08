namespace MessageAppBackend.DTO.ChatDTOs
{
    public class SimpleChatDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastMessageContent { get; set; } = string.Empty;
        public string LastMessageSenderDisplayName { get; set; } = string.Empty;
        public DateTime LastMessageSentTime { get; set; }
    }
}

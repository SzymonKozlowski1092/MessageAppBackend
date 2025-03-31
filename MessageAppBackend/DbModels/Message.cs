﻿using Microsoft.VisualBasic;

namespace MessageAppBackend.DbModels
{
    public class Message
    {
        public Guid Id { get; set; }
        
        public Guid SenderId { get; set; }
        public User? Sender { get; set; }
        
        public Guid ChatId { get; set; }
        public Chat? Chat { get; set; }
        
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
}

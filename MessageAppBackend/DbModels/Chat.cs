﻿using System.ComponentModel.DataAnnotations;

namespace MessageAppBackend.DbModels
{
    public class Chat
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<UserChat>? Users { get; set; }
        public ICollection<Message>? Messages { get; set; }
    }
}

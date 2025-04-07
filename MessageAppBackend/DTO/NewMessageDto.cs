using MessageAppBackend.DbModels;
using System.ComponentModel.DataAnnotations;

namespace MessageAppBackend.DTO
{
    public class NewMessageDto
    {
        [Required(ErrorMessage = "Treść wiadomości nie może być pusta.")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Identyfikator nadawcy jest wymagany.")]
        public Guid SenderId { get; set; }

        [Required(ErrorMessage = "Identyfikator czatu jest wymagany.")]
        public Guid ChatId { get; set; }
    }
}


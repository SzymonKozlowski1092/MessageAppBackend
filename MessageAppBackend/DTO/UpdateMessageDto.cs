using System.ComponentModel.DataAnnotations;

namespace MessageAppBackend.DTO
{
    public class UpdateMessageDto
    {
        [Required(ErrorMessage = "Treść wiadomości nie może być pusta.")]
        public string Content = string.Empty;

        [Required(ErrorMessage = "Identyfikator wiadomości jest wymagany.")]
        public Guid Id { get; set; }

    }
}

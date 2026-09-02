using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTOs.Messages;

public class CreateMessageDto
{
    [Required]
    [StringLength(400, MinimumLength = 1)]
    public string MessageContent { get; set; } = string.Empty;
    
    [Required]
    public string ReceiverId { get; set; } = string.Empty;
}
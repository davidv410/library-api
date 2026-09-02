namespace LibraryApi.DTOs.Messages;

public class MessageResponseDto
{
    public int Id { get; set; }
    public string MessageContent { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
}
namespace LibraryApi.Models;

public class Message
{
    public int Id { get; set; }
    public string MessageContent { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public ApplicationUser Sender { get; set; } = null!;
    public string ReceiverId { get; set; } = string.Empty;
    public ApplicationUser Receiver { get; set; } = null!;
}
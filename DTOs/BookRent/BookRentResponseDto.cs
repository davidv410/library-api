namespace LibraryApi.DTOs.BookRent;

public class BookRentResponseDto
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public DateTime RentedAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public string UserId { get; set; } = string.Empty;
}
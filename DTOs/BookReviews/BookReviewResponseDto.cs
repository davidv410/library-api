namespace LibraryApi.DTOs.BookReviews;

public class BookReviewResponseDto
{
    public int Id { get; set; }
    public string Review { get; set; } = string.Empty;
    public int Rating { get; set; }
    public int LikeCount { get; set; }
    public int DislikeCount { get; set; }
    public int BookId { get; set; }
    public string UserId { get; set; } = string.Empty;
}
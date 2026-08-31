using LibraryApi.DTOs.BookReviews;

namespace LibraryApi.Services;
public interface IBookReviewService
{
    Task<IEnumerable<BookReviewResponseDto>?> GetBookReviews(int bookId);
    Task<BookReviewResponseDto?> CreateBookReview (int bookId, string userId, CreateBookReviewDto dto);
}
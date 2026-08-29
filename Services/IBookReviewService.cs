using LibraryApi.DTOs.BookReviews;

namespace LibraryApi.Services;
public interface IBookReviewService
{
    Task<BookReviewResponseDto> CreateBookReview (int bookId, string userId, CreateBookReviewDto dto);
}